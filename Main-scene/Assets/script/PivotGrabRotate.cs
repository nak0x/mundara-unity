using UnityEngine;
using Leap;

public class PivotGrabRotate : MonoBehaviour
{
    [Header("Target Object")]
    public Transform targetObject; // The object to be grabbed and rotated

    [Header("Pivot Settings")]
    public Transform pivotPoint;
    [Header("Hand Tracking Settings")]
    public Chirality trackedHand = Chirality.Right;
    [Header("Grab Settings")]
    public float grabThreshold = 0.8f;
    public float releaseThreshold = 0.7f;
    public float momentumDamping = 5f; // higher = faster decay
    public Vector2 handDistanceRange = new Vector2(0.1f, 1f); // min and max distance for hand to be considered "close"

    [Header("Hand Renderer Settings")]
    [Tooltip("Renderer for the hand model, used to change color based on grab state.")]
    public Renderer handRenderer;
    public Color notGrabbingColor = Color.gray;
    public Color grabbingColor = Color.blue;
    public Color notInRangeColor = Color.red;

    [Header("Rotation Constraints")]
    public Vector3 blockRotation = Vector3.zero; // 1 = block axis, 0 = allow
    public float rotationFactor = 1f; // scales the rotation applied
    public float rotationDPI = 1f; // DPI-like sensitivity multiplier

    [Header("Debugging")]
    public bool debugging = false;


    // Private variables
    private bool isGrabbing = false;
    private Hand activeHand = null;
    private Vector3 initialPivotVector;
    private Quaternion initialObjectRot;
    private Vector3 angularVelocity = Vector3.zero;
    private Material handMaterialInstance;
    private Rigidbody rb;

    void Start()
    {
        if (handRenderer != null)
        {
            handMaterialInstance = handRenderer.material; // This returns an *instance* (not sharedMaterial)
        }
        SetHandColor(notGrabbingColor);
    }

    void Update()
    {
        Hand hand = Hands.Provider.GetHand(trackedHand);
        if (hand == null)
        {
            return;
        }

        if (debugging)
        {
            Debug.Log($"Hand {trackedHand} - Grab Strength: {hand.GrabStrength}, Palm Position: {hand.PalmPosition}");
            Debug.Log($"Is Grabbing: {isGrabbing}, Active Hand: {activeHand?.Id}, Distance to Pivot: {EvaluateHandDistance(hand)}");
            Debug.Log($"Is Hand In Range: {isHandInRange(hand)}");
            Debug.Log($"Angular Velocity: {angularVelocity}, Initial Pivot Vector: {initialPivotVector}, Initial Object Rotation: {initialObjectRot}");

            // Draw debug lines
            Debug.DrawLine(pivotPoint.position, hand.PalmPosition, Color.cyan);

            if (isGrabbing)
            {
                Debug.DrawRay(transform.position, angularVelocity.normalized * 0.2f, Color.yellow);
                Debug.DrawLine(pivotPoint.position, pivotPoint.position + initialPivotVector, Color.green);
                Debug.DrawRay(hand.PalmPosition, Vector3.up * 0.1f, Color.blue);
            }
        }

        AdaptHandColor(hand);
        Grabbing(hand);
        Orbitting(hand);
    }

    public void Grabbing(Hand hand)
    {
        float grabStrength = hand.GrabStrength;

        if (!isGrabbing && grabStrength > grabThreshold)
        {
            // START GRAB
            isGrabbing = true;
            activeHand = hand;
            initialPivotVector = hand.PalmPosition - pivotPoint.position;
            initialObjectRot = targetObject.rotation;
            angularVelocity = Vector3.zero;
        }
        else if (isGrabbing && grabStrength < releaseThreshold)
        {
            // RELEASE GRAB
            isGrabbing = false;

            activeHand = null;
            initialPivotVector = Vector3.zero;
            initialObjectRot = Quaternion.identity;
        }
    }

    public void Orbitting(Hand hand)
    {
        if (isGrabbing && activeHand != null && isHandInRange(hand))
        {
            Vector3 currentPivotVector = activeHand.PalmPosition - pivotPoint.position;
            if (currentPivotVector.sqrMagnitude < 1e-6f || initialPivotVector.sqrMagnitude < 1e-6f)
                return;

            Quaternion fromTo = Quaternion.FromToRotation(initialPivotVector, currentPivotVector);
            Quaternion targetRot = Quaternion.SlerpUnclamped(Quaternion.identity, fromTo, rotationDPI) * initialObjectRot;

            // Apply rotation constraints
            Vector3 euler = targetRot.eulerAngles;
            euler = Vector3.Scale(euler, Vector3.one - blockRotation);
            targetObject.rotation = Quaternion.Euler(euler);

            Quaternion deltaRot = targetRot * Quaternion.Inverse(initialObjectRot);
            deltaRot.ToAngleAxis(out float angleDegrees, out Vector3 axis);
            if (angleDegrees > 180f) angleDegrees -= 360f;
            float angleRad = angleDegrees * Mathf.Deg2Rad * rotationFactor;

            if (Mathf.Abs(angleRad) > 1e-5f)
                angularVelocity = Vector3.Scale(axis.normalized * (angleRad / Time.deltaTime), Vector3.one - blockRotation);
            else
                angularVelocity = Vector3.zero;

            initialObjectRot = targetObject.rotation;
            initialPivotVector = currentPivotVector;
        }
        else if (!isGrabbing && rb == null)
        {
            if (angularVelocity != Vector3.zero)
            {
                targetObject.Rotate(Vector3.Scale(angularVelocity, Vector3.one - blockRotation) * Mathf.Rad2Deg * Time.deltaTime, Space.World);
                angularVelocity = Vector3.Lerp(angularVelocity, Vector3.zero, momentumDamping * Time.deltaTime);

                if (angularVelocity.magnitude < 0.01f)
                    angularVelocity = Vector3.zero;
            }
        }
    }

    public bool isHandInRange(Hand hand)
    {
        float distance = Vector3.Distance(hand.PalmPosition, pivotPoint.position);
        return distance >= handDistanceRange.x && distance <= handDistanceRange.y;
    }

    public void AdaptHandColor(Hand hand)
    {
        Color color;
        if (!isHandInRange(hand))
            color = notInRangeColor;
        else if (isGrabbing)
            color = grabbingColor;
        else
            color = notGrabbingColor;

        SetHandColor(color);
    }

    public float EvaluateHandDistance(Hand hand)
    {
        return Vector3.Distance(hand.PalmPosition, pivotPoint.position);
    }

    public void SetHandColor(Color color)
    {
        if (handMaterialInstance != null)
        {
            handMaterialInstance.SetColor("_FresnelColor", color);
        }
    }
}