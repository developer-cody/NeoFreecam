using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeoFreecam
{
    public class RayCast : MonoBehaviour
    {
        GameObject Sphere;
        public static readonly LayerMask raycastLayerMask = ~LayerMask.GetMask(LayerMask.LayerToName(15), LayerMask.LayerToName(3), LayerMask.LayerToName(11));

        void Start()
        {
            PhotonNetworkController.Instance.disableAFKKick = true;
            Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(Sphere.GetComponent<SphereCollider>());
            Sphere.GetComponent<Renderer>().material = GorillaTagger.Instance.offlineVRRig.mainSkin.material;
            Sphere.transform.localScale = new Vector3(.1f, .1f, .1f);
            Sphere.name = "Mouse";
        }

        void Update()
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            mousePosition.z = 5f;
            mousePosition = Plugin.Intense.FlyCamera.GetComponent<Camera>().ScreenToWorldPoint(mousePosition);
            Sphere.transform.position = mousePosition;

            if (!Plugin.lockedCursorState)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    Camera activeCamera = GorillaTagger.Instance.thirdPersonCamera.activeInHierarchy
                        ? GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>()
                        : Camera.main;

                    Ray ray = activeCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                    LayerMask mask = LayerMask.GetMask("GorillaInteractable", "Default", "UI");

                    if (Physics.Raycast(ray, out RaycastHit hit, 10f, mask))
                    {
                        Debug.Log("Raycast hit: " + hit.collider.name);
                        GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                    }
                    else
                    {
                        Debug.Log("Raycast did not hit anything.");
                    }
                }
                else
                {
                    ResetHandPositions();
                }
            }
            else
            {
                HandleHandPositioning(Plugin.Intense.HandR.transform, Mouse.current.rightButton.isPressed);
                HandleHandPositioning(Plugin.Intense.HandL.transform, Mouse.current.leftButton.isPressed);
            }
        }

        private void HandleHandPositioning(Transform handTransform, bool isButtonPressed)
        {
            Vector3 defaultPos = new Vector3(0f, -0.4f, 0.1f);
            Vector3 defaultEulerAngles = handTransform == Plugin.Intense.HandL ? Vector3.zero : new Vector3(0, -14.78f, -5f);

            if (isButtonPressed)
            {
                handTransform.localPosition = new Vector3(handTransform.localPosition.x, -0.15f, 0.7f);
            }
            else
            {
                handTransform.localPosition = defaultPos;
            }

            handTransform.localEulerAngles = defaultEulerAngles;

            if (handTransform == Plugin.Intense.HandR)
            {
                GorillaTagger.Instance.rightHandTriggerCollider.transform.position = handTransform.position;
            }
            else
            {
                GorillaTagger.Instance.leftHandTriggerCollider.transform.position = handTransform.position;
            }
        }

        private void ResetHandPositions()
        {
            HandleHandPositioning(Plugin.Intense.HandL.transform, false);
            HandleHandPositioning(Plugin.Intense.HandR.transform, false);
        }
    }
}