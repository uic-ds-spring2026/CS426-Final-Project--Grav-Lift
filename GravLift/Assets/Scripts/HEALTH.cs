/* CS 426 Final Project (Grav Lift)
 * Group members: Rafael Maatouk, Fernando Lopez, Andrew Yoe
 * Description: Script that manages the player's health and its corresponding UI elements
 */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HEALTH : MonoBehaviour {
    /// <summary>
    /// stores the player's health as an integer
    /// </summary>
    [SerializeField] private int health;
    /// <summary>
    /// stores the player's maximum health as an integer
    /// </summary>
    [SerializeField] private int max_health;
    /// <summary>
    /// holds the health bar UI element
    /// </summary>
    [SerializeField] private Slider health_bar;
    /// <summary>
    /// holds the health text UI element displaying the health as numbers
    /// </summary>
    /// 
    [SerializeField] private TextMeshProUGUI health_text;
    /// <summary>
    /// holds the color of the health bar according to how much health the player has
    /// green  if 51 <= health <= 100
    /// yellow if 21 <= health <=  50
    /// red    if  0 <= health <=  20
    /// </summary>
    [SerializeField] private Image health_color;
    private void Awake() {
        health = max_health;
        UpdateUI();
    }

    /// <summary>
    /// Updates the health bar and text
    /// </summary>
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

    /// <summary>
    /// reduces the health by int damage
    /// </summary>
    /// <param name="damage"> how much the player's health should reduce </param>
    public void TakeDamage(int damage){
        health -= damage;
        health = Mathf.Clamp(health, 0, max_health);
        UpdateUI();
    }

    /// <summary>
    /// resets the player's health
    /// </summary>
    public void ResetHealth() {
        health = max_health;
        UpdateUI();
    }

    /// <summary>
    /// determines whether the player is dead
    /// </summary>
    /// <returns> health <= 0 </returns>
    public bool IsDead() {
        return health <= 0;
    }
}