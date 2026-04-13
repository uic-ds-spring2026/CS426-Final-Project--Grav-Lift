using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CapsuleCharacter.Controller
{
    public class CapsuleCharacterController : MonoBehaviour
    {
        Vector3 movement;

        [SerializeField] Transform PlayerCamera;

        [SerializeField] float speed = 5f, mouseSensitivity = 700f;
        float verticalRotation = 0f;

        private void Start()
        {
            // Lock the cursor to the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
        }

        void FixedUpdate()
        {
            // Get movement input and apply it
            movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.fixedDeltaTime;
            transform.Translate(movement * speed);

            // Get mouse input and rotate the player horizontally
            float mouseX = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * mouseSensitivity;
            transform.Rotate(Vector3.up * mouseX);

            // Get mouse input and rotate the camera vertically
            float mouseY = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * mouseSensitivity;
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            PlayerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}