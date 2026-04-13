using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjects.DirectionalPanel
{
    public class DirectionalPanel : MonoBehaviour
    {
        public GameObject cameraGameObject; // Reference to the player's camera
        public float raycastRange = 2f; // Range of the raycast for detection

        private bool isInTriggerZone = false; // Flag to check if player is in trigger zone
        private Animator animator; // Animator component for animation control
        private AudioSource audioSource; // AudioSource component for playing sounds

        // Audio clips for buttons
        public AudioClip bottomButtonSound; // Sound for the bottom button
        public AudioClip topButtonSound; // Sound for the top button

        private bool isAnimating = false; // Flag to check if an animation is currently playing

        void Start()
        {
            // Initialize components
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            // Perform raycast only if the player is in the trigger zone
            if (isInTriggerZone)
            {
                // Create a ray from the camera's position in the forward direction
                Ray ray = new Ray(cameraGameObject.transform.position, cameraGameObject.transform.forward);
                RaycastHit hit;

                // Draw the ray in the editor for debugging
                Debug.DrawRay(ray.origin, ray.direction * raycastRange, Color.red);

                // Perform the raycast
                if (Physics.Raycast(ray, out hit, raycastRange))
                {
                    // Check which button was hit
                    if (Input.GetKeyDown(KeyCode.E) && !isAnimating)
                    {
                        if (hit.collider.name.Contains("Bottom_Button"))
                        {
                            /*
                             * This code is executed when the bottom button is used
                             */
                            animator.SetTrigger("PressBottomButton");
                            audioSource.PlayOneShot(bottomButtonSound);
                            StartCoroutine(PlayAnimationCoroutine("PressBottomButton"));
                        }
                        else if (hit.collider.name.Contains("Top_Button"))
                        {
                            /*
                             * This code is executed when the top button is used
                             */
                            animator.SetTrigger("PressTopButton");
                            audioSource.PlayOneShot(topButtonSound);
                            StartCoroutine(PlayAnimationCoroutine("PressTopButton"));
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Set flag when player enters the trigger zone
            if (other.CompareTag("Player"))
            {
                isInTriggerZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Clear flag when player exits the trigger zone
            if (other.CompareTag("Player"))
            {
                isInTriggerZone = false;
            }
        }

        IEnumerator PlayAnimationCoroutine(string triggerName)
        {
            isAnimating = true;

            // Wait for the animation to complete
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;

            // Wait for the duration of the animation
            yield return new WaitForSeconds(animationLength);

            isAnimating = false;
        }

    }
}