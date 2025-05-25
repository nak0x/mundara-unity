using UnityEngine;

public class OffAxisLookAtCamera : MonoBehaviour
{
    public Transform userPosition;    // Position de l'utilisateur (Kinect)
    public Transform screen;          // Transform de l'écran physique dans Unity
    public Camera virtualCamera;      // Caméra Unity

    // Dimensions physiques de l'écran en unités Unity
    public float screenWidth = 10f;   // 1m = 10 unités Unity selon votre échelle
    public float screenHeight = 5.6f; // Ajustez selon les proportions de votre écran

    public float leftPointOfset;
    public float rightPointOfset;
    public float bottomPointOfset;
    public float topPointOfset;

    void Update()
    {
        // Position de l'utilisateur par rapport à l'écran
        Vector3 localUserPosition = screen.InverseTransformPoint(userPosition.position);

        // Calculer la matrice de projection off-axis
        Matrix4x4 projection = CalculateOffAxisProjection(localUserPosition);

        // Appliquer la matrice de projection à la caméra
        virtualCamera.projectionMatrix = projection;

        // Positionner la caméra à la position de l'utilisateur
        virtualCamera.transform.position = userPosition.position;

        // Orienter la caméra vers l'écran
        virtualCamera.transform.LookAt(screen.position);
    }

    Matrix4x4 CalculateOffAxisProjection(Vector3 eyePos)
    {
        // Demi-dimensions de l'écran
        float halfWidth = screenWidth * 0.5f;
        float halfHeight = screenHeight * 0.5f;

        // Distance de l'œil à l'écran
        float eyeZ = eyePos.z;

        // Calcul des limites du frustum
        float left = ((-halfWidth - eyePos.x) * virtualCamera.nearClipPlane / eyeZ) + leftPointOfset;
        float right = ((halfWidth - eyePos.x) * virtualCamera.nearClipPlane / eyeZ) + rightPointOfset;
        float bottom = ((-halfHeight - eyePos.y) * virtualCamera.nearClipPlane / eyeZ) + bottomPointOfset;
        float top = ((halfHeight - eyePos.y) * virtualCamera.nearClipPlane / eyeZ)+ topPointOfset;

        // Créer la matrice de projection
        return Matrix4x4.Frustum(left, right, bottom, top, virtualCamera.nearClipPlane, virtualCamera.farClipPlane);
    }
}
