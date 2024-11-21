using SkibidiFreecam;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayCast : MonoBehaviour
{
    public LayerMask interactableMask, propMask;
    private Vector3 lastMousePosition;
    private bool isMouseMoving;
    private float sphereScale = 0.1f;

    private const float RaycastDistance = 2f;
    private const float MinSphereScale = 0.05f;
    private const float MaxSphereScale = 1f;
    private const float ScrollSensitivity = 0.01f;

    void Start()
    {
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
                HandleMouseWheel();
            }
        }
        else
        {
            ResetHandPositions();
        }

        lastMousePosition = currentMousePosition;
    }

    private void CheckMouseMovement(Vector3 currentMousePosition)
    {
        isMouseMoving = currentMousePosition != lastMousePosition;
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

    private void HandleMouseWheel()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            sphereScale += scroll * ScrollSensitivity;

            sphereScale = Mathf.Clamp(sphereScale, MinSphereScale, MaxSphereScale);

            Plugin.Intense.HandR.transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
        }
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
