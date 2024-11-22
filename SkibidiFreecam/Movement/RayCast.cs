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

    // Vecotrs
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
            // Right Hand Stuff
            Plugin.Intense.HandR.transform.localEulerAngles = new Vector3(0, -14.78f, -5f);

            Plugin.Intense.HandR.transform.localPosition = new Vector3(0.2f, -0.15f, .3f);
            GorillaTagger.Instance.rightHandTransform.localPosition = Plugin.Intense.HandL.transform.localPosition;
            GorillaTagger.Instance.rightHandTriggerCollider.transform.position = Plugin.Intense.HandR.transform.position;

            if (Mouse.current.rightButton.isPressed)
            {
                Plugin.Intense.HandR.transform.localPosition = new Vector3(0f, -.15f, .7f);
                GorillaTagger.Instance.rightHandTransform.localPosition = Plugin.Intense.HandL.transform.localPosition;
                GorillaTagger.Instance.rightHandTriggerCollider.transform.position = Plugin.Intense.HandR.transform.position;
            }

            // Left Hand Stuff
            Plugin.Intense.HandL.transform.localEulerAngles = Vector3.zero;

            Plugin.Intense.HandL.transform.localPosition = new Vector3(-0.2f, -0.15f, .3f);
            GorillaTagger.Instance.leftHandTransform.localPosition = Plugin.Intense.HandL.transform.localPosition;
            GorillaTagger.Instance.leftHandTriggerCollider.transform.position = Plugin.Intense.HandL.transform.position;

            if (Mouse.current.leftButton.isPressed)
            {
                Plugin.Intense.HandL.transform.localPosition = new Vector3(-0, -.15f, .7f);
                GorillaTagger.Instance.leftHandTransform.localPosition = Plugin.Intense.HandL.transform.localPosition;
                GorillaTagger.Instance.leftHandTriggerCollider.transform.position = Plugin.Intense.HandL.transform.position;
            }
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

    private void ResetHandPositions()
    {
        Vector3 defaultPos = new Vector3(0f, -0.4f, 0.1f);
        Vector3 leftHandEulerAngles = new Vector3(0, -265.4166f, 0);
        Vector3 rightHandEulerAngles = new Vector3(0, 265.4166f, 0);

        GorillaTagger.Instance.leftHandTransform.transform.position = defaultPos;
        GorillaTagger.Instance.leftHandTriggerCollider.transform.position = defaultPos;
        Plugin.Intense.HandL.transform.localPosition = defaultPos;
        Plugin.Intense.HandL.transform.localEulerAngles = leftHandEulerAngles;

        Plugin.Intense.HandR.transform.localPosition = defaultPos;
        Plugin.Intense.HandR.transform.localEulerAngles = rightHandEulerAngles;
    }
}