using UnityEngine;

public class MissilePathfinding : MonoBehaviour
{
    private Transform target;
    private Rigidbody rb;
    private bool hasPlayedAudio = false; 
    
    public float rotationSpeed = 200f; 
    public float speed = 10f;

    public AudioSource audioSource;
    public AudioClip closeProximitySound;
    public float alertDistance = 10f; 

    public int missileDamage = 25; 
    public GameObject explosionPrefab;

    void Start()
    {
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        
        if (player != null) {
            target = player.transform;
        }
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
    }

    void FixedUpdate()
    {
        if (target == null) return;

        
        Vector3 aimTarget = target.position + new Vector3(0, 1.0f, 0); 
        Vector3 direction = (aimTarget - rb.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion newRotation = Quaternion.RotateTowards(
            rb.rotation, 
            lookRotation, 
            rotationSpeed * Time.fixedDeltaTime
        );
        rb.MoveRotation(newRotation);

        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);

        if (!hasPlayedAudio && audioSource != null && closeProximitySound != null) 
        {
            float distanceToTarget = Vector3.Distance(rb.position, target.position);
            
            if (distanceToTarget <= alertDistance) 
            {
                audioSource.PlayOneShot(closeProximitySound);
                hasPlayedAudio = true; 
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerMovement playerScript = collision.gameObject.GetComponentInParent<PlayerMovement>();
        
        if (playerScript != null)
        {
            playerScript.TakeDamage(missileDamage);
        }

        if (explosionPrefab != null) 
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}