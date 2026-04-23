/* CS 426 Final Project (Grav Lift)
 * Group members: Rafael Maatouk, Fernando Lopez, Andrew Yoe
 * Description: Script that manages the arrow UI element
 */
using UnityEngine;
using UnityEngine.UI;

public class ARROW : MonoBehaviour {
    [SerializeField] private GRAVITY gravity;
    private Image arrow;
    private RectTransform rect_transform;

    private void Awake() {
        arrow = GetComponent<Image>();
        rect_transform = GetComponent<RectTransform>();
    }

    private void Update() {
        rect_transform.localRotation = Quaternion.Euler(0.0f, 0.0f, gravity.IsUpsideDown() ? 90.0f : -90.0f);
        arrow.color = gravity.IsUpsideDown() ? Color.red : Color.blue;
    }
}