using UnityEngine;
using UnityEngine.SceneManagement;
public class WinScreen : MonoBehaviour

{
    public string sceneName;
    private void OnTriggerEnter(Collider other)
{
    PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

    if (player != null)
    {
        SceneManager.LoadScene(sceneName);
    }
}
}
