using UnityEngine;
using Leap;

public class OrbitalRotation : MonoBehaviour
{
    [Header("Gesture Mode")]
    public bool usePinch = false;

    [Header("Grab Thresholds")]
    public float grabThreshold = 0.8f;
    public float grabReleaseThreshold = 0.7f;

    [Header("Pinch Thresholds")]
    public float pinchThreshold = 0.8f;
    public float pinchReleaseThreshold = 0.7f;

    [Header("Rotation Settings")]
    public Transform pivot;
    public float momentumDamping = 4f;
    public float grabRadius = 0.3f;

    private Controller controller;
    private bool isGrabbing = false;
    private Hand activeHand;
    private Vector3 initialDir;
    private Quaternion initialRot;

    private Vector3 momentumAxis;
    private float momentumSpeed = 0f;

    void Start()
    {
        controller = new Controller();
        if (pivot == null) pivot = transform;
    }

    void Update()
    {
        Frame frame = controller.Frame();
        float now = Time.time;

        foreach (var hand in frame.Hands)
        {
            float strength = usePinch ? hand.PinchStrength : hand.GrabStrength;
            float startThreshold = usePinch ? pinchThreshold : grabThreshold;
            float endThreshold = usePinch ? pinchReleaseThreshold : grabReleaseThreshold;

            Vector3 handPos = hand.PalmPosition;
            Vector3 toHand = handPos - pivot.position;
            if (toHand.magnitude > grabRadius) continue;

            if (!isGrabbing && strength >= startThreshold)
            {
                isGrabbing = true;
                activeHand = hand;
                initialDir = toHand.normalized;
                initialRot = transform.rotation;
                momentumSpeed = 0f;
                momentumAxis = Vector3.zero;
                break;
            }

            if (isGrabbing && hand.Id == activeHand.Id)
            {
                if (strength < endThreshold)
                {
                    isGrabbing = false;
                    break;
                }

                Vector3 currentDir = toHand.normalized;
                Quaternion delta = Quaternion.FromToRotation(initialDir, currentDir);
                transform.rotation = delta * initialRot;

                delta.ToAngleAxis(out float angle, out Vector3 axis);
                if (angle > 180f) angle -= 360f;
                float angleRad = angle * Mathf.Deg2Rad;
                momentumAxis = axis;
                momentumSpeed = angleRad / Time.deltaTime;

                initialDir = currentDir;
                initialRot = transform.rotation;
                break;
            }
        }

        if (!isGrabbing && momentumSpeed > 0f)
        {
            float angle = momentumSpeed * Time.deltaTime * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, momentumAxis) * transform.rotation;
            momentumSpeed = Mathf.MoveTowards(momentumSpeed, 0f, momentumDamping * Time.deltaTime);
        }
    }
}
