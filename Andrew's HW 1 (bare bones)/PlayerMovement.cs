// shoot
// using __ imports namespace
// Namespaces are collection of classes, data types
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// MonoBehavior is the base class from which every Unity Script Derives
public class PlayerMovement : MonoBehaviour {
    public float speed = 25.0f;
    public float rotationSpeed = 45f;
    public float force = 700f;
    int framesPressed = 0;

    public GameObject cannon;
    public GameObject bullet;

    Rigidbody rb;
    Transform t;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
    }

    // Update is called once per frame
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
    }
}