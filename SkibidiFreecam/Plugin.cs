using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using SkibidiFreecam.Movement;
using SkibidiFreecam.Patches;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

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
        string RoomCode = "";

        void Start()
        {
            GorillaTagger.OnPlayerSpawned(InitializeGame);
        }

        void InitializeGame()
        {
            InitializeFlyCamera();
            ConfigureRig();
            CleanUpUnnecessaryComponents();
            Intense = this;
        }

        void InitializeFlyCamera()
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

        void ConfigureRig()
        {
            GorillaTagger.Instance.offlineVRRig.head.overrideTarget = GorillaTagger.Instance.mainCamera.transform;
            GorillaTagger.Instance.offlineVRRig.leftHand.overrideTarget = HandL.transform;
            GorillaTagger.Instance.offlineVRRig.rightHand.overrideTarget = HandR.transform;
        }

        void CleanUpUnnecessaryComponents()
        {
            var controllerHandler = GorillaTagger.Instance.GetComponent("ConnectedControllerHandler");
            Destroy(controllerHandler);

            Destroy(GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>());
            Destroy(GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<TransformFollow>());
        }

        void Update()
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

            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                patchcontrollers.fingers = "Rgrip, Rindex, Rthumb";
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

        void LateUpdate()
        {
            if (rigConnected)
            {
                GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
                GorillaTagger.Instance.transform.position = FlyCamera.transform.position;
                GorillaTagger.Instance.mainCamera.transform.rotation = FlyCamera.transform.rotation;
            }
        }

        private async void OnGUI()
        {
            float t = Mathf.PingPong(Time.time * 0.8f, 1);
            Color color = Color.HSVToRGB(t, 1, 1);
            GUI.color = color;

            float buttonWidth = 180f;
            float buttonHeight = 30f;
            float buttonX = 30f;

            if (guiEnabled)
            {
                RoomCode = GUI.TextArea(new Rect(buttonX, Screen.height - 150f, buttonWidth, buttonHeight), RoomCode, 10);

                if (GUI.Button(new Rect(buttonX, Screen.height - 180f, buttonWidth, buttonHeight), emptycodecheck))
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
                    else if (RoomCode == "")
                    {
                        if (emptycodecheck != "empty room!")
                        {
                            emptycodecheck = "empty room!";
                            await Task.Delay(3000);
                        }
                    }
                }

                if (PhotonNetwork.InRoom)
                {
                    if (GUI.Button(new Rect(buttonX, Screen.height - 210f, buttonWidth, buttonHeight), "Leave Room"))
                    {
                        await NetworkSystem.Instance.ReturnToSinglePlayer();
                    }
                }

                if (PhotonNetwork.CurrentRoom != null)
                {
                    GUI.Label(new Rect(buttonX, Screen.height - 120, 170, 20), "Code: " + PhotonNetwork.CurrentRoom.Name);
                    GUI.Label(new Rect(buttonX, Screen.height - 100, 170, 20), "Room user count: " + PhotonNetwork.CurrentRoom.PlayerCount);
                }

                if (!PhotonNetwork.InRoom)
                {
                    if (GUI.Button(new Rect(buttonX, Screen.height - 210f, buttonWidth, buttonHeight), "Rejoin"))
                    {
                        await Rejoin();
                    }

                    if (GUI.Button(new Rect(buttonX, Screen.height - 240f, buttonWidth, buttonHeight), "Generate Room"))
                    {
                        await Generate();
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(buttonX, Screen.height - 10000f, buttonWidth, buttonHeight), "Generate Room"))
                    {

                    }
                }

                GUI.Label(new Rect(buttonX, Screen.height - 80, 300, 20), "Live regional player count: " + PhotonNetwork.CountOfPlayers);
                GUI.Label(new Rect(buttonX, Screen.height - 60, 300, 20), "Live regional Room count: " + PhotonNetwork.CountOfRooms);
            }
        }

        private async Task Generate()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(RoomCode, GorillaNetworking.JoinType.Solo);
            }

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
            string code = PhotonNetwork.CurrentRoom.ToString();

            if (PhotonNetwork.InRoom)
            {
                code = NetworkSystem.Instance.RoomName;
                await NetworkSystem.Instance.ReturnToSinglePlayer();
            }

            await Task.Delay(3000);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, GorillaNetworking.JoinType.Solo);
        }
    }
}