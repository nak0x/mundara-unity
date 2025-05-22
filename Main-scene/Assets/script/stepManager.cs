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
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start step
        _currentGameStep = 0;
        gameObject.transform.position = workingPlace.gameObject.transform.position; 

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

    private void UpdateStep()
    {
        
        // display presentation pannel
        if (StepScreenPresentations[_currentGameStep] != null)
        {
            ScreenPresentation currentData = StepScreenPresentations[_currentGameStep][0];
            presentationStepPanel.ShowPanel(currentData.title, currentData.description);
        }
        
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
        
    }
}

[System.Serializable]
public class ScreenPresentation
{
    public String title;
    public String description;
    public GameObject objectOfStep;
}
