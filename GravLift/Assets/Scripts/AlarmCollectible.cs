using UnityEngine;

public class AlarmCollectible : MonoBehaviour
{
    public AudioSource alarmSiren;
    public Light[] levelLights; 
    public Color alarmLightColor = Color.red;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player touched the collectible
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        
        if (player != null)
        {
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

            // and set the lights red
            foreach (Light light in levelLights)
            {
                if (light != null)
                {
                    light.color = alarmLightColor;
                }
            }

            // remove the core since we obtained it.
            Destroy(gameObject);
        }
    }
}