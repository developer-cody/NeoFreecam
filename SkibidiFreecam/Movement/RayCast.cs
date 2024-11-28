using GorillaNetworking;
using PlayFab.GroupsModels;
using SkibidiFreecam;
using System;
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
    private const float RaycastDistance = 10f, MouseDeadZone = 1f;

    GameObject sphere;

    void Start()
    {
        PhotonNetworkController.Instance.disableAFKKick = true;

        interactableMask = LayerMask.GetMask("GorillaInteractable");

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
                HandleRaycast();
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


    private void HandleRaycast()
    {
        Ray ray = Plugin.Intense.FlyCamera.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit,100,interactableMask))
        {
            UpdateHandPosition(hit.transform.position);
            Debug.Log("hit: " + hit.transform.gameObject.name);
        }

    }

    private void UpdateHandPosition(Vector3 hitPoint)
    {
        GorillaTagger.Instance.rightHandTransform.transform.position = hitPoint;

        GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hitPoint;

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