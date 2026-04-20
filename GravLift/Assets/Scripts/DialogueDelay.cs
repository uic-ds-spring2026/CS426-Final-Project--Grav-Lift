using System.Collections;
using UnityEngine;

public class DialogueDelay : MonoBehaviour
{

    public AudioSource audioSource;
    
    public AudioClip dialogueClip;

    public float delayInSeconds = 6f;

    void Start()
    {
        StartCoroutine(PlayDialogueAfterDelay());
    }

    private IEnumerator PlayDialogueAfterDelay()
    {
        // Wait 6 seconds
        yield return new WaitForSeconds(delayInSeconds);

        // Audio source existence check
        if (audioSource != null)
        {
            // clip existence and setting check
            if (dialogueClip != null)
            {
                audioSource.clip = dialogueClip;
            }

            // play the dialogue
            audioSource.Play();
        }
        else
        {   
            // debugging
            Debug.LogWarning("Audio Source Issue: " + gameObject.name);
        }
    }
}