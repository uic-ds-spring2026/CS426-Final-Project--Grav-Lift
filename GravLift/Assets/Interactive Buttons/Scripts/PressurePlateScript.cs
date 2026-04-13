using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjects.PressurePlates
{
    public class PressurePlateScript : MonoBehaviour
    {
        public Material ColorOn;
        public Material ColorOff;

        // Reference to the door we want to open
        [SerializeField] private DoorController linkedDoor; 

        private Renderer pressurePlateRenderer;

        [SerializeField] AudioClip mySound;
        AudioSource audioSource;

        void Start()
        {
            pressurePlateRenderer = GetComponent<Renderer>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Visual and audio feedback for the plate
            pressurePlateRenderer.material = ColorOn;
            audioSource.PlayOneShot(mySound);

            // Tell the linked door to open
            if (linkedDoor != null)
            {
                linkedDoor.OpenDoor();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            // Reset the plate visually
            pressurePlateRenderer.material = ColorOff;

            // Optional: Tell the linked door to close when stepping off
            if (linkedDoor != null)
            {
                linkedDoor.CloseDoor();
            }
        }
    }
}