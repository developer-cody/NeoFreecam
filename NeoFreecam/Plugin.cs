using BepInEx;
using Photon.Pun;
using NeoFreecam.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using GorillaLocomotion;
using GorillaNetworking;

namespace NeoFreecam
{
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject Head, HandL, HandR, FlyCamera;

        private bool rigConnected = true, guiEnabled = true, rigCanBeSeen = false, toggleNoclip;
        private static bool InModded => NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.ToLower().Contains("modded");
        public static bool lockedCursorState, NormalGameModes, ModdedGameModes;

        public static Plugin Intense { get; set; }
        public CamMovement camMovementScript;

        public static int Layer = 29, LayerMask = 1 << Layer;
        private LayerMask baseMask;

        string roomCode = "NEO";
        private static string emptyCodeCheck = "Join Code";
        private string code;

        private float speedSliderValue = 1.5f;

        void Start()
        {
            GorillaTagger.OnPlayerSpawned(InitializeGame);
            camMovementScript = FindObjectOfType<CamMovement>();
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
            if (InModded || !PhotonNetwork.InRoom)
            {
                ProcessGameActions();
            }
            else
            {
                RoomUtils.Disconnect();
            }
        }

        void ProcessGameActions()
        {
            HandleKeyboardInputs();

            if (rigConnected)
            {
                if (toggleNoclip)
                    EnableNoclip();
                else
                    DisableNoclip();
            }
            else
            {
                ResetColliders();
            }

            GorillaTagger.Instance.offlineVRRig.headBodyOffset.x = rigCanBeSeen ? 180f : 0f;

            if (PhotonNetwork.InRoom)
            {
                code = NetworkSystem.Instance.RoomName;
            }
        }

        void LateUpdate()
        {
            if (InModded || !PhotonNetwork.InRoom)
            {
                if (Keyboard.current.pKey.wasPressedThisFrame)
                {
                    rigCanBeSeen = !rigCanBeSeen;
                }
            }

            if (rigConnected)
            {
                GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
                GorillaTagger.Instance.transform.position = FlyCamera.transform.position;
                GorillaTagger.Instance.mainCamera.transform.rotation = FlyCamera.transform.rotation;
            }
        }
        
        private void HandleKeyboardInputs()
        {
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                guiEnabled = !guiEnabled;
            }

            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                rigConnected = !rigConnected;
            }

            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                toggleNoclip = !toggleNoclip;
            }

            if (Keyboard.current.rKey.isPressed)
            {
                Report(true);
            }
            else
            {
                Report(false);
            }
        }

        private void Report(bool yes)
        {
            Vector3 OldPOS = FlyCamera.transform.position;
            Vector3 POS = new Vector3(-61.8696f, 4.0192f, - 61.8069f);

            if (yes)
            {
                FlyCamera.transform.position = POS;
            }
            else
            {
                FlyCamera.transform.position = OldPOS;
            }
        }

        private void EnableNoclip()
        {
            baseMask = Player.Instance.locomotionEnabledLayers;

            Player.Instance.locomotionEnabledLayers = LayerMask;

            Player.Instance.bodyCollider.isTrigger = true;
            Player.Instance.headCollider.isTrigger = true;
        }

        private void DisableNoclip()
        {
            Player.Instance.locomotionEnabledLayers = baseMask;

            Player.Instance.bodyCollider.isTrigger = false;
            Player.Instance.headCollider.isTrigger = false;
        }

        private void ResetColliders()
        {
            Player.Instance.locomotionEnabledLayers = baseMask;
            Player.Instance.bodyCollider.isTrigger = false;
            Player.Instance.headCollider.isTrigger = false;
        }

        private void OnGUI()
        {
            float t = Mathf.PingPong(Time.time * .2f, 1);
            Color color = Color.HSVToRGB(t, 1, 1);
            GUI.color = color;

            float buttonWidth = 180f;
            float buttonHeight = 30f;
            float buttonX = 30f;
            float gamemodeButtonX = 250f;

            if (guiEnabled)
            {
                roomCode = GUI.TextArea(new Rect(buttonX, Screen.height - 150f, buttonWidth, buttonHeight), roomCode, 10);

                if (GUI.Button(new Rect(buttonX, Screen.height - 180f, buttonWidth, buttonHeight), emptyCodeCheck))
                {
                    RoomUtils.JoinRoomWithCode(roomCode);
                }

                if (PhotonNetwork.InRoom)
                {
                    if (GUI.Button(new Rect(buttonX, Screen.height - 210f, buttonWidth, buttonHeight), "Leave Room"))
                    {
                        RoomUtils.Disconnect();
                    }

                    DisplayRoomInfo();
                }
                else
                {
                    DisplayJoinButtons();
                }

                DisplayLiveStats();
                DisplaySpeedSlider();

                if (!NormalGameModes && !ModdedGameModes)
                {
                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 145f, buttonWidth, buttonHeight), "GameModes"))
                    {
                        NormalGameModes = !NormalGameModes;
                        if (NormalGameModes)
                        {
                            ModdedGameModes = false;
                        }
                    }
                }

                if (!NormalGameModes && !ModdedGameModes)
                {
                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 115f, buttonWidth, buttonHeight), "[M] GameModes"))
                    {
                        ModdedGameModes = !ModdedGameModes;
                        if (ModdedGameModes)
                        {
                            NormalGameModes = false;
                        }
                    }
                }

                if (NormalGameModes)
                {
                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 205f, buttonWidth, buttonHeight), "Casual"))
                    {
                        GorillaComputer.instance.currentGameMode.Value = "CASUAL";
                    }

                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 175f, buttonWidth, buttonHeight), "Infection"))
                    {
                        GorillaComputer.instance.currentGameMode.Value = "INFECTION";
                    }

                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 145f, buttonWidth, buttonHeight), "Guardian"))
                    {
                        GorillaComputer.instance.currentGameMode.Value = "GUARDIAN";
                    }
                    
                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 115f, buttonWidth, buttonHeight), "Freeze Tag"))
                    {
                        GorillaComputer.instance.currentGameMode.Value = "FREEZETAG";
                    }
                }

                if (ModdedGameModes)
                {
                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 205f, buttonWidth, buttonHeight), "[M] Casual"))
                    {
                        GorillaComputer.instance.currentGameMode.Value = "MODDED_CASUAL";
                    }

                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 175f, buttonWidth, buttonHeight), "[M] Infection"))
                    {
                        GorillaComputer.instance.currentGameMode.Value = "MODDED_INFECTION";
                    }

                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 145f, buttonWidth, buttonHeight), "[M] Guardian"))
                    {
                        GorillaComputer.instance.currentGameMode.Value = "MODDED_GUARDIAN";
                    }

                    if (GUI.Button(new Rect(gamemodeButtonX, Screen.height - 115f, buttonWidth, buttonHeight), "[M] Freeze Tag"))
                    {
                        GorillaComputer.instance.currentGameMode.Value = "MODDED_FREEZETAG";
                    }
                }

                if (NormalGameModes || ModdedGameModes)
                {
                    if (GUI.Button(new Rect(Screen.width - 250f, Screen.height - 80f, buttonWidth, buttonHeight), "Back"))
                    {
                        NormalGameModes = false;
                        ModdedGameModes = false;
                    }
                }
            }
        }

        private void DisplaySpeedSlider()
        {
            GUI.BeginGroup(new Rect(Screen.width - 220, 10, 200, 100));
            GUI.Box(new Rect(0, 0, 200, 100), "Settings");
            GUI.Label(new Rect(10, 30, 180, 20), "Speed:");

            speedSliderValue = GUI.HorizontalSlider(new Rect(10, 55, 180, 20), speedSliderValue, 0.5f, 5f);

            if (camMovementScript != null)
            {
                camMovementScript.movementSpeed = speedSliderValue;
            }

            GUI.EndGroup();
        }

        private void DisplayRoomInfo()
        {
            GUI.Label(new Rect(30f, Screen.height - 120, 170, 20), "Code: " + PhotonNetwork.CurrentRoom.Name);
            GUI.Label(new Rect(30f, Screen.height - 100, 170, 20), "Room user count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        private void DisplayJoinButtons()
        {
            if (GUI.Button(new Rect(30f, Screen.height - 210f, 180f, 30f), "Rejoin"))
            {
                #pragma warning disable CS4014
                RoomUtils.Rejoin(roomCode);
                #pragma warning disable CS4014
            }

            if (GUI.Button(new Rect(30f, Screen.height - 240f, 180f, 30f), "Generate Room"))
            {
                #pragma warning disable CS4014
                RoomUtils.Generate(roomCode);
                #pragma warning restore CS4014 
            }
        }

        private void DisplayLiveStats()
        {
            GUI.Label(new Rect(30f, Screen.height - 80, 300, 20), "Live regional player count: " + PhotonNetwork.CountOfPlayers);
            GUI.Label(new Rect(30f, Screen.height - 60, 300, 20), "Live regional Room count: " + PhotonNetwork.CountOfRooms);
        }
    }
}