using SkibidiFreecam;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayCast : MonoBehaviour
{
    public LayerMask mask;
    LayerMask mask2;
    private Vector3 heldHandPosition;
    private Vector3 lastMousePosition;
    private bool isMouseMoving;
    private float sphereScale = 0.1f;
    private Vector3 lastSpherePosition;
    private float smoothFactor = 0.1f;

    void Start()
    {
        mask = LayerMask.GetMask(new string[] { "GorillaInteractable"/*, "Default"*/ });
        mask2 = LayerMask.GetMask(new string[] { "Prop" });

        lastMousePosition = Mouse.current.position.ReadValue();
    }

    void Update()
    {
        Vector3 currentMousePosition = Mouse.current.position.ReadValue();

        if (currentMousePosition != lastMousePosition)
        {
            isMouseMoving = true;
        }
        else
        {
            isMouseMoving = false;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            if (isMouseMoving)
            {
                Ray ray = Plugin.Intense.FlyCamera.GetComponent<Camera>().ScreenPointToRay(currentMousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 2f, ~mask2))
                {
                    heldHandPosition = hit.point;
                    GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                    Plugin.Intense.HandR.transform.position = hit.point;
                }
            }
        }
        else
        {
            ResetHandPositions();
        }

        HandleMouseWheel();

        lastMousePosition = currentMousePosition;
    }

    void HandleMouseWheel()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        sphereScale += scroll * 0.01f;

        if (sphereScale < 0.05f) sphereScale = 0.05f;
        if (sphereScale > 1f) sphereScale = 1f;
    }

    void ResetHandPositions()
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
