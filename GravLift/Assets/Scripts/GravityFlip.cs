using UnityEngine;

public class GravityFlip : MonoBehaviour {
    private bool upside_down = false;
    private float flip_duration = 0.5f;
    private float flip_timer = 0.0f;
    private GameObject player;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private bool isGrounded;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("PLAYER"); 
        isGrounded = true;
    }

    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space) && flip_timer <= 0.0f && isGrounded) { 
            FlipGravity();
        }

        if (flip_timer > 0.0f) {
            RotatePlayer();
        }
        
        // Irrelavent now
        // // set the z axis to 180 or 0 depending on if we're upside down or not
        // if (flip_timer <= 0f && player != null) {
        //     Vector3 currentRotation = player.transform.eulerAngles;
        //     float finalZRotation = 0.0f;
        //     if (upside_down) finalZRotation = 180.0f;
        //     else finalZRotation = 0.0f;
        //     player.transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, finalZRotation);
        // }
    }

    private void FlipGravity() {
        Physics.gravity = -Physics.gravity;
        upside_down = !upside_down;
        flip_timer = flip_duration;
        if (player != null)
        {
            startRotation = player.transform.rotation;
            targetRotation = startRotation * Quaternion.Euler(0f, 0f, 180f);
        }
    }

    private void RotatePlayer() {
        if (player != null) {
            float t = 1f - (flip_timer / flip_duration);
            player.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
        }
        flip_timer -= Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
            Vector3 gravityDir = -player.transform.up;

            // if contact is in the direction of gravity, it's ground
            if (Vector3.Dot(contact.normal, -gravityDir) > 0.5f) {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        isGrounded = false;
    }
}