using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class IntroductionManager : MonoBehaviour, ILeapMotionActionInterface
{
    
    // todo create custom class propertie and get a list of properties
    public int numberOfSteps = 5;
    
    // MundaraTitle
    public RectTransform MundaraTitle;
    public List<DataObject> MundaraTileData = new List<DataObject>();

    // subtitle
    public RectTransform Subtitle;
    public List<DataObject> SubtitleData = new List<DataObject>();
    
    // description text
    public RectTransform descriptionText;
    public List<DataObject> DescriptionData = new List<DataObject>();

    // swipeNumberText
    public RectTransform swipeNumberText;
    
    // HAnds
    public GameObject virtualHands;
    public GameObject reelHands;
    
    public float duration = 1f;
    
    private bool canSwipe = false;
    private int swipeCountTutorial = 0;
    private int currentStep = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateStep(currentStep);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right Key");
            currentStep++;
            UpdateStep(currentStep);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {            
            Debug.Log("Left Key");
            currentStep--;
            UpdateStep(currentStep);
        }
    }

    public void UpdateStep(int step)
    {
        Debug.Log("Update step : " + step);
        switch (step)
        {
            case 0:
                // skip auto to case "1" after x sec
                // come in the view
                MundaraTitle.DOAnchorPosY(MundaraTileData[currentStep].positionY, duration);
                MundaraTitle.DOScale(MundaraTileData[currentStep].size, duration);
                Subtitle.DOAnchorPosY(SubtitleData[currentStep].positionY, duration);
                virtualHands.SetActive(false);
                break;
            case 1:
                // skip auto to case "2"
                // display only the subtitle
                MundaraTitle.DOScale(MundaraTileData[currentStep].size, duration);
                Subtitle.DOAnchorPosY(SubtitleData[currentStep].positionY, duration);
                descriptionText.DOAnchorPosY(DescriptionData[currentStep].positionY, duration);
                descriptionText.gameObject.GetComponent<TMP_Text>().DOText("", duration);
                descriptionText.gameObject.GetComponent<TMP_Text>().DOColor(new Color(0,0,0,0), duration);
                reelHands.transform.DOMove(new Vector3(0.00899999961f,-0.395999998f,-0.63499999f), duration);
                break;
            case 2:
                // skip auto after hand detected and place on screen to case "3"
                // pull up the subtitle and display the description
                Subtitle.DOAnchorPosY(SubtitleData[currentStep].positionY, duration);
                descriptionText.gameObject.GetComponent<TMP_Text>().DOColor(Color.white, duration);
                descriptionText.DOAnchorPosY(DescriptionData[currentStep].positionY, duration);
                descriptionText.gameObject.GetComponent<TMP_Text>().DOText("Ouvrez votre main droite et bougez la devants le capteur", duration);
                
                // play hand
                reelHands.transform.DOMove(new Vector3(0.00899999961f,0.231999993f,-0.63499999f), duration);
                
                // hide virtual hand
                virtualHands.SetActive(false);
                swipeNumberText.gameObject.GetComponent<TMP_Text>().DOText("", duration);
                break;
            case 3:
                // todo hand URP auto change to outline and need to be ghost hand
                // feedback when the gesture is correcte
                // tutorial for help guys to swipe
                // skip when the swipe is detected 2 times
                descriptionText.gameObject.GetComponent<TMP_Text>().DOText("Voici le geste pour passez les differentes etapes. Essayer de le reproduire.", duration);
                virtualHands.SetActive(true);
                swipeNumberText.gameObject.GetComponent<TMP_Text>().DOText(swipeCountTutorial.ToString() + " / 3 Swipe réussis", duration);
                if (swipeCountTutorial == 3)
                {
                    currentStep++;
                    UpdateStep(currentStep);
                }
                
                break;
            
            case 4:
                swipeNumberText.gameObject.GetComponent<TMP_Text>().DOText("", duration);
                // texte or go to working scene
                break;
        }
    }

    public void SwipeLeft()
    {

        if (currentStep >= 4)
        {
            // next step
            currentStep++;
            UpdateStep(currentStep);
        }else if (currentStep == 3)
        {
            // tutorial for learn swipe
            swipeCountTutorial++;
            UpdateStep(3);
        }
        
    }

    public void SwipeRight()
    {
        if (currentStep >= 4)
        {
            // previous step
            currentStep--;
            UpdateStep(currentStep);
        }
        
    }
}

[System.Serializable]
public class DataObject
{
    public float positionY = 0;
    public float positionX = 0;
    public float width = 0;
    public float height = 0;
    public float opacity = 1;
    public Vector3 size = new Vector2(1, 1);
}
