using UnityEngine;
using UnityEngine.InputSystem;

namespace NeoFreecam.Movement
{
    public class CamMovement : MonoBehaviour
    {
        private Vector2 lastMousePosition = Vector2.zero;

        private const float HorizontalMultiplier = 6f, VerticalMultiplier = 4.5f;
        public float movementSpeed = 1.5f, cameraMovement = .1f, smoothFactor = 0.5f, currentRotationX = 0f;

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

            var CurrentKey = Keyboard.current;

            if (CurrentKey.wKey.isPressed)
            {
                movementDirection += forwardMovement;
            }
            
            if (CurrentKey.sKey.isPressed)
            {
                movementDirection -= forwardMovement;
            }
            
            if (CurrentKey.dKey.isPressed)
            {
                movementDirection += rightMovement;
            }
            
            if (CurrentKey.aKey.isPressed)
            {
                movementDirection -= rightMovement;
            }

            Plugin.Intense.FlyCamera.transform.position += movementDirection * movementSpeed * Time.deltaTime;
        }

        private void HandleRotation()
        {
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                Vector2 smoothedDelta = Vector2.Lerp(lastMousePosition, mouseDelta, smoothFactor);
                lastMousePosition = smoothedDelta;

                float rotationX = -smoothedDelta.y * cameraMovement;
                float rotationY = smoothedDelta.x * cameraMovement;

                currentRotationX += rotationX;
                currentRotationX = Mathf.Clamp(currentRotationX, -90f, 90f);

                Plugin.Intense.FlyCamera.transform.eulerAngles += new Vector3(rotationX, rotationY, 0);
            }
        }
    }
}