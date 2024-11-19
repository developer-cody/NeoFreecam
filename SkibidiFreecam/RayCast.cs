using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SkibidiFreecam
{
    public class RayCast : MonoBehaviour
    {
        public LayerMask mask;
        public void Start ()
        {
           mask = LayerMask.GetMask("GorillaInteractable", "default");
        }
        public void LateUpdate()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                RaycastHit hit;
                if (Physics.Raycast(Plugin.Intense.FlyCamera.transform.position, Plugin.Intense.FlyCamera.transform.forward, out hit, Mathf.Infinity, mask))
                {
                    Debug.Log(hit.transform.gameObject);
                    GorillaTagger.Instance.rightHandTransform.transform.position = hit.point;
                    GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                    Plugin.Intense.HandR.transform.position = hit.point;
                }

            }
            else
            {
                GorillaTagger.Instance.leftHandTransform.transform.position = new Vector3(0f, -0.4f, 0.1f);
                GorillaTagger.Instance.leftHandTriggerCollider.transform.position = new Vector3(0f, -0.4f, 0.1f);
                Plugin.Intense.HandL.transform.localPosition = new Vector3(0f, -0.4f, 0.1f);
                Plugin.Intense.HandL.transform.localEulerAngles = new Vector3(0, -265.4166f, 0);
                Plugin.Intense.HandR.transform.localPosition = new Vector3(0f, -0.4f, 0.1f);
                Plugin.Intense.HandR.transform.localEulerAngles = new Vector3(0, 265.4166f, 0);
            }



        }
    }
}
