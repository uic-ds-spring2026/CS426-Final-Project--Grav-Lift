// shoot
// using __ imports namespace
// Namespaces are collection of classes, data types
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// MonoBehavior is the base class from which every Unity Script Derives
public class PlayerMovement : MonoBehaviour {
    // PLAYER ATTRIBUTES (Can modify in Unity)
    [SerializeField] private int max_health;
    [SerializeField] private float speed;
    [SerializeField] private float forward_bonus_speed;
    [SerializeField] private float jump_strength;
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
    //public Slider health_slider;
    //public float rotationSpeed = 45f;
    int framesPressed = 0;

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

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        health = max_health;
        animation = GetComponent<Animator>();
    }


    public void Update() {
        //UpdateHealthUI();

        // WASD
        if (Keyboard.current != null && Keyboard.current.wKey.isPressed) {
            MoveForward();
            animation.Play("Walking");
        } if (Keyboard.current != null && Keyboard.current.aKey.isPressed) {
            MoveLeft();
            animation.Play("Walking");
        } if (Keyboard.current != null && Keyboard.current.sKey.isPressed) {
            MoveBackward();
            animation.Play("Walking");
        } if (Keyboard.current != null && Keyboard.current.dKey.isPressed) {
            MoveRight();
            animation.Play("Walking");
        } if (Keyboard.current != null && !Keyboard.current.wKey.isPressed
                                       && !Keyboard.current.aKey.isPressed
                                       && !Keyboard.current.sKey.isPressed
                                       && !Keyboard.current.dKey.isPressed) {
            animation.Play("Idle");
        }

        // JUMP
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && on_ground) {
            Jump();
        }
        
        if (Keyboard.current != null && Keyboard.current.bKey.isPressed) {
            animation.Play("Dancing");
        }

        // MOUSE MOVEMENT
        if (Mouse.current != null && player_camera != null) {
            Turn();
        }

        // SHOOT
        if (Keyboard.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            Shoot();
        }

        // RIGHT CLICK
        if (player_camera != null && Mouse.current != null && Mouse.current.rightButton.isPressed) {
            Aim();
        } else if (player_camera != null) {
            StopAiming();
        }
    }

    // UPDATES THE PLAYER WHEN ON THE GROUND
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

    // UPDATES THE PLAYER WHEN NOT ON THE GROUND
    private void OnCollisionExit(Collision collision) {
        groundCollisionIds.Remove(collision.collider.GetInstanceID());
        on_ground = groundCollisionIds.Count > 0;
    }

    private bool IsGroundCollision(Collision collision) {
        if (collision == null || collision.collider == null) {
            return false;
        }

        // Ground check without string tags to avoid runtime errors when project tags differ.
        foreach (ContactPoint contact in collision.contacts) {
            if (contact.normal.y > 0.4f) {
                return true;
            }
        }

        return false;
    }

    // MOVES THE PLAYER FORWARD
    private void MoveForward() {
        if (on_ground) {
            rb.AddForce(t.forward * speed * forward_bonus_speed);
        } else {
            rb.AddForce(t.forward * speed * forward_bonus_speed * air_movement_percentage);
        }
    }

    // MOVES THE PLAYER LEFT
    private void MoveLeft() {
        if (on_ground) {
            rb.AddForce(-t.right * speed * forward_bonus_speed);
        } else {
            rb.AddForce(-t.right * speed * forward_bonus_speed * air_movement_percentage);
        }
    }

    // MOVES THE PLAYER BACKWARD
    private void MoveBackward() {
        if (on_ground) {
            rb.AddForce(-t.forward * speed * forward_bonus_speed);
        } else {
            rb.AddForce(-t.forward * speed * forward_bonus_speed * air_movement_percentage);
        }
    }

    // MOVES THE PLAYER RIGHT
    private void MoveRight() {
        if (on_ground) {
            rb.AddForce(t.right * speed * forward_bonus_speed);
        } else {
            rb.AddForce(t.right * speed * forward_bonus_speed * air_movement_percentage);
        }
    }

    // MOVES THE PLAYER UP
    private void Jump() {
        rb.AddForce(t.up * jump_strength);
    }

    // TURNS THE PLAYER
    private void Turn() {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * camera_sensitivity;
        t.Rotate(Vector3.up * mouseX);
        float mouseY = mouseDelta.y * camera_sensitivity;
        rotation_x -= mouseY;
        rotation_x = Mathf.Clamp(rotation_x, -90.0f, 90.0f);
        player_camera.transform.localRotation = Quaternion.Euler(rotation_x, 0.0f, 0.0f);
    }

    // CAUSES THE PLAYER TO AIM DOWN SIGHTS
    private void Aim() {
        camera_sensitivity = camera_sensitivity_while_aiming;
        player_camera.fieldOfView = field_of_view_while_aiming;
    }

    // STOPS THE PLAYER FROM AIMING DOWN SIGHTS
    private void StopAiming() {
        camera_sensitivity = camera_sensitivity_while_not_aiming;
        player_camera.fieldOfView = field_of_view_while_not_aiming;
    }

    private void Shoot()
    {
        GameObject newBullet = GameObject.Instantiate(bullet, cannon.transform.position, cannon.transform.rotation) as GameObject;
        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        
        float randomness = 0.01f;
        Vector3 spread = new Vector3(
        Random.Range(-randomness, randomness),
        Random.Range(-randomness, randomness),
        Random.Range(-randomness, randomness)
        );
        Vector3 randomizedForward = (newBullet.transform.forward + spread).normalized;
        Vector3 shotDirection = (randomizedForward * 500) + (Vector3.up * 1050);
        rb.AddForce(shotDirection);
    }

    /*private void UpdateHealthUI() {
        if (health_slider != null) {
            health_slider.value = health.Value;
        }
    }*/

    // REDUCES THE HEALTH OF THE PLAYER
    public void TakeDamage(int damage) {
        health -= damage;
        //Debug.Log("Player " + OwnerClientId + "'s health is now: " + health);
        if (health <= 0) {
            Die();
        }
    }

    // KILLS AND RESPAWNS THE PLAYER
    public void Die() {
        //Debug.Log("Player " + OwnerClientId + " has died!");
        t.position = spawn_point;
        rb.linearVelocity = Vector3.zero;
        health = max_health;
    }

    /*// Update is called once per frame
    void Update() {
        // Time.deltaTime represents the time that passed since the last frame
        //the multiplication below ensures that GameObject moves constant speed every frame
        if (Keyboard.current != null && Keyboard.current.wKey.isPressed) {
            rb.linearVelocity += this.transform.forward * speed * Time.deltaTime;
        } else if (Keyboard.current != null && Keyboard.current.sKey.isPressed) {
            rb.linearVelocity -= this.transform.forward * speed * Time.deltaTime;
        }

        if (Keyboard.current != null && Keyboard.current.dKey.isPressed) {
            t.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime * framesPressed, 0);
            framesPressed++;
        } else if (Keyboard.current != null && Keyboard.current.aKey.isPressed) {
            t.rotation *= Quaternion.Euler(0, -rotationSpeed * Time.deltaTime * framesPressed, 0);
            framesPressed++;
        } else {
            framesPressed = 0;
        }

        //////////
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) {
            rb.AddForce(t.up * force);
        }

        // https://docs.unity3d.com/ScriptReference/Input.html
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            GameObject newBullet = GameObject.Instantiate(bullet, cannon.transform.position, cannon.transform.rotation) as GameObject;
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            
            float randomness = 0.01f;
            Vector3 spread = new Vector3(
            Random.Range(-randomness, randomness),
            Random.Range(-randomness, randomness),
            Random.Range(-randomness, randomness)
            );
            Vector3 randomizedForward = (newBullet.transform.forward + spread).normalized;
            Vector3 shotDirection = (randomizedForward * 500) + (Vector3.up * 1050);
            rb.AddForce(shotDirection);
        }
    }*/
}