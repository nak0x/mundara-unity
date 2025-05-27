using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IntroductionManager : MonoBehaviour
{
    
    // todo create custom class propertie and get a list of properties
    
    // MundaraTitle
    public RectTransform MundaraTitle;
    public List<float> mundaraPositions;
    public List<float> mundaraSize;

    // subtitle
    public RectTransform Subtitle;
    public List<float> subtitlePositionY;
    public List<float> subtitleSize;
    
    public float duration = 1f;
    
    
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
        switch (step)
        {
            case 0:
                MundaraTitle.DOAnchorPosY(mundaraPositions[currentStep], duration);
                Subtitle.DOAnchorPosY(subtitlePositionY[currentStep], duration);
                break;
            case 1:
                MundaraTitle.DOAnchorPosY(mundaraPositions[currentStep], duration);
                Subtitle.DOAnchorPosY(subtitlePositionY[currentStep], duration);
                break;
            case 2:
                Subtitle.DOAnchorPosY(subtitlePositionY[currentStep], duration);
                break;
        }
    }
}
