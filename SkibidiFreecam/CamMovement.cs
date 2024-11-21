using UnityEngine;
using UnityEngine.InputSystem;

namespace SkibidiFreecam
{
    public class CamMovement : MonoBehaviour
    {
        private const float HorizontalMultiplier = 6f;
        private const float VerticalMultiplier = 4.5f;
        public float movementSpeed = 1f;
        public float cameraMovement = .1f;

        public void Update()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            Vector3 forwardMovement = Plugin.Intense.FlyCamera.transform.forward * VerticalMultiplier;
            Vector3 rightMovement = Plugin.Intense.FlyCamera.transform.right * HorizontalMultiplier;

            Vector3 movementDirection = Vector3.zero;

            if (Keyboard.current.wKey.isPressed)
                movementDirection += forwardMovement;

            if (Keyboard.current.sKey.isPressed)
                movementDirection -= forwardMovement;

            if (Keyboard.current.aKey.isPressed)
                movementDirection -= rightMovement;

            if (Keyboard.current.dKey.isPressed)
                movementDirection += rightMovement;

            Plugin.Intense.FlyCamera.transform.position += movementDirection * movementSpeed * Time.deltaTime;
        }

        private void HandleRotation()
        {
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();

                Vector3 rotation = new Vector3(-mouseDelta.y, mouseDelta.x, 0) * cameraMovement;
                Plugin.Intense.FlyCamera.transform.eulerAngles += rotation;
            }
        }
    }
}