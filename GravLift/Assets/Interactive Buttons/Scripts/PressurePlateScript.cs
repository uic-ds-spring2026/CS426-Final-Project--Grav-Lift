using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjects.PressurePlates
{
    public class PressurePlateScript : MonoBehaviour
    {
        public Material ColorOn;
        public Material ColorOff;

        [SerializeField] private DoorController[] linkedDoors; 

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

            foreach (DoorController door in linkedDoors)
            {
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            // Reset the plate visually
            pressurePlateRenderer.material = ColorOff;

            foreach (DoorController door in linkedDoors)
            {
                if (door != null)
                {
                    door.CloseDoor();
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            // Visual and audio feedback for the plate
            pressurePlateRenderer.material = ColorOn;
        }

        private void OnTriggerExit(Collider other)
        {
            // Reset the plate visually
            pressurePlateRenderer.material = ColorOff;

        }
    }
}