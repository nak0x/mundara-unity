using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Narateur : MonoBehaviour
{

    public TMP_Text informationText;
    public TMP_Text tutorialText;
    public TMP_Text TitleText;
    public RectTransform background;
    public bool showBackground = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Say("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        informationText.text = "";
    }

    public void Say(string text, NarratorSay narratorSay = NarratorSay.BOTH)
    {
        switch (narratorSay)
        {
            case NarratorSay.INFORMATION:
                SayInformation(text);
                break;
            case NarratorSay.TUTORIAL:
                sayTutorial(text);
                break;
            case NarratorSay.BOTH:
                SayInformation(text);
                sayTutorial(text);
                break;
        }
    }

    private void SayInformation(string text)
    {
        // clear background if there is not text
        if(text == "")
        {
            ShowBackground(false);
            TitleText.DOText("", 1.0f);
        }
        else
        {
            if (showBackground == true)
            {
                TitleText.DOText("Pourquoi ca ?", 1.0f);
                ShowBackground(true);
            }
        }

        // display text
        informationText.DOText(text, 1f);
    }

    private void sayTutorial(string text)
    {
        tutorialText.DOText(text, 1f);
    }

    private void ShowBackground(bool show)
    {
        if (show)
        {
            background.transform.DOScale(new Vector3(1,1,1), 0.5f);
            // background.SetActive(true);
        }
        else
        {
            // background.SetActive(false);
            background.transform.DOScale(new Vector3(1,0,1), 0.5f);
        }
    }
}

public enum NarratorSay
{
    INFORMATION,
    TUTORIAL,
    BOTH
}