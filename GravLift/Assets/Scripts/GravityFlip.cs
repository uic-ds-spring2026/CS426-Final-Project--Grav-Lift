/* CS 426 Final Project (Grav Lift)
 * Group members: Rafael Maatouk, Fernando Lopez, Andrew Yoe
 * Description: Script that manages flipping gravity
 */
using UnityEngine;

public class GRAVITYFLIP : MonoBehaviour {
    private bool upside_down = false;
    private float flip_duration = 0.5f;
    private float flip_timer = 0.0f;
    private GameObject player;
    private Quaternion start_rotation;
    private Quaternion target_rotation;
    private bool is_grounded;
    private Transform camera_pivot;

    private void Start() {
        player = this.gameObject;
        camera_pivot = GameObject.Find("camera_pivot").transform;
        is_grounded = true;
    }

    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space) && flip_timer <= 0.0f && is_grounded) { 
            FlipGravity();
        }

        if (flip_timer > 0.0f) {
            RotatePlayer();

            // keeps camera from dipping into floor while flipping gravity
            if (camera_pivot != null) {
                Vector3 pos = camera_pivot.localPosition;
                pos.y = Mathf.Max(pos.y, 0f);
                camera_pivot.localPosition = pos;
            }
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
            Rigidbody rb = player.GetComponent<Rigidbody>();

            // move a bit away from ground to stop head clipping
            Vector3 gravity_direction = Physics.gravity.normalized;
            rb.AddForce(-gravity_direction * 2.5f, ForceMode.VelocityChange);
            rb.position += -gravity_direction * 0.05f;

            start_rotation = player.transform.rotation;
            Vector3 up_direction = upside_down ? Vector3.down : Vector3.up;
            target_rotation = Quaternion.LookRotation(player.transform.forward, up_direction);
        }
    }

    private void RotatePlayer() {
        if (player != null) {
            float t = Mathf.SmoothStep(0f, 1f, 1f - (flip_timer / flip_duration));
            player.transform.rotation = Quaternion.Slerp(start_rotation, target_rotation, t);
            if (camera_pivot != null) {
                camera_pivot.localRotation = Quaternion.identity;
            }
        }

        flip_timer -= Time.deltaTime;

        if (flip_timer <= 0f && player != null) {
            player.transform.rotation = target_rotation;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.angularVelocity = Vector3.zero;

            // stop any drift entirely to stop it from slanting
            Vector3 forward = Vector3.ProjectOnPlane(player.transform.forward, Physics.gravity);
            player.transform.rotation = Quaternion.LookRotation(forward, -Physics.gravity.normalized);
        }
    }

    private void OnCollisionStay(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
            Vector3 gravity_direction = Physics.gravity.normalized;
            // if if we're touching the ground the same as gravity, its floor, or else its wall
            if (Vector3.Dot(contact.normal, -gravity_direction) > 0.5f) {
                is_grounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        is_grounded = false;
    }
}