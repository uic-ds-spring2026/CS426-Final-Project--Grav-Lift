using UnityEngine;

public class GravityFlip : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Physics.gravity = -Physics.gravity;
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerObj.transform.eulerAngles = new Vector3(0f, 0f, 180f);
            }
        }
        
    }
}
