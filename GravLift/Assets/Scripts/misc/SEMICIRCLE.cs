using UnityEngine;

[ExecuteInEditMode]
public class SEMICIRCLE : MonoBehaviour {
    public GameObject cylinderPrefab;
    public int segments = 100;
    public float radius = 22.0f;
    public float lineThickness = 0.5f;

    public bool generateLine = false;

    void Update() {
        if (generateLine) {
            generateLine = false;
            Create3DLine();
        }
    }

    void Create3DLine() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < segments; i++) {
            float angle = Mathf.Deg2Rad * (i * 180f / segments - 90f);
            float nextAngle = Mathf.Deg2Rad * ((i + 1) * 180f / segments - 90f);

            Vector3 pos1 = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Vector3 pos2 = new Vector3(Mathf.Cos(nextAngle) * radius, 0, Mathf.Sin(nextAngle) * radius);

            GameObject seg = Instantiate(cylinderPrefab, transform);
            
            seg.transform.localPosition = (pos1 + pos2) / 2;
            
            seg.transform.LookAt(transform.TransformPoint(pos2));
            
            seg.transform.Rotate(90, 0, 0);

            float distance = Vector3.Distance(pos1, pos2);
            seg.transform.localScale = new Vector3(lineThickness, distance / 2, lineThickness);
        }
    }
}