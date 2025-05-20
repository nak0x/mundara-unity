using UnityEngine;
using UnityEngine.UI;

public class WindowCalibrationTool : MonoBehaviour
{
    [Header("References")]
    public WindowOnWorldRenderer windowRenderer;
    public KinectEyeTracker eyeTracker;

    [Header("UI Elements")]
    public Button calibrateButton;
    public Slider screenWidthSlider;
    public Slider screenHeightSlider;
    public Slider eyeOffsetXSlider;
    public Slider eyeOffsetYSlider;
    public Slider eyeOffsetZSlider;

    [Header("Calibration Target")]
    public GameObject calibrationTarget;
    public float targetDistance = 1.0f;

    void Start()
    {
        if (calibrateButton != null)
            calibrateButton.onClick.AddListener(CalibrateSystem);

        // Initialiser les sliders avec les valeurs actuelles
        InitializeSliders();
    }

    void InitializeSliders()
    {
        if (windowRenderer == null) return;

        if (screenWidthSlider != null)
        {
            screenWidthSlider.value = windowRenderer.screenWidthInMeters;
            screenWidthSlider.onValueChanged.AddListener(UpdateScreenWidth);
        }

        if (screenHeightSlider != null)
        {
            screenHeightSlider.value = windowRenderer.screenHeightInMeters;
            screenHeightSlider.onValueChanged.AddListener(UpdateScreenHeight);
        }

        if (eyeOffsetXSlider != null)
        {
            eyeOffsetXSlider.value = windowRenderer.eyePositionOffset.x;
            eyeOffsetXSlider.onValueChanged.AddListener(UpdateEyeOffsetX);
        }

        if (eyeOffsetYSlider != null)
        {
            eyeOffsetYSlider.value = windowRenderer.eyePositionOffset.y;
            eyeOffsetYSlider.onValueChanged.AddListener(UpdateEyeOffsetY);
        }

        if (eyeOffsetZSlider != null)
        {
            eyeOffsetZSlider.value = windowRenderer.eyePositionOffset.z;
            eyeOffsetZSlider.onValueChanged.AddListener(UpdateEyeOffsetZ);
        }
    }

    void CalibrateSystem()
    {
        if (windowRenderer == null) return;

        Debug.Log("D�marrage de la calibration...");

        // Positionner la cible de calibration si elle existe
        if (calibrationTarget != null && windowRenderer.screenTransform != null)
        {
            // Placer la cible � une distance sp�cifique derri�re l'�cran
            calibrationTarget.transform.position = windowRenderer.screenTransform.position
                - windowRenderer.screenTransform.forward * targetDistance;

            // Rendre la cible visible
            calibrationTarget.SetActive(true);
        }

        // Appeler la m�thode de calibration du renderer
        windowRenderer.CalibrateToCurrentEyePosition();

        // Mettre � jour les sliders avec les nouvelles valeurs
        UpdateSlidersFromCurrentValues();

        Debug.Log("Calibration termin�e!");
    }

    void UpdateSlidersFromCurrentValues()
    {
        if (windowRenderer == null) return;

        if (screenWidthSlider != null)
            screenWidthSlider.value = windowRenderer.screenWidthInMeters;

        if (screenHeightSlider != null)
            screenHeightSlider.value = windowRenderer.screenHeightInMeters;

        if (eyeOffsetXSlider != null)
            eyeOffsetXSlider.value = windowRenderer.eyePositionOffset.x;

        if (eyeOffsetYSlider != null)
            eyeOffsetYSlider.value = windowRenderer.eyePositionOffset.y;

        if (eyeOffsetZSlider != null)
            eyeOffsetZSlider.value = windowRenderer.eyePositionOffset.z;
    }

    // M�thodes de mise � jour des param�tres via les sliders
    void UpdateScreenWidth(float value)
    {
        if (windowRenderer != null)
        {
            windowRenderer.screenWidthInMeters = value;
            Debug.Log($"Largeur d'�cran mise � jour: {value} m�tres");
        }
    }

    void UpdateScreenHeight(float value)
    {
        if (windowRenderer != null)
        {
            windowRenderer.screenHeightInMeters = value;
            Debug.Log($"Hauteur d'�cran mise � jour: {value} m�tres");
        }
    }

    void UpdateEyeOffsetX(float value)
    {
        if (windowRenderer != null)
        {
            Vector3 offset = windowRenderer.eyePositionOffset;
            offset.x = value;
            windowRenderer.eyePositionOffset = offset;
            Debug.Log($"Offset X des yeux mis � jour: {value}");
        }
    }

    void UpdateEyeOffsetY(float value)
    {
        if (windowRenderer != null)
        {
            Vector3 offset = windowRenderer.eyePositionOffset;
            offset.y = value;
            windowRenderer.eyePositionOffset = offset;
            Debug.Log($"Offset Y des yeux mis � jour: {value}");
        }
    }

    void UpdateEyeOffsetZ(float value)
    {
        if (windowRenderer != null)
        {
            Vector3 offset = windowRenderer.eyePositionOffset;
            offset.z = value;
            windowRenderer.eyePositionOffset = offset;
            Debug.Log($"Offset Z des yeux mis � jour: {value}");
        }
    }

    // M�thode pour sauvegarder la configuration actuelle
    public void SaveConfiguration()
    {
        if (windowRenderer == null) return;

        PlayerPrefs.SetFloat("ScreenWidth", windowRenderer.screenWidthInMeters);
        PlayerPrefs.SetFloat("ScreenHeight", windowRenderer.screenHeightInMeters);
        PlayerPrefs.SetFloat("EyeOffsetX", windowRenderer.eyePositionOffset.x);
        PlayerPrefs.SetFloat("EyeOffsetY", windowRenderer.eyePositionOffset.y);
        PlayerPrefs.SetFloat("EyeOffsetZ", windowRenderer.eyePositionOffset.z);
        PlayerPrefs.Save();

        Debug.Log("Configuration sauvegard�e");
    }

    // M�thode pour charger la configuration
    public void LoadConfiguration()
    {
        if (windowRenderer == null) return;

        if (PlayerPrefs.HasKey("ScreenWidth"))
        {
            windowRenderer.screenWidthInMeters = PlayerPrefs.GetFloat("ScreenWidth");
            windowRenderer.screenHeightInMeters = PlayerPrefs.GetFloat("ScreenHeight");

            Vector3 offset = Vector3.zero;
            offset.x = PlayerPrefs.GetFloat("EyeOffsetX");
            offset.y = PlayerPrefs.GetFloat("EyeOffsetY");
            offset.z = PlayerPrefs.GetFloat("EyeOffsetZ");
            windowRenderer.eyePositionOffset = offset;

            // Mettre � jour les sliders
            UpdateSlidersFromCurrentValues();

            Debug.Log("Configuration charg�e");
        }
        else
        {
            Debug.Log("Aucune configuration sauvegard�e trouv�e");
        }
    }
}