using SkibidiFreecam;
using UnityEngine.InputSystem;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    public LayerMask mask;
    LayerMask mask2;

    void Start()
    {
        mask = LayerMask.GetMask("GorillaInteractable", "default");
        mask2 = LayerMask.GetMask(new string[] { "Gorilla Trigger", "Zone", "Gorilla Body" });
    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); 
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f, mask))
            {
                GorillaTagger.Instance.rightHandTransform.transform.position = hit.point;
                GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                Plugin.Intense.HandR.transform.position = hit.point;
            }
            
            if (Physics.Raycast(ray, out hit, 10f, mask2))
            {
                GorillaTagger.Instance.rightHandTransform.transform.position = hit.point;
                GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                Plugin.Intense.HandR.transform.position = hit.point;
            }
        }
        else
        {
            ResetHandPositions();
        }
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
