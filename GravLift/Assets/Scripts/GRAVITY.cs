using UnityEngine;

public class GRAVITY : MonoBehaviour {
    private bool is_upside_down;
    private float flip_duration = 0.5f;
    private float flip_timer = 0.0f;
    private GameObject player;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private bool isGrounded;
    private Transform cameraPivot;

    public AudioSource audioSource;
    public AudioClip flipUpSound;
    public AudioClip flipDownSound;
    public bool canFlipGravity = true;

    private void Start() {
        is_upside_down = false;
        player = this.gameObject;
        cameraPivot = GameObject.Find("CameraPivot").transform;
        isGrounded = true;
        flip_duration = 0.5f;
        flip_timer = 0.0f;
    }

    private void Update() {
        
        if (canFlipGravity && Input.GetKeyDown(KeyCode.Space) && flip_timer <= 0.0f && isGrounded) { 
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

    }

    private void FlipGravity() {
        if (!isGrounded) return; //We already have ways to prevent this when in the air, but just in case

        //Since one of the levels has a Sliding block, we don't want players to flip while standing on it
        // and crush themselves, so we added this segment to FlipGravity() to be sure they can't do that
        float checkDistance = 10.0f; //theres no other sliding block, this can afford to be larger than average to be safe
        RaycastHit hit;

        Vector3[] raycastDirections; //Since we flip gravity, vector3 up and down swap so we need to account for that
        if (Physics.gravity.y > 0) {
            // Normal gravity, check both in case
            raycastDirections = new Vector3[] { Vector3.up, Vector3.down };
        } else {
            // Flipped gravity, check both in case
            raycastDirections = new Vector3[] { Vector3.down, Vector3.up };
        }

        Vector3 origin = transform.position + (is_upside_down ? Vector3.down : Vector3.up) * 0.1f;  // Offset from origin according to gravity direction

        foreach (Vector3 direction in raycastDirections) {
            if (Physics.Raycast(origin, direction, out hit, checkDistance)) {
                if (hit.collider.CompareTag("SLIDINGBLOCK")) {
                    // if the Sliding Block is on our same axis (above or below us depending on gravity), skip the flip
                    Debug.Log("Blocked gravity flip: standing on SLIDINGBLOCK.");
                    return;
                }
            }
        }

        //Now we move onto the actual gravity flip
        Physics.gravity = -Physics.gravity;
        is_upside_down = !is_upside_down;
        flip_timer = flip_duration;

        // if we have the audio source game object
        if (audioSource != null) {

            // check if they're upside down and that we have the sound effect, then play
            if (is_upside_down && flipUpSound != null) {
                audioSource.PlayOneShot(flipUpSound);

            // otherwise check if it's the otherway around and the effect exists and use it if so
            } else if (!is_upside_down && flipDownSound != null) {
                audioSource.PlayOneShot(flipDownSound);
            }
        }

        player.GetComponent<PlayerMovement>().gravityFlipped = true; //used in player movement to restrict air movement

        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();

            // move a bit away from ground to stop head clipping
            Vector3 gravityDir = Physics.gravity.normalized;
            rb.AddForce(-gravityDir * 2.5f, ForceMode.VelocityChange);
            rb.position += -gravityDir * 0.05f;

            startRotation = player.transform.rotation;
            Vector3 upDir = is_upside_down ? Vector3.down : Vector3.up;
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

    /// <summary>
    /// getter function for is_upside_down
    /// </summary>
    /// <returns> is_upside_down </returns>
    public bool IsUpsideDown() {
        return is_upside_down;
    }
}