/* CS 426 Final Project
 * Group members: Rafael Maatouk, Fernando Lopez, Andrew Yoe
 * Description: Script that manages missle path finding
 */

using UnityEngine;

public class MISSILE : MonoBehaviour {
    private Transform target;
    private Rigidbody rb;
    private bool has_played_audio = false; 
    
    public float rotation_speed = 200f; 
    public float speed = 10f;

    public AudioSource audio_source;
    public AudioClip close_proximity_sound;
    public float alert_distance = 10f; 

    public int damage = 25;
    public GameObject player_object;

    void Start() {
        player_object = GameObject.FindGameObjectWithTag("Player");
        if (player_object != null) {
            target = player_object.transform;
        }
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
    }

    void FixedUpdate() {
        if (target == null) {
            return;
        }
        
        Vector3 aim_target = target.position + new Vector3(0, 1.0f, 0); 
        Vector3 direction = (aim_target - rb.position).normalized;

        Quaternion look_rotation = Quaternion.LookRotation(direction);
        Quaternion new_rotation = Quaternion.RotateTowards(
            rb.rotation, 
            look_rotation, 
            rotation_speed * Time.fixedDeltaTime
        );
        rb.MoveRotation(new_rotation);

        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);

        if (!has_played_audio && audio_source != null && close_proximity_sound != null) {
            float distance_to_target = Vector3.Distance(rb.position, target.position);
            
            if (distance_to_target <= alert_distance) {
                audio_source.PlayOneShot(close_proximity_sound);
                has_played_audio = true; 
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            PLAYER player_script = collision.gameObject.GetComponentInParent<PLAYER>();
            if (player_script != null) {
                player_script.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}