using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperienceManager : MonoBehaviour
{
    
    public static ExperienceManager instance;

    private void Awake()
    {
        // Si une instance existe déjà et que ce n'est pas celle-ci, on la détruit
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Sinon, on définit l'instance
        instance = this;

        // Optionnel : garder ce GameObject entre les scènes
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStateOfExperience(ExperienceState state)
    {
        switch (state)
        {
            case ExperienceState.INTRODUCTION:
                break;
            case ExperienceState.WORKING:
                SceneManager.LoadScene("main");
                break;
            case ExperienceState.OUTRODUCTION:
                Debug.Log("===== ENDINGGGGGGG =====");
                break;
        }
    }
    
}

public enum ExperienceState
{
    INTRODUCTION,
    WORKING,
    OUTRODUCTION,
}
