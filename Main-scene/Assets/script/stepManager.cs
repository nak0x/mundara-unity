using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine;

public class stepManager : MonoBehaviour
{
    
    public GameObject sceneOfObject;
    public Narateur narateur;
    public List<GameObject> listObjects;
    private GameObject currentObject;
    private int idObject = 0;

    public progressBar progressBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.position = sceneOfObject.gameObject.transform.position; 
        if (listObjects[idObject] != null)
        {
            currentObject = Instantiate(listObjects[0], gameObject.transform.position, Quaternion.identity);
            currentObject.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);
            progressBar.updateState(idObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            
            if (idObject + 1 < listObjects.Count)
            {
                updateStep(1);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (idObject - 1 >= 0)
            {
                updateStep(0);
            }
        }
    }

    bool objectIsValid()
    {
        bool validateState = true;

        // if object is valid

        return validateState;
    }

    private void updateStep(int direction)
    {
        // Set the direction of the next step
        if (direction == 0) {
            idObject--;
        }else if (direction == 1)
        {
            idObject++;
        }
        else
        {
            Debug.LogError("Error in the direction of the step is different to 0 or 1");
        }

        // Display the next Object
        if (currentObject != null)
        {
            Destroy(currentObject);
        }
        currentObject = Instantiate(listObjects[idObject], gameObject.transform.position, Quaternion.identity);
        currentObject.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);
        progressBar.updateState(idObject);


        // Do other action in function of the step
        switch (idObject)
        {
            case 1:
                narateur.say("Hello World");
                break;

        }
    }
}
