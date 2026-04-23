using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HEALTH : MonoBehaviour {
    [SerializeField] private int health;
    [SerializeField] private int max_health;
    [SerializeField] private Slider health_bar;
    [SerializeField] private TextMeshProUGUI health_text;
    [SerializeField] private Image health_color;
    private void Awake() {
        health = max_health;
        UpdateUI();
    }

    private void Update(){
        // FOR TESTING PURPOSES ONLY
        if (Input.GetKeyDown(KeyCode.P)){
            TakeDamage(10);
            Debug.Log("Health is now " + health);
        }
    }

    private void UpdateUI() {
        // controls the health bar
        if (health_bar != null) {
            health_bar.value = (float) health / max_health;
        }

        // controls the health text
        if (health_text != null) {
            health_text.text = "HP: " + health.ToString() + " / " + max_health.ToString();
        }

        // controls the health color
        if (health_color != null && health_text != null) {
            if (health <= 20) {
                health_color.color = Color.red;
                health_text.color = Color.red;
            } else if (health <= 50) {
                health_color.color = Color.yellow;
                health_text.color = Color.yellow;
            } else {
                health_color.color = Color.green;
                health_text.color = Color.green;
            }
        }
    }

    public void TakeDamage(int damage){
        health -= damage;
        health = Mathf.Clamp(health, 0, max_health);
        UpdateUI();
    }

    public void ResetHealth() {
        health = max_health;
        UpdateUI();
    }

    public bool IsDead() {
        return health <= 0;
    }
}