using UnityEngine;

public class MissilePathfinding : MonoBehaviour
{
    private GameObject target;
    private Rigidbody rb;
    
    public float rotationSpeed = 5f;
    public float speed = 10f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        
        rb.freezeRotation = true; 
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 direction = (target.transform.position - rb.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.fixedDeltaTime));

        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);
    }
}