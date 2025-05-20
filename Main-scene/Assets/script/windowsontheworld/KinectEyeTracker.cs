using UnityEngine;
using Microsoft.Kinect;
using Windows.Kinect;

public class KinectEyeTracker : MonoBehaviour
{
    [Header("Kinect Configuration")]
    public Vector3 kinectPositionOffset = Vector3.zero;  // Position de la Kinect dans l'espace Unity
    public Vector3 kinectRotationOffset = Vector3.zero;  // Rotation de la Kinect dans l'espace Unity
    public float kinectToUnityScale = 1.0f;              // Échelle entre Kinect et Unity

    [Header("Head Tracking")]
    public float eyeVerticalOffset = 0.1f;               // Distance verticale entre tête et yeux
    public Transform headTransform;                      // Transform qui sera mise à jour

    // Variables Kinect
    private KinectSensor kinectSensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;
    private bool isInitialized = false;

    void Start()
    {
        InitializeKinect();
    }

    void InitializeKinect()
    {
        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor == null)
        {
            Debug.LogError("Aucun capteur Kinect détecté!");
            return;
        }

        bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();

        if (!kinectSensor.IsOpen)
        {
            kinectSensor.Open();
        }

        bodies = new Body[kinectSensor.BodyFrameSource.BodyCount];
        isInitialized = true;

        Debug.Log("Kinect initialisée avec succès");
    }

    void Update()
    {
        if (!isInitialized || bodyFrameReader == null) return;

        bool dataReceived = false;

        using (BodyFrame bodyFrame = bodyFrameReader.AcquireLatestFrame())
        {
            if (bodyFrame != null)
            {
                bodyFrame.GetAndRefreshBodyData(bodies);
                dataReceived = true;
            }
        }

        if (dataReceived)
        {
            ProcessBodyData();
        }
    }

    void ProcessBodyData()
    {
        foreach (Body body in bodies)
        {
            if (body.IsTracked)
            {
                // Obtenir la position de la tête
                CameraSpacePoint headPosition = body.Joints[JointType.Head].Position;

                // Calculer la position des yeux (légèrement en dessous de la tête)
                Vector3 eyePosition = new Vector3(
                    headPosition.X,
                    headPosition.Y - eyeVerticalOffset,
                    headPosition.Z
                );

                // Convertir en coordonnées Unity
                Vector3 unityPosition = ConvertKinectToUnitySpace(eyePosition);

                // Mettre à jour la position
                if (headTransform != null)
                {
                    headTransform.position = unityPosition;

                    // Optionnel: orienter la tête en fonction de l'orientation du corps
                    UpdateHeadOrientation(body);
                }

                // On ne traite que le premier corps détecté
                break;
            }
        }
    }

    Vector3 ConvertKinectToUnitySpace(Vector3 kinectPosition)
    {
        // Appliquer l'échelle
        Vector3 scaledPosition = kinectPosition * kinectToUnityScale;

        // Inverser Z pour Unity (Kinect Z pointe vers l'avant, Unity Z pointe vers l'avant)
        scaledPosition.z *= -1;

        // Appliquer la rotation de la Kinect
        Quaternion kinectRotation = Quaternion.Euler(kinectRotationOffset);
        scaledPosition = kinectRotation * scaledPosition;

        // Appliquer l'offset de position
        return scaledPosition + kinectPositionOffset;
    }

    void UpdateHeadOrientation(Body body)
    {
        // Cette méthode est optionnelle - elle permet d'orienter la tête
        // en fonction de l'orientation du corps détecté par la Kinect

        // Obtenir l'orientation des épaules
        Vector3 shoulderLeft = GetJointPosition(body, JointType.ShoulderLeft);
        Vector3 shoulderRight = GetJointPosition(body, JointType.ShoulderRight);
        Vector3 shoulderDirection = (shoulderRight - shoulderLeft).normalized;

        // Créer une rotation qui aligne l'axe X avec la direction des épaules
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        rotation *= Quaternion.FromToRotation(Vector3.right, shoulderDirection);

        // Appliquer la rotation
        headTransform.rotation = rotation;
    }

    Vector3 GetJointPosition(Body body, JointType jointType)
    {
        CameraSpacePoint position = body.Joints[jointType].Position;
        return ConvertKinectToUnitySpace(new Vector3(position.X, position.Y, position.Z));
    }

    void OnDestroy()
    {
        if (bodyFrameReader != null)
        {
            bodyFrameReader.Dispose();
            bodyFrameReader = null;
        }

        if (kinectSensor != null && kinectSensor.IsOpen)
        {
            kinectSensor.Close();
        }
    }
}