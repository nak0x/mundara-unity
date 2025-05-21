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
                narateur.say("La mod�lisation du petit pot commence par la r�alisation d�une boule de terre bien homog�ne. Cette premi�re �tape est essentielle pour assurer une base r�guli�re et mall�able.");
                break;
            case 1:
                narateur.say("Tracer une ligne au centre de la boule permettait non seulement de mieux tenir les pots pendant la fabrication, mais aussi de faciliter leur suspension ou leur transport une fois termin�s.");
                break;
            case 2:
                narateur.say("� cette �tape, on d�termine le volume que le pot devra contenir. Cela implique de creuser et d��largir l�int�rieur de mani�re r�guli�re pour assurer � la fois la capacit� et la stabilit�.");
                break;
            case 3:
                narateur.say("Pour faciliter le service des liquides, il �tait courant de fa�onner un petit bec verseur. Ce d�tail rendait le pot plus fonctionnel tout en lui donnant une touche esth�tique.");
                break;
            case 4:
                narateur.say("En aplatissant soigneusement le fond du pot, on lui assurait une bonne stabilit� une fois pos�. Cette �tape est cruciale pour que le pot tienne debout de mani�re s�re et durable.");
                break;
            case 5:
                narateur.say("");
                break;

        }
    }
}
