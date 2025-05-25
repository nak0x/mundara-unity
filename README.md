# Mundara Unity Projects

## Overview

**Mundara** is a multi-faceted project exploring interactive 3D experiences using Unity. This repository contains **three separate Unity projects**:

* **ClaySim** – Real-time soft-body clay simulation.
* **HandMouvements** – Ultraleap Leap Motion hand tracking and gesture recognition.
* **Main-scene** – Microsoft Kinect-based full-body tracking.

Each module is a standalone Unity project that can be run independently or integrated into a larger immersive system.

---

## Repository Structure

```
/
├── ClaySim/            # Soft body mesh deformation project
├── HandMouvements/     # Hand tracking and gesture recognition project
├── Main-scene/         # Body tracking and joint detection using Kinect
```

---

## Features

### ClaySim

* Deformable mesh simulating clay
* Collision-driven and key-press driven deformation
* Adaptive triangle subdivision for detailed deformation

### HandMouvements

* Leap Motion hand tracking with 3D rendered hands
* Fist gesture detection
* Swipe left/right gesture detection

### Main-scene

* Kinect v2-based full-body tracking
* Real-time skeleton visualization
* Camera follows tracked joint (e.g., head)

---

## Requirements

* **Unity Version:** Unity 2020.3.43f1 (LTS recommended)
* **ClaySim:** No external hardware required
* **HandMouvements:** Ultraleap Leap Motion + Ultraleap Tracking Software
* **Main-scene:** Microsoft Kinect v2 + Kinect for Windows SDK 2.0

---

## Installation

1. Clone the repo

   ```bash
   git clone https://github.com/nak0x/mundara-unity.git
   ```
2. Open Unity Hub and add the desired sub-project (`ClaySim/`, `HandMouvements/`, `Main-scene/`)
3. Ensure your hardware is connected and drivers/software installed

---

## Running Projects

### ClaySim

* Open `ClaySim/` in Unity
* Open `Assets/Scenes/SampleScene.unity`
* Press `Play`
* Press `Space` to apply deformation or drop objects to impact the clay

### HandMouvements

* Open `HandMouvements/` in Unity
* Open `Assets/Scenes/Base.unity`
* Place hands over Leap Motion
* Make a **fist** or **swipe** left/right
* Check Unity Console for debug output

### Main-scene

* Open `Main-scene/` in Unity (Windows only)
* Open `Assets/Scenes/SampleScene.unity`
* Stand in front of Kinect
* Observe live skeleton tracking and debug UI
* Camera may follow head joint

---

## Key Scripts

### ClaySim

* `AdaptiveMeshDeformer`: Subdivides and deforms mesh
* `SimpleMeshDeformer`: Basic collision-based indentation
* `HybridSoftBodyDeformer`: Combines SkinnedMesh with mesh deformation

### HandMouvements

* `FistGestureDetector`: Logs fist gesture
* `SwipeGestureDetector`: Logs swipe left/right with hand identifier

### Main-scene

* `BodySourceManager`: Manages Kinect data stream
* `BodySourceView`: Visualizes tracked bodies and joints
* `DetectJoints`: Tracks specific joint and moves an object
* `lookatcamera`: Keeps objects facing the camera

---

## Integration Notes

* URP is used across projects – ensure URP is installed and active
* Leap and Kinect are desktop-only input systems – XR optional
* TMP is used for UI – import TMP Essentials if prompted
* Projects can be merged by importing Assets from one into another

---

## Extending the Project

* Combine gesture input with clay manipulation
* Use head tracking to shift camera POV dynamically
* Add UI elements or tools triggered by gestures

---

## License

This project is for educational and experimental use.

---

## Author

Created by [@nak0x](https://github.com/nak0x) & [@math-pixel](https://github.com/math-pixel)as part of the **Mundara** initiative.
