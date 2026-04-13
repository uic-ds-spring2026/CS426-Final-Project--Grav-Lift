using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How far and in what direction the door should move. (e.g., 0, 5, 0 moves it up by 5)")]
    public Vector3 moveOffset = new Vector3(0, 5f, 0); 
    public float moveSpeed = 3f;

    [Header("Audio Settings")]
    public AudioClip doorMovingSound;
    private AudioSource audioSource;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;

    void Start()
    {
        // Require an AudioSource component on the door
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Calculate positions based on the door's starting spot
        closedPosition = transform.position;
        openPosition = closedPosition + moveOffset;
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            PlayDoorSound();
            StopAllCoroutines(); // Stop closing if it's currently closing
            StartCoroutine(MoveDoor(openPosition));
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            PlayDoorSound();
            StopAllCoroutines(); // Stop opening if it's currently opening
            StartCoroutine(MoveDoor(closedPosition));
        }
    }

    private void PlayDoorSound()
    {
        if (doorMovingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(doorMovingSound);
        }
    }

    // Coroutine to animate the door smoothly over time
    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        // While the door is not yet at the target position...
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // Move a little bit towards the target every frame
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Wait until the next frame to continue the loop
            yield return null; 
        }
        
        // Ensure it snaps perfectly to the target at the end
        transform.position = targetPosition; 
    }
}