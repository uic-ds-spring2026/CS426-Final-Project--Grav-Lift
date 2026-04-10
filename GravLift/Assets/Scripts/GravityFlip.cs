using UnityEngine;

public class GravityFlip : MonoBehaviour {
    // Update is called once per frame
    private bool upside_down = false;
    private float flip_duration = 0.5f;
    private float flip_timer = 0.0f;
    private GameObject player;
    private void Start() {
        player = GameObject.FindGameObjectWithTag("PLAYER");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && flip_timer <= 0.0) { // gravity only flips if not already flipping
            FlipGravity();
        }

        if (flip_timer > 0.0) {
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
            player.transform.Rotate(0.0f, 0.0f, 180.0f / (180.0f / flip_duration * Time.deltaTime)); // rotate the player by a small amount each frame
        }
        flip_timer -= Time.deltaTime;
        if (flip_timer <= 0) {
            player.transform.rotation = upside_down ? Quaternion.Euler(0,0,180) : Quaternion.identity; // this ensures the player is perfectly perpendicular to the ground when finished rotating
        }
    }
}