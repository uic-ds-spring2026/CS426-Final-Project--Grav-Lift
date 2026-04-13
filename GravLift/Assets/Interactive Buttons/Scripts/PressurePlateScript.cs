using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjects.PressurePlates
{
    public class PressurePlateScript : MonoBehaviour
    {
        public Material ColorOn;
        public Material ColorOff;

        private Renderer pressurePlateRenderer;

        [SerializeField] AudioClip mySound;
        AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            pressurePlateRenderer = GetComponent<Renderer>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            /*
             * This code is executed when stepping on the pressure plate
             */
            pressurePlateRenderer.material = ColorOn;
            audioSource.PlayOneShot(mySound);
        }

        private void OnCollisionExit(Collision collision)
        {
            pressurePlateRenderer.material = ColorOff;
        }
    }
}