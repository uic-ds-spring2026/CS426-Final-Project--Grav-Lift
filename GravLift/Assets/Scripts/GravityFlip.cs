using UnityEngine;

public class GravityFlip : MonoBehaviour {
    private bool upside_down = false;
    private float flip_duration = 0.5f;
    private float flip_timer = 0.0f;
    private GameObject player;

    private void Start() {
        // NOTE: Make sure your tag is exactly "PLAYER" in Unity, 
        // tags are case-sensitive. Usually, the default is "Player".
        player = GameObject.FindGameObjectWithTag("PLAYER"); 
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && flip_timer <= 0.0f) { 
            FlipGravity();
        }

        if (flip_timer > 0.0f) {
            RotatePlayer();
        }
    }

    private void FlipGravity() {
        Physics.gravity = -Physics.gravity;
        upside_down = !upside_down;
        flip_timer = flip_duration;
    }

    private void RotatePlayer() {
        if (player != null) {
            // Calculate how many degrees we should rotate this exact frame
            float degreesThisFrame = (180.0f / flip_duration) * Time.deltaTime;
            
            // Apply the rotation relative to the player's local space
            player.transform.Rotate(0.0f, 0.0f, degreesThisFrame, Space.Self);
        }

        flip_timer -= Time.deltaTime;

        // When the flip is done, snap ONLY the Z axis to perfectly 180 or 0
        if (flip_timer <= 0f && player != null) {
            Vector3 currentRotation = player.transform.eulerAngles;
            float finalZRotation = upside_down ? 180f : 0f;
            
            // Keep the current X and Y so we don't mess up the camera direction
            player.transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, finalZRotation);
        }
    }
}