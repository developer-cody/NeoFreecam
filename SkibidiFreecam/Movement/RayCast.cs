using GorillaNetworking;
using SkibidiFreecam;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayCast : MonoBehaviour
{
    // Layers
    public static readonly LayerMask raycastLayerMask = ~LayerMask.GetMask(LayerMask.LayerToName(15), LayerMask.LayerToName(3), LayerMask.LayerToName(11));
    //public LayerMask interactableMask, propMask;

    // Floats
    private const float RaycastDistance = 10f, MouseDeadZone = 1f;

    GameObject sphere;

    void Start()
    {
        PhotonNetworkController.Instance.disableAFKKick = true;
        //interactableMask = LayerMask.GetMask("GorillaInteractable");
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(sphere.GetComponent<SphereCollider>());
        sphere.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.mainSkin.material.shader;
        sphere.transform.localScale = new Vector3(.1f, .1f, .1f);
        sphere.name = "Mouse";
    }

    void Update()
    {
        Vector3 mousepositon = Mouse.current.position.ReadValue();
        mousepositon.z = 5f;
        mousepositon = Plugin.Intense.FlyCamera.GetComponent<Camera>().ScreenToWorldPoint(mousepositon);
        sphere.transform.position = mousepositon;

        if (!Plugin.lockedCursorState)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Ray ray;
                LayerMask mask = LayerMask.GetMask(new string[] { "Gorilla Trigger", "Zone", "Gorilla Body" });
                if (GorillaTagger.Instance.thirdPersonCamera.activeInHierarchy) { ray = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>().ScreenPointToRay(Mouse.current.position.value); } else { ray = Camera.main.ScreenPointToRay(Mouse.current.position.value); }
                if (Physics.Raycast(ray, out RaycastHit hit, 5f, ~mask)) { GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point; }
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