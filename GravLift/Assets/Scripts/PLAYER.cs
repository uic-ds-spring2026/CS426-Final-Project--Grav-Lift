using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour {
    /// <summary>
    /// stores health, max health, and UI information for the player
    /// </summary>
    [SerializeField] private HEALTH health;
    /// <summary>
    /// how fast the player is
    /// </summary>
    [SerializeField] private float speed;
    /// <summary>
    /// how much faster the player can move forwards
    /// </summary>
    [SerializeField] private float forward_bonus_speed;
    /// <summary>
    /// the player's camera sensitivity while aiming down sights
    /// </summary>
    [SerializeField] private float camera_sensitivity_while_aiming;
    /// <summary>
    /// the player's camera sensitivity while not aiming down sights
    /// </summary>
    [SerializeField] private float camera_sensitivity_while_not_aiming;
    /// <summary>
    /// the player's field of view while aiming down sights
    /// </summary>
    [SerializeField] private float fov_while_aiming;
    /// <summary>
    /// the player's field of view while not aiming down sights
    /// </summary>
    [SerializeField] private float fov_while_not_aiming;
    /// <summary>
    /// slows down the speed of the player while in the air
    /// </summary>
    [SerializeField] private float air_movement_percentage;
    /// <summary>
    /// plays footstep audio while moving
    /// </summary>
    [SerializeField] private AudioSource footstep_audio_source;
    /// <summary>
    /// the first-person player camera
    /// </summary>
    [SerializeField] private Camera player_camera;
    /// <summary>
    /// considers the player on the ground when colliding with it during a short time frame
    /// </summary>
    [SerializeField] private float ground_tolerence_time;
    /// <summary>
    /// animates the player when moving, dancing, or idle
    /// </summary>
    private Animator animation;
    /// <summary>
    /// controls the player camera moving up and down
    /// </summary>
    private float rotation_x;
    /// <summary>
    /// controls the player spawn point when spawning / respawning
    /// </summary>
    private Vector3 spawn_point;
    /// <summary>
    /// tracks whether the player is on the ground
    /// </summary>
    private bool on_ground;
    /// <summary>
    /// tracks the current camera sensitivity
    /// </summary>
    private float camera_sensitivity;
    /// <summary>
    /// holds the player's rigidbody
    /// </summary>
    private Rigidbody rb;
    /// <summary>
    /// holds the player's transform
    /// </summary>
    private Transform t;
    /// <summary>
    /// tracks what floor objects the player is colliding with
    /// </summary>
    private HashSet<int> groundCollisionIds;
    /// <summary>
    /// tracks which direction the player is moving
    /// </summary>
    private Vector3 move_input;
    /// <summary>
    /// tracks how long the player has been on the ground for
    /// </summary>
    private float ground_timer;
    /// <summary>
    /// holds the UI of the pause menu
    /// </summary>
    private PAUSEMENU pause_menu;
    public bool gravityFlipped = false; // needs access from GravityFlip so this is a public variable
    

    public void Start() {
        animation = GetComponent<Animator>();
        rotation_x = 0.0f;
        on_ground = true;
        camera_sensitivity = camera_sensitivity_while_not_aiming;
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        groundCollisionIds = new HashSet<int>();
        ground_timer = 0.0f;
        pause_menu = FindAnyObjectByType<PAUSEMENU>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update() {
        if (pause_menu != null && pause_menu.IsPaused()) {
            return;
        }
        if (Keyboard.current != null) {
            GatherInput();
        }
        if (Mouse.current != null && player_camera != null) {
            Turn();
        }
        if (player_camera != null) {
            if (Mouse.current != null && Mouse.current.rightButton.isPressed) {
                Aim();
            } else {
                StopAiming();
            }
        }
    }

    private void FixedUpdate() {
        ground_timer -= Time.fixedDeltaTime;
        if (ground_timer <= 0f) {
            on_ground = false;
        }
        ApplyMovement();
    }

    private void GatherInput() {
        if (Keyboard.current == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1.2f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1.2f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1.2f;
        if (Keyboard.current.dKey.isPressed) moveX += 1.2f;

        // normalize values to make it more realistic, forward is NOT the same speed as diagonal
        move_input = new Vector3(moveX, 0f, moveZ).normalized;

        if (move_input.magnitude > 0.1f) {
            animation.Play("Running");
            
            if (on_ground && !footstep_audio_source.isPlaying) {
                footstep_audio_source.Play();
            } else if (!on_ground && footstep_audio_source.isPlaying) {
                footstep_audio_source.Stop();
            }
            
        } else if (Keyboard.current.bKey.isPressed) {
            animation.Play("Dancing");
            if (footstep_audio_source.isPlaying) footstep_audio_source.Stop();
            
        } else {
            animation.Play("Idle");
            if (footstep_audio_source.isPlaying) footstep_audio_source.Stop();
        }
    }

    private void ApplyMovement() {

        if (!on_ground && gravityFlipped)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return; //This disables movement in the air only if the gravity flip was initiated
        }

        SmallStepAssist(t.forward * move_input.z + t.right * move_input.x);
        

        // Calculate the velocity
        Vector3 targetVelocity = (t.forward * move_input.z + t.right * move_input.x) * speed * forward_bonus_speed;
        
        // if (!on_ground) {
        //     targetVelocity *= air_movement_percentage;
        // }

        // keep current velocity but isolate movement plane (prevents edge fighting)
        Vector3 currentVelocity = rb.linearVelocity;

        // remove only the velocity along movement plane, keep gravity/vertical motion intact
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(currentVelocity, transform.up);

        // Calculate the difference between our current velocity and next velocity
        Vector3 velocityChange = targetVelocity - horizontalVelocity;

        // No touch y axis 
        velocityChange = Vector3.ProjectOnPlane(velocityChange, transform.up);

        // Use velocityChange to ignore rigidBody, it sucks
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision) {
        if (IsGroundCollision(collision)) {
            groundCollisionIds.Add(collision.collider.GetInstanceID());
            on_ground = true;
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (IsGroundCollision(collision)) {
            groundCollisionIds.Add(collision.collider.GetInstanceID());
            ground_timer = ground_tolerence_time;
            on_ground = true;
            if (gravityFlipped) {
                gravityFlipped = false;  //Now that we are on the ground, if it was from a Gravity Flip, we mark it.
            }
        }
        
    }

    private void OnCollisionExit(Collision collision) {
        groundCollisionIds.Remove(collision.collider.GetInstanceID());
        on_ground = groundCollisionIds.Count > 0;
    }

    private bool IsGroundCollision(Collision collision) {
        if (collision == null || collision.collider == null) return false;

        // determine the opposite side of gravity, force up, not down
        float gravityDirection = Mathf.Sign(Physics.gravity.y);

        int validContacts = 0;

        foreach (ContactPoint contact in collision.contacts) {
            /**
                // If gravity is normal (-9.81), gravityDirection is -1. We check if normal.y > 0.4f
                // Else if gravity is flipped (9.81), gravityDirection is 1. We check if normal.y < -0.4f
                // literally checks for if we're touching the floor equivalent to be able to move
            */
            if (contact.normal.y * -gravityDirection > 0.6f) {
                validContacts++;
            }
        }

        if (validContacts > 0){
            return true;
        }

        return false;
    }

    private void Turn() {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * camera_sensitivity;
        t.Rotate(Vector3.up * mouseX);
        
        float mouseY = mouseDelta.y * camera_sensitivity;
        rotation_x -= mouseY;
        rotation_x = Mathf.Clamp(rotation_x, -90.0f, 90.0f);
        player_camera.transform.localRotation = Quaternion.Euler(rotation_x, 0.0f, 0.0f);
    }

    private void Aim() {
        camera_sensitivity = camera_sensitivity_while_aiming;
        player_camera.fieldOfView = fov_while_aiming;
    }

    private void StopAiming() {
        camera_sensitivity = camera_sensitivity_while_not_aiming;
        player_camera.fieldOfView = fov_while_not_aiming;
    }

    /// <summary>
    /// reduces the player's health by int damage
    /// </summary>
    /// <param name="damage"> how much the health should reduce </param>
    public void TakeDamage(int damage) {
        health.TakeDamage(damage);
        if (health.IsDead()) {
            Die();
        }
    }

    /// <summary>
    /// kills and respawns the player
    /// </summary>
    public void Die() {
        // perhaps add a death screeen?
        t.position = spawn_point;
        rb.linearVelocity = Vector3.zero;
        health.ResetHealth();
    }

    private void SmallStepAssist(Vector3 moveDir) {
        if (!on_ground || moveDir.magnitude < 0.1f)
        {
            return;
        } 
        //Setup for raycasting, makes sure we can detect flat or ramped surfaces
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        float checkDistance = 0.4f;
        float stepHeight = 0.3f; 
        RaycastHit hit; 
        Vector3 raycastDirection = Physics.gravity.y > 0 ? Vector3.down : Vector3.up; //since we flip gravity, we need a direction for each side

        if (Physics.Raycast(origin, moveDir, out hit, checkDistance)) { //Performs the raycast and sets hit at the same time
            // if its a relatively small flat height difference, step up it
            if (hit.normal.y > 0.5f) {
                Vector3 upperOrigin = origin + Vector3.up * stepHeight;
                if (!Physics.Raycast(upperOrigin, moveDir, checkDistance)) {
                    if (Physics.Raycast(upperOrigin, raycastDirection, stepHeight + 0.1f)) {
                        rb.MovePosition(rb.position + Vector3.up * stepHeight);
                    }
                }
            }
            // if its a ramp of some kind, add some speed when going down it to emulate going downhill
            else if (hit.normal.y < 0.5f) {
                float slideFactor = 0.5f;  // Adds extra speed to slopes to feel less chunky
                rb.AddForce(Vector3.ProjectOnPlane(Physics.gravity, hit.normal) * slideFactor, ForceMode.Acceleration);
            }
            // we didn't make one for uphill as that would be a bit annoying to work with
        }
    }
}