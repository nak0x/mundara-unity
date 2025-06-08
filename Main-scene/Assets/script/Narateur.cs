using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Narateur : MonoBehaviour
{

    public TMP_Text narrateurText;
    public TMP_Text narrateurNameText;
    public RectTransform background;
    public bool showBackground = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        say("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clear()
    {
        narrateurText.text = "";
    }

    public void say(string text)
    {
        
        // clear background if there is not text
        if(text == "")
        {
            ShowBackground(false);
            narrateurNameText.DOText("", 1.0f);
        }
        else
        {
            if (showBackground == true)
            {
                narrateurNameText.DOText("Pourquoi ca ?", 1.0f);
                ShowBackground(true);
            }
        }

        // display text
        narrateurText.DOText(text, 1f);
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
