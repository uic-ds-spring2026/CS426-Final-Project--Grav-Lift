using UnityEngine;

public class GravityFlip : MonoBehaviour {
    private bool upside_down = false;
    private float flip_duration = 0.5f;
    private float flip_timer = 0.0f;
    private GameObject player;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private bool isGrounded;
    private Transform cameraPivot;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("PLAYER"); 
        cameraPivot = GameObject.Find("CameraPivot").transform;
        isGrounded = true;
    }

    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space) && flip_timer <= 0.0f && isGrounded) { 
            FlipGravity();
        }

        if (flip_timer > 0.0f) {
            RotatePlayer();

            // keeps camera from dipping into floor while flipping gravity
            if (cameraPivot != null)
            {
                Vector3 pos = cameraPivot.localPosition;
                pos.y = Mathf.Max(pos.y, 0f);
                cameraPivot.localPosition = pos;
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
            Vector3 gravityDir = Physics.gravity.normalized;
            rb.AddForce(-gravityDir * 2.5f, ForceMode.VelocityChange);
            rb.position += -gravityDir * 0.05f;

            startRotation = player.transform.rotation;
            Vector3 upDir = upside_down ? Vector3.down : Vector3.up;
            targetRotation = Quaternion.LookRotation(player.transform.forward, upDir);
        }
    }

    private void RotatePlayer() {
        if (player != null) {

            float t = Mathf.SmoothStep(0f, 1f, 1f - (flip_timer / flip_duration));
            player.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.identity;
            }
        }

        flip_timer -= Time.deltaTime;

        if (flip_timer <= 0f && player != null) {

            player.transform.rotation = targetRotation;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.angularVelocity = Vector3.zero;

            // stop any drift entirely to stop it from slanting
            Vector3 forward = Vector3.ProjectOnPlane(player.transform.forward, Physics.gravity);
            player.transform.rotation = Quaternion.LookRotation(forward, -Physics.gravity.normalized);
        }
    }

    private void OnCollisionStay(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
            Vector3 gravityDir = Physics.gravity.normalized;
            // if if we're touching the ground the same as gravity, its floor, or else its wall
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