using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private int health;
    [SerializeField] private int max_health;
    [SerializeField] private Slider health_bar;
    [SerializeField] private Image health_color;
    [SerializeField] private TextMeshProUGUI health_text;
    [SerializeField] private float speed;
    [SerializeField] private float forward_bonus_speed;
    [SerializeField] private float camera_sensitivity_while_aiming;
    [SerializeField] private float camera_sensitivity_while_not_aiming;
    [SerializeField] private float field_of_view_while_aiming;
    [SerializeField] private float field_of_view_while_not_aiming;
    [SerializeField] private float air_movement_percentage;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private Camera player_camera;
    [SerializeField] private float ground_tolerence_time = 0.15f;

    public GameObject cannon;
    public GameObject bullet;
    private Animator animation;
    private float rotation_x = 0.0f;
    private Vector3 spawn_point;
    private bool on_ground = true;
    private float camera_sensitivity;
    private Rigidbody rb;
    private Transform t;
    private readonly HashSet<int> groundCollisionIds = new HashSet<int>();
    private Vector3 moveInput;
    private float ground_timer = 0f;
    

    public void Start() {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        health = max_health;
        animation = GetComponent<Animator>();
        camera_sensitivity = camera_sensitivity_while_not_aiming;
        UpdateHealthUI();
    }

    public void Update() {
        GatherInput();
        UpdateHealthUI(); // here for testing purposes. doesn't need to run every frame

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

        if (on_ground) {
            Vector3 push = Vector3.ProjectOnPlane(t.forward * moveInput.z + t.right * moveInput.x, transform.up);
            rb.AddForce(push * 0.5f, ForceMode.VelocityChange);
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
        health -= damage;
        UpdateHealthUI();
        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        // perhaps add a death screeen?
        t.position = spawn_point;
        rb.linearVelocity = Vector3.zero;
        health = max_health;
        UpdateHealthUI();
    }

    /* UPDATES THE HEALTH BAR AND TEXT
     */
    private void UpdateHealthUI() {
        // controls the health bar
        if (health_bar != null) {
            health_bar.value = (float) health / max_health;
        }

        // controls the health text
        if (health_text != null) {
            health_text.text = "HP: " + health.ToString() + " / " + max_health.ToString();
        }

        // controls the health color
        if (health_color != null && health_text != null) {
            if (health <= 20) {
                health_color.color = Color.red;
                health_text.color = Color.red;
            } else if (health <= 50) {
                health_color.color = Color.yellow;
                health_text.color = Color.yellow;
            } else {
                health_color.color = Color.green;
                health_text.color = Color.green;
            }
        }
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
                    transform.position += Vector3.up * stepHeight;
                }
            }
        }
    }
}