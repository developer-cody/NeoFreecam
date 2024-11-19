using System;
using BepInEx;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilla;

namespace SkibidiFreecam
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject Head;
        public GameObject HandL;
        public GameObject HandR;
        public GameObject FlyCamera;
        bool rigconnected;
        public static Plugin Intense {  get; set; }
        public static int layer = 29, layerMask = 1 << layer;
        private LayerMask baseMask;
        void Start()
        {
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void OnGameInitialized()
        {
            Intense = this;
            FlyCamera = new GameObject();
            HandL = new GameObject();
            HandR = new GameObject();
            HandL.transform.parent = Camera.main.transform;
            HandR.transform.parent = Camera.main.transform;
            GorillaTagger.Instance.leftHandTransform.parent = Camera.main.transform;
            GorillaTagger.Instance.rightHandTransform.parent = Camera.main.transform;
            HandR.transform.parent = Camera.main.transform;
            FlyCamera.name = "FlyCamera";
            FlyCamera.AddComponent<CamMovement>();
            FlyCamera.AddComponent<RayCast>();
            FlyCamera.transform.position = Camera.main.transform.position;
            FlyCamera.AddComponent<Camera>();
            FlyCamera.GetComponent<Camera>().fieldOfView = 90f;
            FlyCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
            Destroy(GorillaTagger.Instance.thirdPersonCamera);
            HandleRigCONST();
            rigconnected = true;
            HandL.transform.localPosition = new Vector3(0f, -0.4f, 0.1f);
            HandL.transform.localEulerAngles = new Vector3(0, -265.4166f, 0);
            HandR.transform.localPosition = new Vector3(0f, -0.4f, 0.1f);
            HandR.transform.localEulerAngles = new Vector3(0, 265.4166f, 0);
            var componet = GorillaTagger.Instance.GetComponent("ConnectedControllerHandler");
            Destroy(componet);
            Destroy(GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>());
            Destroy(GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<TransformFollow>());

        }

        public void HandleRigCONST()
        {

            GorillaTagger.Instance.offlineVRRig.head.overrideTarget = GorillaTagger.Instance.mainCamera.transform;
            GorillaTagger.Instance.offlineVRRig.leftHand.overrideTarget = HandL.transform;
            GorillaTagger.Instance.offlineVRRig.rightHand.overrideTarget = HandR.transform;
        }

        void Update()
        {
            if(Keyboard.current.cKey.wasPressedThisFrame)
            {
                rigconnected = !rigconnected;
            }
           if(rigconnected)
            {
                baseMask = GorillaLocomotion.Player.Instance.locomotionEnabledLayers;
                GorillaLocomotion.Player.Instance.locomotionEnabledLayers = layerMask;
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
        public void LateUpdate()
        {
            if (rigconnected)
            {
                GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
                GorillaTagger.Instance.transform.position = FlyCamera.transform.position;
                GorillaTagger.Instance.mainCamera.transform.rotation = FlyCamera.transform.rotation;



            }

        }
    }
}
