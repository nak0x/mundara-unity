using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine;


public enum STEPDIRECTION
{
    Forward,
    Backward,
    Nothing
}

public class stepManager : MonoBehaviour
{
    
    public GameObject sceneOfObject;
    public Narateur narateur;
    public List<GameObject> stepObjects;
    private GameObject currentObject;
    private int currentGameStep = 0;

    public progressBar progressBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start step
        currentGameStep = 0;
        gameObject.transform.position = sceneOfObject.gameObject.transform.position; 

        updateStep(STEPDIRECTION.Nothing);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            
            if (currentGameStep + 1 < stepObjects.Count)
            {
                updateStep(STEPDIRECTION.Forward);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentGameStep - 1 >= 0)
            {
                updateStep(STEPDIRECTION.Backward);
            }
        }
    }

    bool objectIsValid()
    {
        bool validateState = true;

        // if object is valid

        return validateState;
    }

    public void updateStep(STEPDIRECTION direction)
    {
        // Set the direction of the next step
        if (direction == STEPDIRECTION.Backward) {
            currentGameStep--;
        }else if (direction == STEPDIRECTION.Forward)
        {
            currentGameStep++;
        }else if(direction == STEPDIRECTION.Nothing){

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

        if (stepObjects[currentGameStep] != null)
        {
            currentObject = Instantiate(stepObjects[currentGameStep], gameObject.transform.position, Quaternion.identity);
            currentObject.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);

        }
        
        // Update the progress bar
        progressBar.updateState(currentGameStep);


        // Do other action in function of the step
        switch (currentGameStep)
        {
            case 0:
                narateur.say("La modélisation du petit pot commence par la réalisation d’une boule de terre bien homogène. Cette première étape est essentielle pour assurer une base régulière et malléable.");
                break;
            case 1:
                narateur.say("Tracer une ligne au centre de la boule permettait non seulement de mieux tenir les pots pendant la fabrication, mais aussi de faciliter leur suspension ou leur transport une fois terminés.");
                break;
            case 2:
                narateur.say("À cette étape, on détermine le volume que le pot devra contenir. Cela implique de creuser et d’élargir l’intérieur de manière régulière pour assurer à la fois la capacité et la stabilité.");
                break;
            case 3:
                narateur.say("Pour faciliter le service des liquides, il était courant de façonner un petit bec verseur. Ce détail rendait le pot plus fonctionnel tout en lui donnant une touche esthétique.");
                break;
            case 4:
                narateur.say("En aplatissant soigneusement le fond du pot, on lui assurait une bonne stabilité une fois posé. Cette étape est cruciale pour que le pot tienne debout de manière sûre et durable.");
                break;
            case 5:
                narateur.say("");
                break;

        }
    }
}
