using UnityEngine;
using Leap;
using System.Collections.Generic;

public class LeapMotion : MonoBehaviour
{
    Controller controller;
    public float minSwipeDistance = 0.2f; // in meters
    public float maxSwipeDuration = 1.0f; // in seconds

    private class SwipeTracker
    {
        public Vector3 startPosition;
        public float startTime;
    }

    private Dictionary<int, SwipeTracker> activeSwipes = new();

    void Start()
    {
        controller = new Controller();
    }

    void Update()
    {
        Frame frame = controller.Frame();

        foreach (Hand hand in frame.Hands)
        {
            int id = hand.Id;

            if (!IsHandOpen(hand))
            {
                activeSwipes.Remove(id);
                continue;
            }

            if (!activeSwipes.ContainsKey(id))
            {
                // Start tracking
                activeSwipes[id] = new SwipeTracker
                {
                    startPosition = hand.PalmPosition,
                    startTime = Time.time
                };
            }
            else
            {
                SwipeTracker tracker = activeSwipes[id];
                float elapsed = Time.time - tracker.startTime;
                float distance = hand.PalmPosition.x - tracker.startPosition.x;

                if (elapsed <= maxSwipeDuration && Mathf.Abs(distance) >= minSwipeDistance)
                {
                    if (distance > 0)
                        OnSwipeRight(hand);
                    else
                        OnSwipeLeft(hand);

                    activeSwipes.Remove(id); // prevent re-trigger
                }
                else if (elapsed > maxSwipeDuration)
                {
                    // Timeout
                    activeSwipes.Remove(id);
                }
            }
        }
    }

    bool IsHandOpen(Hand hand)
    {
        int extended = 0;
        foreach (Finger f in hand.fingers)
            if (f.IsExtended) extended++;

        return extended >= 5;
    }

    void OnSwipeRight(Hand hand)
    {
        Debug.Log($"Swipe Right with {(hand.IsLeft ? "Left" : "Right")} hand.");
        // Your logic here
    }

    void OnSwipeLeft(Hand hand)
    {
        Debug.Log($"Swipe Left with {(hand.IsLeft ? "Left" : "Right")} hand.");
        // Your logic here
    }
}