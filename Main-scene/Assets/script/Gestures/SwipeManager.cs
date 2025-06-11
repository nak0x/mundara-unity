using System;
using UnityEngine;
using Leap;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Scripting;

namespace Gestures
{
    /// <summary>
    /// LeapMotion class handles swipe gestures using the Leap Motion controller.
    /// It detects left and right swipes based on hand position and palm orientation.
    /// The class requires a Leap Motion controller and implements swipe detection logic.
    /// It uses a minimum swipe distance and maximum swipe duration to determine valid swipes.
    /// The class also includes a delay mechanism to prevent rapid consecutive swipes.
    /// </summary>
    public class SwipeManager: MonoBehaviour, ILeapMotionActionInterface
    {
        Controller controller;

        [Header("Swipe Settings")]
        public float minSwipeDistance = 0.2f; // in meters
        public float maxSwipeDuration = 1.0f; // in seconds

        [Header("Dependencies")]
        public PivotGrabRotate pivotGrabRotate;

        // [RequiredInterface(ILeapMotionActionInterface)]
        public MonoBehaviour LeapMotionActionMono;
        private ILeapMotionActionInterface LeapMotionAction;


        [System.NonSerialized]
        public bool canSwipe;

        private class SwipeTracker
        {
            public Vector3 startPosition;
            public float startTime;
        }

        private Dictionary<int, SwipeTracker> activeSwipes = new();

        void Awake()
        {
            LeapMotionAction = LeapMotionActionMono as ILeapMotionActionInterface;
            if (LeapMotionAction == null)
            {
                Debug.LogError("L'objet assigné ne contient pas l'implémentation de ILeapMotionActionInterface");
            }
        }

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
                        if (canSwipe == true && pivotGrabRotate.isGrabbing == false)
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

        // ReSharper disable Unity.PerformanceAnalysis
        void OnSwipeRight(Hand hand)
        {
            Debug.Log($"Swipe Right with {(hand.IsLeft ? "Left" : "Right")} hand.");
            LeapMotionAction.SwipeRight();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        void OnSwipeLeft(Hand hand)
        {
            Debug.Log($"Swipe Left with {(hand.IsLeft ? "Left" : "Right")} hand.");
            LeapMotionAction.SwipeLeft();
        }
    }

    public interface ILeapMotionActionInterface
    {
        public void SwipeRight() { }

        public void SwipeLeft() { }
    }
}