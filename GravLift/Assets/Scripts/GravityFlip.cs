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
        if (Input.GetKeyDown(KeyCode.Space) && timer <= 0.0) {
            FlipGravity();
        }

        if (timer > 0.0) {
            RotatePlayer();
        }
    }

    private void FlipGravity() {
        Physics.gravity = -Physics.gravity;
        upside_down = !upside_down;
        timer = flip_duration;
    }

    private void RotatePlayer() {
        if (player != null) {
            player.transform.Rotate(0.0f, 0.0f, 180.0f / (180.0f / flip_duration * Time.deltaTime));
        }
        timer -= Time.deltaTime;
        if (timer <= 0) {
            player.transform.rotation = upside_down ? Quaternion.Euler(0,0,180) : Quaternion.identity;
        }
    }
}