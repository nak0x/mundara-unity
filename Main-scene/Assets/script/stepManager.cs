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
    
    private int _currentPanelStep = 0;
    private StateStep currentStateStep;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start step
        _currentGameStep = 0;
        gameObject.transform.position = workingPlace.gameObject.transform.position; 

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
            
            if (_currentGameStep + 1 < objectsOfSteps.Count)
            {
                NextStep();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_currentGameStep - 1 >= 0)
            {
                PreviousStep();
            }
        }
    }

    bool ObjectIsValid()
    {
        bool validateState = true;

        // if object is valid

        return validateState;
    }

    public void NextStep()
    {
        if (_currentGameStep + 1 < objectsOfSteps.Count)
        {
            _currentGameStep++;
            currentStateStep = StateStep.PannelStep;
            UpdateStep();
        }
    }

    public void PreviousStep()
    {
        if (_currentGameStep - 1 >= 0)
        {
            _currentGameStep--;
            UpdateStep();
        }
    }
    
    public void NextPannelStep()
    {
        if (_currentPanelStep + 1 < objectsOfSteps.Count)
        {
            _currentPanelStep++;
            UpdateStep();
        }
    }

    public void PreviousPannelStep()
    {
        if (_currentPanelStep - 1 >= 0)
        {
            _currentPanelStep--;
            UpdateStep();
        }
    }

    private void UpdateStep()
    {
        
        // check if need to display step pannel
        
        // if there is screens to display
        if (currentStateStep == StateStep.PannelStep)
        {
            if (StepScreenPresentations[_currentGameStep] != null)
            {
                // check last pannel and if the pannel exits
                if (_currentPanelStep < StepScreenPresentations[_currentGameStep].Count - 1 && StepScreenPresentations[_currentGameStep][_currentPanelStep] != null)
                {
                    // display pannel
                    ScreenPresentation currentData = StepScreenPresentations[_currentGameStep][_currentPanelStep];
                    presentationStepPanel.ShowPanel(currentData.title, currentData.description);
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
        }else if (currentStateStep == StateStep.WorkingStep)
        {
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
            currentStateStep = StateStep.PannelStep;
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
