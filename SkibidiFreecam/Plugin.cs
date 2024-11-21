using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using SkibidiFreecam.Patches;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace SkibidiFreecam
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject Head, HandL, HandR, FlyCamera;
        private bool rigConnected = true;
        public static Plugin Intense { get; set; }
        public static int Layer = 29, LayerMask = 1 << Layer;
        private LayerMask baseMask;
        private Rect windowRect = new Rect(Screen.width - 420, 10, 400, 550);
        private bool guiEnabled;
        public static bool lockedCursorState;
        void Start()
        {
            GorillaTagger.OnPlayerSpawned(InitializeGame);
        }
        bool Toggle;
        void OnEnable()
        {
            Toggle = true;
            HarmonyPatches.ApplyHarmonyPatches();
        }
        void OnDisable()
        {
            Toggle = false;
            HarmonyPatches.RemoveHarmonyPatches();
        }
        void InitializeGame()
        {
            if (!vrheadset && Toggle)
            {
                InitializeFlyCamera();
                ConfigureRig();
                CleanUpUnnecessaryComponents();
            }
            Intense = this;
        }
        void CleanUpUnnecessaryComponents()
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(xrDisplaySubsystems);

            if (xrDisplaySubsystems.Count > 0 && xrDisplaySubsystems[0].running)
            {
                vrheadset = true;
            }
            else
            {
                vrheadset = false;
                if (!vrheadset && Toggle)
                {
                    var controllerHandler = GorillaTagger.Instance.GetComponent("ConnectedControllerHandler");
                    Destroy(controllerHandler);

                    Destroy(GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>());
                    Destroy(GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<TransformFollow>());
                }
            }
        }
        void ConfigureRig()
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(xrDisplaySubsystems);

            if (xrDisplaySubsystems.Count > 0 && xrDisplaySubsystems[0].running)
            {
                vrheadset = true;
            }
            else
            {
                vrheadset = false;
                if (!vrheadset && Toggle)
                {
                    GorillaTagger.Instance.offlineVRRig.head.overrideTarget = GorillaTagger.Instance.mainCamera.transform;
                    GorillaTagger.Instance.offlineVRRig.leftHand.overrideTarget = HandL.transform;
                    GorillaTagger.Instance.offlineVRRig.rightHand.overrideTarget = HandR.transform;
                }
            }
        }
        void InitializeFlyCamera()
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(xrDisplaySubsystems);

            if (xrDisplaySubsystems.Count > 0 && xrDisplaySubsystems[0].running)
            {
                vrheadset = true;
            }
            else
            {
                vrheadset = false;
                if (!vrheadset && Toggle)
                {
                    FlyCamera = new GameObject("FlyCamera");
                    HandL = new GameObject("LeftHand");
                    HandR = new GameObject("RightHand");

                    HandL.transform.SetParent(Camera.main.transform);
                    HandR.transform.SetParent(Camera.main.transform);
                    GorillaTagger.Instance.leftHandTransform.SetParent(Camera.main.transform);
                    GorillaTagger.Instance.rightHandTransform.SetParent(Camera.main.transform);

                    FlyCamera.AddComponent<CamMovement>();
                    FlyCamera.AddComponent<RayCast>();
                    FlyCamera.AddComponent<Camera>();
                    FlyCamera.GetComponent<Camera>().fieldOfView = 90f;
                    FlyCamera.GetComponent<Camera>().nearClipPlane = 0.01f;

                    FlyCamera.transform.position = Camera.main.transform.position;

                    Destroy(GorillaTagger.Instance.thirdPersonCamera);
                }
            }
        }
        bool vrheadset = true;
        string RoomCode = "";
        async void OnGUI()
        {
            if (!vrheadset && Toggle)
            {
                GUI.color = Color.red;
                float buttonWidth = 180f;
                float buttonHeight = 30f;
                float buttonX = 30f;
                RoomCode = GUI.TextArea(new Rect(buttonX, Screen.height - 170f, buttonWidth, buttonHeight), RoomCode, 10);
                if (GUI.Button(new Rect(buttonX, Screen.height - 200f, buttonWidth, buttonHeight), emptycodecheck))
                {
                    if (RoomCode != "" && emptycodecheck == "Join Code")
                    {
                        emptycodecheck = "Join Code";
                        if (PhotonNetwork.InRoom)
                        {
                            await NetworkSystem.Instance.ReturnToSinglePlayer();
                        }
                        PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(RoomCode, GorillaNetworking.JoinType.Solo);
                    }
                    else
                    {
                        if (emptycodecheck != "empty room!")
                        {
                            emptycodecheck = "empty room!";
                            await wait3000();
                        }
                    }
                }
                if (PhotonNetwork.InRoom)
                {
                    if (GUI.Button(new Rect(buttonX, Screen.height - 290f, buttonWidth, buttonHeight), "Leave Room"))
                    {
                        await NetworkSystem.Instance.ReturnToSinglePlayer();
                    }
                }
                if (PhotonNetwork.CurrentRoom != null)
                {
                    GUI.Label(new Rect(buttonX, Screen.height - 140, 170, 20), "Code: " + PhotonNetwork.CurrentRoom.Name);
                    GUI.Label(new Rect(buttonX, Screen.height - 120, 170, 20), "Room user count: " + PhotonNetwork.CurrentRoom.PlayerCount);
                }
                if (PhotonNetwork.InRoom)
                {
                    if (GUI.Button(new Rect(buttonX, Screen.height - 230f, buttonWidth, buttonHeight), "Rejoin"))
                    {
                        await Rejoin();
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(buttonX, Screen.height - 200f, buttonWidth, buttonHeight), "Generate Room"))
                    {
                        await Generate();
                    }
                }
                GUI.Label(new Rect(buttonX, Screen.height - 100, 300, 20), "Live regional player count: " + PhotonNetwork.CountOfPlayers);
                GUI.Label(new Rect(buttonX, Screen.height - 80, 300, 20), "Live regional Room count: " + PhotonNetwork.CountOfRooms);
            }
        }
        private async Task Generate()
        {
            string code = null;

            if (NetworkSystem.Instance.InRoom)
            {
                await NetworkSystem.Instance.ReturnToSinglePlayer();
            }

            await Task.Delay(1000);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, GorillaNetworking.JoinType.Solo);
        }
        private static string emptycodecheck = "Join Code";
        private async Task Rejoin()
        {
            string code = NetworkSystem.Instance.RoomName;

            if (NetworkSystem.Instance.InRoom)
            {
                code = NetworkSystem.Instance.RoomName;
                await NetworkSystem.Instance.ReturnToSinglePlayer();
            }


            await Task.Delay(3000);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, GorillaNetworking.JoinType.Solo);
        }
        public void HandleRigCONST()
        {
            if (!vrheadset && Toggle)
            {
                GorillaTagger.Instance.offlineVRRig.head.overrideTarget = GorillaTagger.Instance.mainCamera.transform;
                GorillaTagger.Instance.offlineVRRig.leftHand.overrideTarget = HandL.transform;
                GorillaTagger.Instance.offlineVRRig.rightHand.overrideTarget = HandR.transform;
            }
        }
        private async Task wait3000()
        {
            await Task.Delay(3000);
            emptycodecheck = "Join Room";
        }

        void Update()
        {
            if (!vrheadset && Toggle)
            {
                if (Keyboard.current.hKey.wasPressedThisFrame)
                {
                    guiEnabled = !guiEnabled;
                }

                if (Keyboard.current.cKey.wasPressedThisFrame)
                {
                    rigConnected = !rigConnected;
                }

                if (Keyboard.current.lKey.wasPressedThisFrame)
                {
                    lockedCursorState = !lockedCursorState;
                }

                if (lockedCursorState)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                if (rigConnected)
                {
                    baseMask = GorillaLocomotion.Player.Instance.locomotionEnabledLayers;
                    GorillaLocomotion.Player.Instance.locomotionEnabledLayers = LayerMask;
                    GorillaLocomotion.Player.Instance.bodyCollider.isTrigger = true;
                    GorillaLocomotion.Player.Instance.headCollider.isTrigger = true;
                }
                else
                {
                    GorillaLocomotion.Player.Instance.locomotionEnabledLayers = baseMask;
                    GorillaLocomotion.Player.Instance.bodyCollider.isTrigger = false;
                    GorillaLocomotion.Player.Instance.headCollider.isTrigger = false;
                }
            }
        }

        void LateUpdate()
        {
            if (rigConnected && !vrheadset && Toggle)
            {
                GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
                GorillaTagger.Instance.transform.position = FlyCamera.transform.position;
                GorillaTagger.Instance.mainCamera.transform.rotation = FlyCamera.transform.rotation;
            }
        }
    }
}
