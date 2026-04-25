
using UnityEngine;

public class TurretPlayerTracking : MonoBehaviour {
    public Transform turret;          
    public Transform firePoint;      
    public GameObject bulletPrefab;

    public float range = 75.0f;
    public float fireRate = 3.0f;
    public float rotationSpeed = 150.0f;
    public float fireAngle = 5.0f;     

    [SerializeField] public Transform player;
    private float fireCooldown = 0.0f;

    private enum State { Idle, Tracking, Firing }
    private State currentState = State.Idle;

    void Start() {
    }

    void Update() {
        if (player == null) {
            return;
        }

        float dist = Vector3.Distance(player.position, transform.position);
        // FSM STATE TRANSITIONS
        switch (currentState) {
            case State.Idle:
                if (dist <= range) {
                    currentState = State.Tracking;
                }
                break;

            case State.Tracking:
                if (dist > range) {
                    currentState = State.Idle;
                }
                break;

            case State.Firing:
                if (dist > range) {
                    currentState = State.Idle;
                }
                break;
        }

        //FSM ACTUAL STATES
        switch (currentState) {
            case State.Idle:
                // idling until player nearby
                break;

            case State.Tracking:
                TrackPlayer();

                if (IsAimedAtPlayer(10.0f)) { // strict to start firing
                    currentState = State.Firing;
                }
                break;

            case State.Firing:
                TrackPlayer();
                Fire();

                if (!IsAimedAtPlayer(20.0f)) { // looser to stay firing
                    currentState = State.Tracking;
                }
                break;
        }
    }

    void TrackPlayer() {
        // get player position
        Vector3 dir = player.position - turret.position;

        // get the rotation to look at the player
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        // rotate to the player
        turret.rotation = Quaternion.RotateTowards(
            turret.rotation, 
            lookRotation, 
            rotationSpeed * Time.deltaTime
        );
    }

    bool IsAimedAtPlayer(float angleThreshold) {
        Vector3 dirToPlayer = (player.position - firePoint.position).normalized;
        float angle = Vector3.Angle(firePoint.forward, dirToPlayer);
        return angle < angleThreshold;
    }

    void Fire() {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0.0f) {
            GameObject myClone = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            fireCooldown = 1.0f / fireRate;
            Destroy(myClone, 8.0f);
        }
        
    }

}