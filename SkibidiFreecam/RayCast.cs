using SkibidiFreecam;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class RayCast : MonoBehaviour
{
    public LayerMask mask;
    LayerMask mask2;
    GameObject Line;

    void Start()
    {
        mask = LayerMask.GetMask("GorillaInteractable");
        mask2 = LayerMask.GetMask(new string[] { "Gorilla Trigger", "Zone", "Gorilla Body", "default" });
        Line = new GameObject("LineRender", typeof(LineRenderer));
        Line.GetComponent<LineRenderer>().material.shader = GorillaTagger.Instance.offlineVRRig.mainSkin.material.shader;
    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Ray ray = Plugin.Intense.FlyCamera.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue()); 
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f, ~mask2))
            {
                    GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                    Plugin.Intense.HandR.transform.position = Vector3.Lerp(Plugin.Intense.HandR.transform.position, hit.point, 1f);
                
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
