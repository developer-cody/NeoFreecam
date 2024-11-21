using GorillaNetworking;
using SkibidiFreecam;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayCast : MonoBehaviour
{
    public LayerMask interactableMask, propMask;
    private Vector3 lastMousePosition;
    private bool isMouseMoving;
    private const float RaycastDistance = 2f;
    private const float MouseDeadZone = 1f;

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
            if (Mouse.current.rightButton.isPressed)
            {
                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward * 0.5f + GorillaTagger.Instance.headCollider.transform.right * 0.2f;
                GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward * -0.5f + GorillaTagger.Instance.headCollider.transform.right * -0.2f;
            }
            else
            {
                ResetHandPositions();
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