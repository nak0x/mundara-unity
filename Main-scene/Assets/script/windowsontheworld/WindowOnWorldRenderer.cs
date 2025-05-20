using UnityEngine;

public class WindowOnWorldRenderer : MonoBehaviour
{
    [Header("Configuration")]
    public Transform eyeTracker;                // Position des yeux (depuis Kinect)
    public Transform screenTransform;           // Transform de l'écran transparent
    public float screenWidthInMeters = 0.5f;    // Largeur physique de l'écran en mètres
    public float screenHeightInMeters = 0.3f;   // Hauteur physique de l'écran en mètres

    [Header("Calibration")]
    public Vector3 eyePositionOffset = Vector3.zero;  // Ajustement fin de la position des yeux
    public bool visualizeScreen = true;               // Afficher un cadre autour de l'écran pour debug

    [Header("Performance")]
    [Range(0f, 1f)]
    public float smoothing = 0.2f;              // Lissage des mouvements (0 = pas de lissage)

    // Variables privées
    private Camera cam;
    private Vector3 smoothedEyePosition;
    private GameObject screenVisualizer;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Initialiser la position lissée
        if (eyeTracker != null)
            smoothedEyePosition = eyeTracker.position;

        // Créer un visualiseur d'écran pour le débogage
        if (visualizeScreen)
            CreateScreenVisualizer();
    }

    void LateUpdate()
    {
        if (eyeTracker == null || screenTransform == null || cam == null)
            return;

        // Obtenir la position actuelle des yeux avec offset de calibration
        Vector3 currentEyePosition = eyeTracker.position + eyePositionOffset;

        // Appliquer le lissage
        smoothedEyePosition = Vector3.Lerp(smoothedEyePosition, currentEyePosition, 1f - smoothing);

        // Mettre à jour la caméra pour qu'elle suive exactement la position des yeux
        UpdateCameraTransform(smoothedEyePosition);

        // Calculer et appliquer la matrice de projection correcte
        UpdateProjectionMatrix(smoothedEyePosition);

        // Mettre à jour le visualiseur d'écran si activé
        if (visualizeScreen && screenVisualizer != null)
            UpdateScreenVisualizer();
    }

    void UpdateCameraTransform(Vector3 eyePosition)
    {
        // Positionner la caméra exactement à la position des yeux
        transform.position = eyePosition;

        // Orienter la caméra vers le centre de l'écran
        transform.LookAt(screenTransform.position);
    }

    void UpdateProjectionMatrix(Vector3 eyePosition)
    {
        // 1. Définir les coins de l'écran dans l'espace monde
        Vector3 screenCenter = screenTransform.position;
        Vector3 screenRight = screenTransform.right * (screenWidthInMeters * 0.5f);
        Vector3 screenUp = screenTransform.up * (screenHeightInMeters * 0.5f);

        Vector3 bottomLeft = screenCenter - screenRight - screenUp;
        Vector3 bottomRight = screenCenter + screenRight - screenUp;
        Vector3 topLeft = screenCenter - screenRight + screenUp;
        Vector3 topRight = screenCenter + screenRight + screenUp;

        // 2. Convertir les coins en espace local de la caméra
        Vector3 localBottomLeft = transform.InverseTransformPoint(bottomLeft);
        Vector3 localBottomRight = transform.InverseTransformPoint(bottomRight);
        Vector3 localTopLeft = transform.InverseTransformPoint(topLeft);
        Vector3 localTopRight = transform.InverseTransformPoint(topRight);

        // 3. Vérifier que l'écran est devant la caméra
        if (localBottomLeft.z >= 0 || localBottomRight.z >= 0 ||
            localTopLeft.z >= 0 || localTopRight.z >= 0)
        {
            Debug.LogWarning("L'écran est derrière ou intersecte la caméra!");
            return;
        }

        // 4. Calculer les paramètres du frustum
        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;

        // Calculer les limites du frustum au near plane
        float scale = near / Mathf.Abs(localBottomLeft.z);
        float left = localBottomLeft.x * scale;
        float right = localBottomRight.x * scale;
        float bottom = localBottomLeft.y * scale;
        float top = localTopLeft.y * scale;

        // 5. Créer et appliquer la matrice de projection asymétrique
        Matrix4x4 projMatrix = Matrix4x4.Frustum(left, right, bottom, top, near, far);
        cam.projectionMatrix = projMatrix;
    }

    void CreateScreenVisualizer()
    {
        screenVisualizer = new GameObject("ScreenVisualizer");
        screenVisualizer.transform.parent = screenTransform;
        screenVisualizer.transform.localPosition = Vector3.zero;
        screenVisualizer.transform.localRotation = Quaternion.identity;

        LineRenderer lineRenderer = screenVisualizer.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 5;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
    }

    void UpdateScreenVisualizer()
    {
        LineRenderer lineRenderer = screenVisualizer.GetComponent<LineRenderer>();
        if (lineRenderer == null) return;

        Vector3 screenCenter = screenTransform.position;
        Vector3 screenRight = screenTransform.right * (screenWidthInMeters * 0.5f);
        Vector3 screenUp = screenTransform.up * (screenHeightInMeters * 0.5f);

        Vector3 bottomLeft = screenCenter - screenRight - screenUp;
        Vector3 bottomRight = screenCenter + screenRight - screenUp;
        Vector3 topRight = screenCenter + screenRight + screenUp;
        Vector3 topLeft = screenCenter - screenRight + screenUp;

        lineRenderer.SetPosition(0, bottomLeft);
        lineRenderer.SetPosition(1, bottomRight);
        lineRenderer.SetPosition(2, topRight);
        lineRenderer.SetPosition(3, topLeft);
        lineRenderer.SetPosition(4, bottomLeft);
    }

    // Méthode utilitaire pour calibrer le système
    public void CalibrateToCurrentEyePosition()
    {
        if (eyeTracker == null) return;

        // Enregistrer la position actuelle comme référence
        eyePositionOffset = Vector3.zero; // Réinitialiser d'abord

        // Vous pouvez ajouter ici une logique de calibration plus complexe
        Debug.Log("Système calibré à la position actuelle des yeux");
    }
}