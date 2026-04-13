using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjects.LightValve
{
    public class LightValveController : MonoBehaviour
    {
        public Light lightSource; // Reference to the light
        public Transform valve; // Reference to the valve object
        public float maxRotationAngle = 720f; // Maximum rotation angle for the valve
        public float minIntensity = 0f; // Minimum intensity of the light
        public float maxIntensity = 3f; // Maximum intensity of the light
        public float initialIntensity = 0f; // Initial intensity of the light
        public float rotationSpeed = 100f; // Speed of the valve rotation

        private float currentRotationAngle = 0f; // Current rotation angle of the valve
        private Quaternion initialRotation; // Initial rotation of the valve
        private bool canTurnValve = false; // Whether the player can turn the valve

        [SerializeField] AudioClip mySound;
        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            // Prevent values ​​from exceeding limits
            if (initialIntensity > maxIntensity)
            {
                initialIntensity = maxIntensity;
            }
            if (initialIntensity < minIntensity)
            {
                initialIntensity = minIntensity;
            }
        }

        void Start()
        {
            initialRotation = valve.localRotation;  // Save the initial rotation

            // Set initial intensity (to modify if you want to manage something else)
            lightSource.intensity = initialIntensity;

            // Calculate the initial rotation angle based on the emission rate
            float initialRotationAngle = Mathf.Lerp(0f, maxRotationAngle, (initialIntensity - minIntensity) / (maxIntensity - minIntensity));

            // Get the current local rotation angles
            Vector3 localAngles = valve.localEulerAngles;

            // Apply the calculated rotation angle only on the Z axis (or the appropriate axis)
            localAngles.z = initialRotationAngle;
            valve.localEulerAngles = localAngles;

            // Set current rotation angle to match the initial rotation
            currentRotationAngle = initialRotationAngle;
        }

        void Update()
        {
            if (!canTurnValve) return; // Exit if the player cannot turn the valve

            float rotationInput = 0f;

            // Check if the corresponding key is pressed
            if (Input.GetKey(KeyCode.Q))
            {
                /*
                 * This code is executed when the valve is turned to the left
                 */
                rotationInput = -rotationSpeed * Time.deltaTime; // Rotate counterclockwise
            }
            else if (Input.GetKey(KeyCode.E))
            {
                /*
                 * This code is executed when the valve is turned to the right
                 */
                rotationInput = rotationSpeed * Time.deltaTime; // Rotate clockwise
            }

            ManageSound(rotationInput);
            AdjustValve(rotationInput);
        }

        void AdjustValve(float rotationInput)
        {
            // Calculate the new rotation angle
            float newRotationAngle = currentRotationAngle + rotationInput;
            newRotationAngle = Mathf.Clamp(newRotationAngle, 0f, maxRotationAngle);

            // Update the current rotation angle
            currentRotationAngle = newRotationAngle;

            // Get the current local rotation angles
            Vector3 localAngles = valve.localEulerAngles;

            // Apply the new rotation angle only on the Z axis (or the appropriate axis)
            localAngles.z = newRotationAngle;
            valve.localEulerAngles = localAngles;

            // Adjust the light intensity based on the rotation (to modify if you want to manage something else)
            lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, newRotationAngle / maxRotationAngle);
        }

        void ManageSound(float rotationInput)
        {
            // Play or stop the sound based on rotation input and current rotation angle
            if (rotationInput != 0f && currentRotationAngle != maxRotationAngle && currentRotationAngle != 0) // If there is a rotation input and the valve has not reached its angle limit
            {
                if (!audioSource.isPlaying) // Play the sound if it's not already playing
                {
                    audioSource.clip = mySound;
                    audioSource.Play();
                }
            }
            else // If no rotation input
            {
                if (audioSource.isPlaying) // Stop the sound if it's playing
                {
                    audioSource.Stop();
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) // Check if the player entered the trigger
            {
                canTurnValve = true; // Allow turning the valve
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) // Check if the player exited the trigger
            {
                canTurnValve = false; // Disallow turning the valve
                audioSource.Stop();
            }
        }
    }
}