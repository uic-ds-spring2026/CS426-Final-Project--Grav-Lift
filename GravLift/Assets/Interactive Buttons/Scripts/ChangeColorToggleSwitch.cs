using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjects.ToggleSwitch
{
    public class ChangeColorToggleSwitch : MonoBehaviour
    {
        public Material BrightRed;
        public Material BrightGreen;
        public Material NonGlossyRed;
        public Material NonGlossyGreen;

        public GameObject textOff;
        public GameObject textOn;

        private Renderer textOffRenderer;
        private Renderer textOnRenderer;

        private Animator animator;

        // Start is called before the first frame update
        void Start()
        {
            textOffRenderer = textOff.GetComponent<Renderer>();
            textOnRenderer = textOn.GetComponent<Renderer>();

            animator = GetComponentInParent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            bool isUp = animator.GetBool("IsUp");

            // Apply the appropriate materials based on the IsUp value
            if (isUp)
            {
                textOffRenderer.material = NonGlossyRed;
                textOnRenderer.material = BrightGreen;
            }
            else
            {
                textOffRenderer.material = BrightRed;
                textOnRenderer.material = NonGlossyGreen;
            }
        }
    }
}