using UnityEngine;
using Leap;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class GhostRotationGrab : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform pivot;
    public Transform target;

    [Header("Grab Settings")]
    public float grabRadius = 0.3f;
    public float grabActivationDelay = 0.1f;
    public float grabReleaseDelay = 0.2f;

    [Header("Momentum Settings")]
    public float angularVelocityThreshold = 0.5f; // rad/s
    public float momentumDecay = 4f;

    [Header("Fist Detection")]
    public float fingerTipDistance = 5f; // cm
    public float curledFingerRatio = 0.6f;

    private Controller controller;
    private bool isGrabbing = false;
    private float grabStartTime = 0f;
    private float grabEndTime = 0f;

    private Vector3 lastGrabDir;
    private Quaternion grabInitialRotation;

    private Vector3 momentumAxis;
    private float momentumSpeed = 0f;

    void Start()
    {
        controller = new Controller();
        if (target == null) target = transform;
    }

    void Update()
    {
        Frame frame = controller.Frame();
        float now = Time.time;
        bool activeHandFound = false;

        foreach (Hand hand in frame.Hands)
        {
            if (!hand.IsRight) continue;

            Vector3 handPos = hand.PalmPosition;
            Vector3 toHand = handPos - pivot.position;
            Vector3 currentDir = toHand.normalized;

            bool handInRange = toHand.magnitude <= grabRadius;
            bool grabbingPose = IsGrabbing(hand);

            // Draw debug visuals
            Debug.DrawLine(pivot.position, handPos, Color.red);
            if (isGrabbing)
            {
                Debug.DrawRay(handPos, Vector3.up * 0.05f, Color.green);
                Debug.DrawRay(handPos, Vector3.right * 0.05f, Color.green);
                Debug.DrawRay(handPos, Vector3.forward * 0.05f, Color.green);
            }

            activeHandFound = true;

            if (!isGrabbing)
            {
                if (grabbingPose && handInRange)
                {
                    if (grabStartTime == 0f) grabStartTime = now;

                    if (now - grabStartTime >= grabActivationDelay)
                    {
                        lastGrabDir = currentDir;
                        grabInitialRotation = target.rotation;
                        isGrabbing = true;
                        grabEndTime = 0f;
                        momentumAxis = Vector3.zero;
                        momentumSpeed = 0f;
                    }
                }
                else
                {
                    grabStartTime = 0f;
                }
            }
            else
            {
                if (!grabbingPose || !handInRange)
                {
                    if (grabEndTime == 0f) grabEndTime = now;

                    if (now - grabEndTime >= grabReleaseDelay)
                    {
                        isGrabbing = false;
                        grabStartTime = 0f;

                        // Compute momentum from hand angular velocity
                        Vector3 angularVelocity = hand.PalmVelocity; // radians/sec

                        if (angularVelocity.magnitude >= angularVelocityThreshold)
                        {
                            momentumAxis = angularVelocity.normalized;
                            momentumSpeed = angularVelocity.magnitude;
                        }
                    }
                }
                else
                {
                    grabEndTime = 0f;

                    Quaternion delta = Quaternion.FromToRotation(lastGrabDir, currentDir);
                    target.rotation = delta * target.rotation;

                    lastGrabDir = currentDir;
                }
            }

            break; // Only use one hand
        }

        // Apply momentum when no hand is grabbing
        if (!isGrabbing && momentumSpeed > 0f)
        {
            float angle = momentumSpeed * Mathf.Rad2Deg * Time.deltaTime;
            Quaternion momentumDelta = Quaternion.AngleAxis(angle, momentumAxis);
            target.rotation = momentumDelta * target.rotation;

            momentumSpeed = Mathf.MoveTowards(momentumSpeed, 0f, Time.deltaTime * momentumDecay);
        }
    }

    bool IsGrabbing(Hand hand)
    {
        Vector3 palm = hand.PalmPosition;
        int curled = 0;

        foreach (Finger finger in hand.fingers)
        {
            if (finger.Type == Finger.FingerType.THUMB) continue;

            float dist = Vector3.Distance(finger.TipPosition, palm);
            if (dist < fingerTipDistance / 100f) curled++;
        }

        return (float)curled / 4f >= curledFingerRatio;
    }
}
