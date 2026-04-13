using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Presets;
using UnityEngine;

namespace InteractiveObjects.Button
{
    // This code can be used for push buttons
    public class ButtonInteractionController : MonoBehaviour
    {
        Animator animator;

        [SerializeField] AudioClip mySound;
        AudioSource audioSource;

        bool isInTriggerZone = false; // Is the player in the trigger
        bool isAnimating = false; // Is the object playing an animation

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            // Check if the key is pressed, the player is in the detection zone and no animation is playing
            if (Input.GetKeyDown(KeyCode.E) && isInTriggerZone && !isAnimating)
            {
                /*
                 * This code is executed when the button is used
                 */
                isAnimating = true;
                StartAnimation();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") // Check if the player entered the trigger (remember to specify the "Player" tag to your player)
            {
                isInTriggerZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player") // Check if player exited in trigger (remember to specify the “Player” tag for your player)
            {
                isInTriggerZone = false;
            }
        }

        void StartAnimation()
        {
            animator.SetTrigger("Press");
            audioSource.PlayOneShot(mySound);

            // Start a coroutine to wait for the animation to finish
            StartCoroutine(WaitForAnimation());
        }

        IEnumerator WaitForAnimation()
        {
            // Get the current animator state info
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Wait for the animation to finish
            yield return new WaitForSeconds(stateInfo.length);

            // Animation finished
            isAnimating = false;
        }
    }
}