using UnityEngine;
using Leap;

public class FistGestureDetector : MonoBehaviour
{
    Controller controller;

    void Start()
    {
        controller = new Controller();
    }

    void Update()
    {
        Frame frame = controller.Frame();
        foreach (Hand hand in frame.Hands)
        {
            if (IsFist(hand))
            {
                OnFistGesture();
            }
        }
    }

    bool IsFist(Hand hand)
    {
        foreach (Finger finger in hand.fingers)
        {
            if (finger.IsExtended) return false;
        }
        return true;
    }

    void OnFistGesture()
    {
        Debug.Log("Fist gesture detected!");
        // Trigger your function here
    }
}

