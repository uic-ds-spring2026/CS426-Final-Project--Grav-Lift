using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioSource audioToPlay;

    void OnTriggerEnter(Collider other)
    {
        // Checks if the colliding object has the PlayerMovement component
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement playerRef))
        {
            // Plays the audio if it isn't already playing
            if (!audioToPlay.isPlaying)
            {
                audioToPlay.Play();
            }
            
        }
    }
}