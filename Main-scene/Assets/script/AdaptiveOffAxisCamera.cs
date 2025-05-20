//using UnityEngine;

//public class AdaptiveOffAxisCamera : MonoBehaviour
//{
//    public Transform eyeTracker; // Position des yeux de l'utilisateur (depuis Kinect)
//    public Transform screenTransform; // Transform de votre écran transparent
//    public float screenWidth = 0.5f; // Largeur physique de l'écran en mètres
//    public float screenHeight = 0.3f; // Hauteur physique de l'écran en mètres
//    public float nearClipPlane = 0.01f;
//    public float farClipPlane = 1000f;

//    private Camera cam;

//    void Start()
//    {
//        cam = GetComponent<Camera>();
//        cam.nearClipPlane = nearClipPlane;
//        cam.farClipPlane = farClipPlane;
//    }

//    void LateUpdate()
//    {
//        // Mettre à jour la projection à chaque frame
//        UpdateOffAxisProjection();
//    }

//    void UpdateOffAxisProjection()
//    {
//        // Position des yeux dans l'espace monde
//        Vector3 eyePosition = eyeTracker.position;

//        // IMPORTANT: Ne déplacez PAS la caméra à la position des yeux
//        // transform.position = eyePosition; // SUPPRIMER CETTE LIGNE

//        // Gardez la caméra à une position fixe, mais utilisez la position des yeux
//        // uniquement pour calculer la matrice de projection

//        // Définir le plan de l'écran
//        Plane screenPlane = new Plane(screenTransform.forward, screenTransform.position);

//        // Calculer les coins de l'écran dans l'espace monde
//        Vector3 screenCenter = screenTransform.position;
//        Vector3 screenRight = screenTransform.right * (screenWidth * 0.5f);
//        Vector3 screenUp = screenTransform.up * (screenHeight * 0.5f);

//        Vector3 bottomLeft = screenCenter - screenRight - screenUp;
//        Vector3 bottomRight = screenCenter + screenRight - screenUp;
//        Vector3 topLeft = screenCenter - screenRight + screenUp;
//        Vector3 topRight = screenCenter + screenRight + screenUp;

//        // Calculer les vecteurs de l'œil aux coins de l'écran
//        Vector3 eyeToBottomLeft = bottomLeft - eyePosition;
//        Vector3 eyeToBottomRight = bottomRight - eyePosition;
//        Vector3 eyeToTopLeft = topLeft - eyePosition;
//        Vector3 eyeToTopRight = topRight - eyePosition;

//        // Calculer l'intersection de ces vecteurs avec le near plane de la caméra
//        float near = cam.nearClipPlane;
//        float far = cam.farClipPlane;

//        // Calculer la distance de l'œil au plan de l'écran
//        float eyeToScreenDistance = screenPlane.GetDistanceToPoint(eyePosition);

//        // Calculer les coordonnées sur le near plane
//        float scale = near / eyeToScreenDistance;

//        // Calculer les limites du frustum
//        float left = Vector3.Dot(eyeToBottomLeft, -transform.right) * scale;
//        float right = Vector3.Dot(eyeToBottomRight, transform.right) * scale;
//        float bottom = Vector3.Dot(eyeToBottomLeft, -transform.up) * scale;
//        float top = Vector3.Dot(eyeToTopLeft, transform.up) * scale;

//        // Créer et appliquer la matrice de projection asymétrique
//        Matrix4x4 projMatrix = Matrix4x4.Frustum(left, right, bottom, top, near, far);
//        cam.projectionMatrix = projMatrix;
//    }
//}