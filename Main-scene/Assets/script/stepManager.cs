using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class StepManager : MonoBehaviour, ILeapMotionActionInterface
{
    
    [Header("Dépendance")]
    public GameObject workingPlace;
    public Narateur narateur;
    public progressBar progressBar;
    public PresentationPanel presentationStepPanel;
    public TMP_Text titleText;
    public AudioManager audioManager;
    
    [Header("Steps")]
    public int numberOfSteps;
    public List<GameObject> objectsOfSteps;
    public List<String> textsOfSteps;
    public List<String> titlesOfSteps;
    public List<AudioClip> audioOfSteps;
    public List<float> audioTimeoutActivations;
    
    [SerializedDictionary("Step to Display Pannel", "Panel Content")]
    public SerializedDictionary<int, SerializedDictionary<int, ScreenPresentation>> StepScreenPresentations;
    
    
    private GameObject _currentObject;
    private int _currentGameStep = 0;
    
    private int _currentPanelStep;
    private StateStep currentStateStep;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start step
        _currentGameStep = 0;
        gameObject.transform.position = workingPlace.gameObject.transform.position;

        _currentPanelStep = 0;
        
        // init STATE Step
        currentStateStep = StateStep.PannelStep;
        
        // init Step
        UpdateStep();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right Key");
            SwipeRight();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {            
            Debug.Log("Left Key");
            SwipeLeft();
        }
    }

    bool ObjectIsValid()
    {
        bool validateState = true;

        // if object is valid

        return validateState;
    }

    public void SwipeLeft()
    {
        Debug.Log("Swipe Left in STEP MANAGER in WORKING SCENE");
        if (currentStateStep == StateStep.PannelStep)
        {
            NextPannelStep();
        }else if (currentStateStep == StateStep.WorkingStep)
        {
            NextStep();
        }
    }

    public void SwipeRight()
    {
        if (currentStateStep == StateStep.PannelStep)
        {
            PreviousPannelStep();

        }else if (currentStateStep == StateStep.WorkingStep)
        {
            PreviousStep();
        }
    }

    void NextStep()
    {
        if (_currentGameStep + 1 < numberOfSteps)
        {
            // _currentPanelStep = 0;
            _currentGameStep++;
            
            // Start timer for the audio
            StopTimer();
            if (audioOfSteps[_currentGameStep] != null)
            {
                StartTimer(audioTimeoutActivations[_currentGameStep], PlayAudioOfSteps());
            }
            
            currentStateStep = StateStep.PannelStep;
            UpdateStep();
        }
        else
        {
            Debug.Log("STEPMANAGER switch to Ending by next step methods");
            ExperienceManager.instance.UpdateStateOfExperience(ExperienceState.OUTRODUCTION);
        }
    }

    void PreviousStep()
    {
        if (_currentGameStep - 1 >= 0)
        {
            // _currentPanelStep = 0;
            _currentGameStep--;
            currentStateStep = StateStep.PannelStep; // if you dont want the pannel information on back you can remove this 2 ligne 
            _currentPanelStep = StepScreenPresentations[_currentGameStep].Count - 1;
            UpdateStep();
        }
    }
    
    public void NextPannelStep()
    {
        _currentPanelStep++;
        UpdateStep();
    }

    public void PreviousPannelStep()
    {
        if (_currentPanelStep - 1 >= 0)
        {
            _currentPanelStep--;
            UpdateStep();
        }
        else
        {
            // change step
            // currentStateStep = StateStep.PannelStep; // if you dont want the pannel information on back you can remove this 2 ligne 
            PreviousStep();
        }
    }

    private void UpdateStep()
    {
        // check if need to display step pannel
        Debug.Log("State of the current updata is : "+ currentStateStep);
        Debug.Log("Current GAME STEP :" + _currentGameStep);
        Debug.Log("Current PANNEL STEP :" + _currentPanelStep);
        
        // if there is screens to display
        if (currentStateStep == StateStep.PannelStep)
        {
            if (_currentGameStep < StepScreenPresentations.Count && StepScreenPresentations[_currentGameStep] != null)
            {
                
                presentationStepPanel.ShowPanel();
                
                Debug.Log("there is pannel in step");
                // check last pannel and if the pannel exits in step
                if (_currentPanelStep < StepScreenPresentations[_currentGameStep].Count)
                {
                    Debug.Log("Show current pannel " + _currentPanelStep + " At the step "+ _currentGameStep);
                    
                    // check if is there is pannel in the list pannel
                    if (StepScreenPresentations[_currentGameStep][_currentPanelStep] != null)// 
                    {
                        // display pannel
                        ScreenPresentation currentData = StepScreenPresentations[_currentGameStep][_currentPanelStep];
                        presentationStepPanel.updatePanel(currentData.title, currentData.description);
                    }
                    else
                    {
                        // display working UI
                        currentStateStep = StateStep.WorkingStep;
                        UpdateStep();
                    }
                    
                }
                else
                {
                    // display working UI
                    currentStateStep = StateStep.WorkingStep;
                    UpdateStep();
                }
            }
            else
            {
                // display working UI
                currentStateStep = StateStep.WorkingStep;
                UpdateStep();
            }
        }
        else if (currentStateStep == StateStep.WorkingStep)
        {
            
            Debug.Log("Show working place");
            presentationStepPanel.hidePanel();
            
            // Display the next Object
            if (_currentObject != null)
            {
                Destroy(_currentObject);
            }
            if (objectsOfSteps[_currentGameStep] != null)
            {
                _currentObject = Instantiate(objectsOfSteps[_currentGameStep], gameObject.transform.position, Quaternion.identity);
                _currentObject.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);
            }
        
            // Update the progress bar
            progressBar.updateState(_currentGameStep);

            // Update narrator
            narateur.say(textsOfSteps[_currentGameStep]);
            
            // Update the title
            titleText.DOText(titlesOfSteps[_currentGameStep], 1.0f);
            
            // reset State to pannel for the next step
            // currentStateStep = StateStep.PannelStep;
            _currentPanelStep = 0;
            
        }
        else
        {
            // end Working time
            Debug.Log("STEPMANAGER switch to Ending");
            ExperienceManager.instance.UpdateStateOfExperience(ExperienceState.OUTRODUCTION);
        }
        
        
    }

    private Action PlayAudioOfSteps()
    {
        return delegate { audioManager.PlayAudio(audioOfSteps[_currentGameStep]); };
    }
    
    public void StartTimer(float seconds, System.Action onFinish)
    {
        StopTimer(); // Toujours s'assurer qu'un ancien est arrêté
        isStopped = false;
        timerCoroutine = StartCoroutine(Timer(seconds, onFinish));
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            isStopped = true;
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }
    
    private Coroutine timerCoroutine;
    private bool isStopped = false;
    private IEnumerator Timer(float seconds, System.Action onFinish)
    {
        float elapsed = 0f;

        while (elapsed < seconds)
        {
            if (isStopped)
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        onFinish?.Invoke();
        timerCoroutine = null;
    }
    
}

[Serializable]
public class ScreenPresentation
{
    public String title;
    public String description;
    public GameObject objectOfStep;
}

enum StateStep
{
    PannelStep,
    WorkingStep
}
