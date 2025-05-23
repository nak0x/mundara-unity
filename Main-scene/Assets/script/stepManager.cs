using System;
using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    
    public GameObject workingPlace;
    public Narateur narateur;
    
    public int numberOfSteps;
    public List<GameObject> objectsOfSteps;
    public List<String> textsOfSteps;
    
    public progressBar progressBar;
    
    public PresentationPanel presentationStepPanel;

    [SerializedDictionary("Steps", "Panel Content")]
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
            currentStateStep = StateStep.PannelStep;
            UpdateStep();
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
        if (_currentPanelStep + 1 < numberOfSteps)
        {
            _currentPanelStep++;
            UpdateStep();
        }
        else
        {
            Debug.Log("cant next pannel");
        }
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
            
            // reset State to pannel for the next step
            // currentStateStep = StateStep.PannelStep;
            _currentPanelStep = 0;
        }
        
        
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
