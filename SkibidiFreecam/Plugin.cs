using BepInEx;
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

        void Start()
        {
            GorillaTagger.OnPlayerSpawned(InitializeGame);
        }

        void InitializeGame()
        {
            Intense = this;

            InitializeFlyCamera();
            ConfigureRig();
            CleanUpUnnecessaryComponents();
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

        private void OnGUI()
        {
            if (guiEnabled)
            {
                windowRect = GUI.Window(0, windowRect, DrawWindowContent, "Skibidi Freecam");
            }
        }

        private void DrawWindowContent(int windowID)
        {
            GUI.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, Screen.width, 20));
        }
    }
}