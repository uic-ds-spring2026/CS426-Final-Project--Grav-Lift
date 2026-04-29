using UnityEngine;

public partial class AlarmCollectible : MonoBehaviour
{
    public AudioSource alarmSiren;
    public Light[] levelLights; 
    public Color alarmLightColor = Color.red;
    public GameObject winPrefab; 
    public Transform spawnPoint;

    public DoorController[] linkedDoors; 

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        
        if (player != null)
        {


            
            Instantiate(winPrefab, spawnPoint.position, spawnPoint.rotation);
            // disable gravity flipping
            GRAVITY gravityScript = player.GetComponent<GRAVITY>();
            if (gravityScript != null)
            {
                gravityScript.canFlipGravity = false;
            }

            // play the siren
            if (alarmSiren != null)
            {
                alarmSiren.Play();
            }

            // set the lights red
            foreach (Light light in levelLights)
            {
                if (light != null)
                {
                    light.color = alarmLightColor;
                }
            }

            // open every door
            foreach (DoorController door in linkedDoors)
            {
                if (door != null)
                {
                    door.OpenDoor();
                }
            }

            // remove the core
            Destroy(gameObject);
        }
    }
}