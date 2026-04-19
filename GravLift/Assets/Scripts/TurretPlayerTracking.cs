
using UnityEngine;

public class TurretPlayerTracking : MonoBehaviour {
    public Transform turret;          
    public Transform firePoint;      
    public GameObject bulletPrefab;

    public float range = 25.0f;
    public float fireRate = 1.0f;
    public float rotationSpeed = 5.0f;
    public float fireAngle = 5.0f;     

    private Transform player;
    private float fireCooldown = 0.0f;

    private enum State { Idle, Tracking, Firing }
    private State currentState = State.Idle;

    void Start() {
        GameObject playerObj = GameObject.FindGameObjectWithTag("AIMHERE");
        if (playerObj != null) {
            player = playerObj.transform;
        }
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

                if (IsAimedAtPlayer(5.0f)) { // strict to start firing
                    currentState = State.Firing;
                }
                break;

            case State.Firing:
                TrackPlayer();
                Fire();

                if (!IsAimedAtPlayer(10.0f)) { // looser to stay firing
                    currentState = State.Tracking;
                }
                break;
        }
    }

    void TrackPlayer() {
        Vector3 dir = player.position - turret.position;

        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Slerp(turret.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        turret.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    bool IsAimedAtPlayer(float angleThreshold) {
        Vector3 dirToPlayer = (player.position - firePoint.position).normalized;
        float angle = Vector3.Angle(firePoint.forward, dirToPlayer);
        return angle < angleThreshold;
    }

    void Fire() {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0.0f) {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            fireCooldown = 1.0f / fireRate;
        }
    }
}