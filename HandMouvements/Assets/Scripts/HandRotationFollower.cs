using UnityEngine;
using Leap;

public class HandRotationFollower : MonoBehaviour
{
    public Transform target; // Assign target via Inspector or leave null to use this GameObject
    public int requiredExtendedFingers = 2; // Semi-closed hand (e.g. 2 fingers)

    private Controller controller;

    void Start()
    {
        controller = new Controller();
        if (target == null)
            target = transform;
    }

    void Update()
    {
        Frame frame = controller.Frame();

        foreach (Hand hand in frame.Hands)
        {
            if (!hand.IsRight) continue; // Only right hand

            int extended = 0;
            foreach (Finger f in hand.fingers)
                if (f.IsExtended) extended++;

            if (extended != requiredExtendedFingers) continue;

            target.rotation = hand.Rotation; // Leap 7.2: already a Unity-compatible Quaternion
        }
    }
}

