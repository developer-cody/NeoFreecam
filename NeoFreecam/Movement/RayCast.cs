using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeoFreecam
{
    public class RayCast : MonoBehaviour
    {
        GameObject Sphere;
        void Start() => GorillaTagger.OnPlayerSpawned(InitStuff);

        void InitStuff()
        {
            PhotonNetworkController.Instance.disableAFKKick = true;
            Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Sphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            Sphere.GetComponent<Renderer>().material.color = Color.white;
            Sphere.transform.localScale = new Vector3(.1f, .1f, .1f);
            Sphere.name = "Mouse";
            Destroy(Sphere.GetComponent<SphereCollider>());

            ResetHandPositions();
        }

        void FixedUpdate()
        {
            LayerMask interactionMask = LayerMask.GetMask("Gorilla Trigger", "Zone");
            Camera activeCamera = Plugin.Intense.FlyCamera.GetComponent<Camera>();
            Ray ray = activeCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Mouse.current.leftButton.isPressed)
            {
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, ~interactionMask))
                {
                    GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hitInfo.point;

                    Sphere.SetActive(true);
                    Sphere.transform.position = hitInfo.point;
                }
            }
            else
            {
                Sphere.SetActive(false);
            }
        }

        private void HandleHandPositioning(Transform handTransform, bool isButtonPressed)
        {
            if (handTransform == null)
                return;

            Vector3 defaultPos = new Vector3(0f, -0.4f, 0.1f);
            Vector3 activePos = new Vector3(handTransform.localPosition.x, -0.12f, 0.65f);
            Vector3 defaultEulerAngles = new Vector3(0, -14.78f, -5f);

            handTransform.localPosition = isButtonPressed ? Vector3.Lerp(handTransform.localPosition, activePos, 0.2f) : defaultPos;
            handTransform.localEulerAngles = defaultEulerAngles;

            if (ReferenceEquals(handTransform, Plugin.Intense.HandR))
            {
                GorillaTagger.Instance.rightHandTriggerCollider.transform.position = handTransform.position;
            }
            else if (ReferenceEquals(handTransform, Plugin.Intense.HandL))
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