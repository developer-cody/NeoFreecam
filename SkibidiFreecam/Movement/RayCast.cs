using GorillaNetworking;
using PlayFab.GroupsModels;
using SkibidiFreecam;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayCast : MonoBehaviour
{
    // Bools
    private bool isMouseMoving;

    // Layers
    public LayerMask interactableMask, propMask;

    // Vectors
    private Vector3 lastMousePosition;

    // Floats
    private const float RaycastDistance = 2f, MouseDeadZone = 1f;

    void Start()
    {
        PhotonNetworkController.Instance.disableAFKKick = true;

        interactableMask = LayerMask.GetMask("GorillaInteractable");
        propMask = LayerMask.GetMask("Prop");

        lastMousePosition = Mouse.current.position.ReadValue();
    }

    void Update()
    {
        Vector3 currentMousePosition = Mouse.current.position.ReadValue();
        CheckMouseMovement(currentMousePosition);

        if (!Plugin.lockedCursorState)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                HandleRaycast(currentMousePosition);
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

        lastMousePosition = currentMousePosition;
    }

    private void CheckMouseMovement(Vector3 currentMousePosition)
    {
        isMouseMoving = (currentMousePosition - lastMousePosition).magnitude > MouseDeadZone;
    }

    private void HandleRaycast(Vector3 currentMousePosition)
    {
        if (!isMouseMoving) return;

        Ray ray = Plugin.Intense.FlyCamera.GetComponent<Camera>().ScreenPointToRay(currentMousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RaycastDistance, ~propMask))
        {
            UpdateHandPosition(hit.point);
        }
    }

    private void UpdateHandPosition(Vector3 hitPoint)
    {
        GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hitPoint;
        Plugin.Intense.HandR.transform.position = hitPoint;
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