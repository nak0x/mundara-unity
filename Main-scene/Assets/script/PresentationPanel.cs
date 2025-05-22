using System;
using UnityEngine;
using TMPro;

public class PresentationPanel : MonoBehaviour
{
    // todo set _ to pivate methods
    
    
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPanel(String title, String description)
    {
        gameObject.SetActive(true);
        titleText.text = title;
        animationTitleIn();
        descriptionText.text = description;
        animationDescriptionIn();
    }
    
    void hidePanel()
    {
        animationTitleOut();
        animationDescriptionOut();
        gameObject.SetActive(false);
    }

    void updatePanel(String title = null, String description = null)
    {
        if (title != null)
        {
            changeTitle(title);
        }

        if (description != null)
        {
            changeDescription(description);
        }
    }

    void changeTitle(String title)
    {
        animationTitleOut();
        titleText.text = title;
        animationTitleIn();
    }

    void changeDescription(String description)
    {
        animationDescriptionOut();
        descriptionText.text = description;
        animationDescriptionIn();
    }

    void animationTitleIn()
    {
        // 700 out - 255 in
    }
    
    void animationTitleOut()
    {
        // 700 out - 255 in
    }

    void animationDescriptionIn()
    {
        
    }

    void animationDescriptionOut()
    {
        
    }
}
