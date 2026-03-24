using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SWISH : MonoBehaviour {
    public static int score;
    public int maxScore = 5;
    public TMP_Text scoreText;

    void Start() {
        score = 0;
        scoreText.text = "Score: " + score;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("BALL")) {
            score++;
            Debug.Log("SWISH! score: " + score);
            if (scoreText != null) {
                if (score < maxScore) {
                    scoreText.text = "Score: " + score;
                }
                else {
                    scoreText.text = "You won!";
                }
            }
        }
    }
}