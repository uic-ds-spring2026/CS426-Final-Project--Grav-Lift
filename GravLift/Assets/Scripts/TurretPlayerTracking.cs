using UnityEngine;

public class TurretPlayerTracking : MonoBehaviour
{
    public Transform turret;          
    public Transform firePoint;      
    public GameObject bulletPrefab;

    public float range = 25f;
    public float fireRate = 1f;
    public float rotationSpeed = 5f;
    public float fireAngle = 5f;     

    private Transform player;
    private float fireCooldown = 0f;

    private enum State { Idle, Tracking, Firing }
    private State currentState = State.Idle;

    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        // FSM STATE TRANSITIONS
        switch (currentState)
        {
            case State.Idle:
                if (dist <= range)
                    currentState = State.Tracking;
                break;

            case State.Tracking:
                if (dist > range)
                    currentState = State.Idle;
                break;

            case State.Firing:
                if (dist > range)
                    currentState = State.Idle;
                break;
        }

        //FSM ACTUAL STATES
        switch (currentState)
        {
            case State.Idle:
                // idling until player nearby
                break;

            case State.Tracking:
                TrackPlayer();

                if (IsAimedAtPlayer())
                    currentState = State.Firing;
                break;

            case State.Firing:
                TrackPlayer();
                Fire();

                if (!IsAimedAtPlayer())
                    currentState = State.Tracking;
                break;
        }
    }

    void TrackPlayer()
    {
        Vector3 lookPos = player.position;
        lookPos.y = turret.position.y;

        Vector3 dir = (lookPos - turret.position).normalized;

        // the prefab barrel faces the X axis which sucks so it needs to rotate -90 on y axis
        Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -90, 0);

        turret.rotation = Quaternion.Slerp(
            turret.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    bool IsAimedAtPlayer()
    {
        Vector3 dirToPlayer = (player.position - turret.position).normalized;

        // use turret.right because barrel faces X axis
        float angle = Vector3.Angle(turret.right, dirToPlayer);

        return angle < fireAngle;
    }

    void Fire()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            fireCooldown = 10f / fireRate;
        }
    }
}