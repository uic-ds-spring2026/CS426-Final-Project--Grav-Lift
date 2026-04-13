using UnityEngine;

public class GravityFlip : MonoBehaviour {
    private bool upside_down = false;
    private float flip_duration = 0.5f;
    private float flip_timer = 0.0f;
    private GameObject player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("PLAYER"); 
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && flip_timer <= 0.0f) { 
            FlipGravity();
        }

        if (flip_timer > 0.0f) {
            RotatePlayer();
        }

        // set the z axis to 180 or 0 depending on if we're upside down or not
        if (flip_timer <= 0f && player != null) {
            Vector3 currentRotation = player.transform.eulerAngles;
            if (upside_down) finalZRotation = 180.0f;
            else finalZRotation = 0.0f;
            player.transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, finalZRotation);
        }
    }

    private void FlipGravity() {
        Physics.gravity = -Physics.gravity;
        upside_down = !upside_down;
        flip_timer = flip_duration;
        if (player != null)
        {
            if (upside_down) {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 3.0f, player.transform.position.z);
            } else {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 3.0f, player.transform.position.z);
            }
        }
    }

    private void RotatePlayer() {
        if (player != null) {

            // Degrees per frame calculation so we have a simple screen rotation
            float degreesThisFrame = (180.0f / flip_duration) * Time.deltaTime;
            
            // Apply rotation for around the player for the screen essentially
            player.transform.Rotate(0.0f, 0.0f, degreesThisFrame, Space.Self);
        }

        flip_timer -= Time.deltaTime;
        float finalZRotation = 0f;
    }
}