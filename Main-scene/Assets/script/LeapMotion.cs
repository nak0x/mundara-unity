using UnityEngine;
using Leap;
using System.Collections.Generic;
using System.Collections;

public class LeapMotion : MonoBehaviour
{
    Controller controller;
    public float minSwipeDistance = 0.2f; // in meters
    public float maxSwipeDuration = 1.0f; // in seconds

    public stepManager stepManager;

    public bool canSwipe;

    private class SwipeTracker
    {
        public Vector3 startPosition;
        public float startTime;
    }

    private Dictionary<int, SwipeTracker> activeSwipes = new();

    void Start()
    {
        canSwipe = true;
        Debug.Log("start leap");
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
                    if (canSwipe == true)
                    {
                        Debug.Log("Swiping !!!");
                        canSwipe = false;
                        if (distance > 0)
                            OnSwipeRight(hand);
                        else
                            OnSwipeLeft(hand);

                        activeSwipes.Remove(id); // prevent re-trigger
                        StartCoroutine(DelaySwipe(3f));
                    }
                    else
                    {
                        Debug.Log("Cant swipe cause : delay interval");
                    }

                    
                }
                else if (elapsed > maxSwipeDuration)
                {
                    // Timeout
                    activeSwipes.Remove(id);
                }
            }
        }
    }

    IEnumerator DelaySwipe(float interval)
    { 
        yield return new WaitForSeconds(interval);
        canSwipe = true;
        //Debug.Log("Tick at: " + Time.time);

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
        stepManager.updateStep(STEPDIRECTION.Backward);
    }

    void OnSwipeLeft(Hand hand)
    {
        Debug.Log($"Swipe Left with {(hand.IsLeft ? "Left" : "Right")} hand.");
        // Your logic here
        stepManager.updateStep(STEPDIRECTION.Forward);
    }
}