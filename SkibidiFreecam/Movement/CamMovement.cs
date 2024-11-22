using UnityEngine;
using UnityEngine.InputSystem;

namespace SkibidiFreecam.Movement
{
    public class CamMovement : MonoBehaviour
    {
        // Vectors
        private Vector2 lastMousePosition = Vector2.zero;

        // Float
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
            if (Mouse.current.rightButton.isPressed && !Plugin.lockedCursorState || Plugin.lockedCursorState)
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