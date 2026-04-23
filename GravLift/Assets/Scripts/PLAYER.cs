/* CS 426 Final Project
 * Group members: Rafael Maatouk, Fernando Lopez, Andrew Yoe
 * Description: Script that manages player movement
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PLAYER : MonoBehaviour {
    [SerializeField] private HEALTH health;
    [SerializeField] private float speed;
    [SerializeField] private float forward_bonus_speed;
    [SerializeField] private float camera_sensitivity_while_aiming;
    [SerializeField] private float camera_sensitivity_while_not_aiming;
    [SerializeField] private float field_of_view_while_aiming;
    [SerializeField] private float field_of_view_while_not_aiming;
    [SerializeField] private float air_movement_percentage;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private Camera player_camera;
    [SerializeField] private float ground_tolerence_time;
    private Animator animation;
    private float rotation_x;
    private Vector3 spawn_point;
    private bool on_ground;
    private float camera_sensitivity;
    private Rigidbody rb;
    private Transform t;
    private HashSet<int> groundCollisionIds;
    private Vector3 moveInput;
    private float ground_timer;
    private PAUSEMENU pause_menu;

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
        SmallStepAssist(t.forward * moveInput.z + t.right * moveInput.x);
        

        // Calculate the velocity
        Vector3 targetVelocity = (t.forward * moveInput.z + t.right * moveInput.x) * speed * forward_bonus_speed;
        
        if (!on_ground) {
            targetVelocity *= air_movement_percentage;
        }

        // keep current velocity but isolate movement plane (prevents edge fighting)
        Vector3 currentVelocity = rb.linearVelocity;

        // remove only the velocity along movement plane, keep gravity/vertical motion intact
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(currentVelocity, transform.up);

        // Calculate the difference between our current velocity and next velocity
        Vector3 velocityChange = targetVelocity - horizontalVelocity;

        // No touch y axis (still respects your gravity flip system)
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
        player_camera.fieldOfView = field_of_view_while_aiming;
    }

    private void StopAiming() {
        camera_sensitivity = camera_sensitivity_while_not_aiming;
        player_camera.fieldOfView = field_of_view_while_not_aiming;
    }

    public void TakeDamage(int damage) {
        health.TakeDamage(damage);
        if (health.IsDead()) {
            Die();
        }
    }

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

        Vector3 origin = transform.position + Vector3.up * 0.05f;
        float checkDistance = 0.4f;
        float stepHeight = 0.3f; 

        if (Physics.Raycast(origin, moveDir, checkDistance)) {
            Vector3 upperOrigin = origin + Vector3.up * stepHeight;
            if (!Physics.Raycast(upperOrigin, moveDir, checkDistance)) {
                if (Physics.Raycast(upperOrigin, Vector3.down, stepHeight + 0.1f)) {
                    rb.MovePosition(rb.position + Vector3.up * stepHeight);
                }
            }
        }
    }
}