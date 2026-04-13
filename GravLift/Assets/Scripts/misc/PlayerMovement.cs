using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private int max_health;
    [SerializeField] private float speed;
    [SerializeField] private float forward_bonus_speed;
    [SerializeField] private float camera_sensitivity_while_aiming;
    [SerializeField] private float camera_sensitivity_while_not_aiming;
    [SerializeField] private float field_of_view_while_aiming;
    [SerializeField] private float field_of_view_while_not_aiming;
    [SerializeField] private float first_person_camera_height;
    [SerializeField] private float spawn_ground_offset;
    [SerializeField] private float bullet_speed;
    [SerializeField] private float bullet_bloom;
    [SerializeField] private int[] bullet_damages = {10, 12, 14, 16, 18, 20, 23, 26, 29, 32};
    [SerializeField] private float interactionDistance;
    [SerializeField] private float air_movement_percentage;
    [SerializeField] private AudioSource footstepAudioSource;

    public GameObject cannon;
    public GameObject bullet;
    private Animator animation;

    public int health = 100;
    private float rotation_x = 0.0f;
    private Vector3 spawn_point;
    private bool on_ground = true;
    private float camera_sensitivity;
    private Rigidbody rb;
    private Transform t;
    [SerializeField] private Camera player_camera;
    private readonly HashSet<int> groundCollisionIds = new HashSet<int>();
    private Vector3 moveInput;

    void Start() {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        health = max_health;
        animation = GetComponent<Animator>();
        
        camera_sensitivity = camera_sensitivity_while_not_aiming;
    }

    public void Update() {
        GatherInput();

        if (Mouse.current != null && player_camera != null) {
            Turn();
        }

        if (player_camera != null && Mouse.current != null && Mouse.current.rightButton.isPressed) {
            Aim();
        } else if (player_camera != null) {
            StopAiming();
        }
    }

    private void FixedUpdate() {
        ApplyMovement();
    }

    private void GatherInput() {
        if (Keyboard.current == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;

        // normalize values to make it more realistic, forward is NOT the same speed as diagonal
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveInput.magnitude > 0.1f) {
            animation.Play("Running");
            
            if (on_ground && !footstepAudioSource.isPlaying) {
                footstepAudioSource.Play();
            } else if (!on_ground && footstepAudioSource.isPlaying) {
                footstepAudioSource.Stop();
            }
            
        } else if (Keyboard.current.bKey.isPressed) {
            animation.Play("Dancing");
            if (footstepAudioSource.isPlaying) footstepAudioSource.Stop();
            
        } else {
            animation.Play("Idle");
            if (footstepAudioSource.isPlaying) footstepAudioSource.Stop();
        }
    }

    private void ApplyMovement() {

        // Calculate the velocity
        Vector3 targetVelocity = (t.forward * moveInput.z + t.right * moveInput.x) * speed * forward_bonus_speed;
        
        if (!on_ground) {
            targetVelocity *= air_movement_percentage;
        }

        // Calculate the difference between our current velocity and next velocity
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        
        // No touch y axis
        velocityChange.y = 0f;

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
            on_ground = true;
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

    foreach (ContactPoint contact in collision.contacts) {
        /**
            // If gravity is normal (-9.81), gravityDirection is -1. We check if normal.y > 0.4f
            // Else if gravity is flipped (9.81), gravityDirection is 1. We check if normal.y < -0.4f
            // literally checks for if we're touching the floor equivalent to be able to move
         */
        if (contact.normal.y * -gravityDirection > 0.4f) {
            return true;
        }
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
        player_camera.fieldOfView = field_of_view_while_aiming;
    }

    private void StopAiming() {
        camera_sensitivity = camera_sensitivity_while_not_aiming;
        player_camera.fieldOfView = field_of_view_while_not_aiming;
    }

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        t.position = spawn_point;
        rb.linearVelocity = Vector3.zero;
        health = max_health;
    }
}