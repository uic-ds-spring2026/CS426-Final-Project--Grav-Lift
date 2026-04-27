using UnityEngine;

public class TransformBridge : MonoBehaviour
{
    //Each bridge is a target and they move/scale slightly differently so
    //we need 3 targets and 2 moves
    public Transform targetA;
    public Transform targetB;
    public Transform targetC;

    public Vector3 moveA;
    public Vector3 moveBC;

    //Same scale change for all 3
    public Vector3 scale;


    public float speed = 2f; // controls how fast it moves

    private static int pressedCount = 0;

    // Saving the original positions of the 3 of them and sets their target locations
    // Needed for all 3 since they have different positions
    private Vector3 origPosA, origPosB, origPosC;
    private Vector3 origScaleA, origScaleB, origScaleC;

    private Vector3 targetPosA, targetPosB, targetPosC;
    private Vector3 targetScaleA, targetScaleB, targetScaleC;

    void Start()
    {
        origPosA = targetA.position;
        origPosB = targetB.position;
        origPosC = targetC.position;

        //Scales slightly vary so this is needed
        origScaleA = targetA.localScale;
        origScaleB = targetB.localScale;
        origScaleC = targetC.localScale;

        SetTargetsToOriginal();
    }

    void Update()
    {
        targetA.position = Vector3.Lerp(targetA.position, targetPosA, Time.deltaTime * speed);
        targetB.position = Vector3.Lerp(targetB.position, targetPosB, Time.deltaTime * speed);
        targetC.position = Vector3.Lerp(targetC.position, targetPosC, Time.deltaTime * speed);

        targetA.localScale = Vector3.Lerp(targetA.localScale, targetScaleA, Time.deltaTime * speed);
        targetB.localScale = Vector3.Lerp(targetB.localScale, targetScaleB, Time.deltaTime * speed);
        targetC.localScale = Vector3.Lerp(targetC.localScale, targetScaleC, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("CUBE")) return;

        pressedCount++;

        if (pressedCount == 2)
            Activate();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("CUBE")) return;
 
        if (pressedCount == 2)
            Deactivate();

        pressedCount--;
    }

    void Activate()
    {
        targetPosA = origPosA + moveA;
        targetPosB = origPosB + moveBC;
        targetPosC = origPosC + moveBC;

        targetScaleA = origScaleA + scale;
        targetScaleB = origScaleB + scale;
        targetScaleC = origScaleC + scale;
    }

    void Deactivate()
    {
        SetTargetsToOriginal();
    }

    void SetTargetsToOriginal()
    {
        targetPosA = origPosA;
        targetPosB = origPosB;
        targetPosC = origPosC;

        targetScaleA = origScaleA;
        targetScaleB = origScaleB;
        targetScaleC = origScaleC;
    }
}