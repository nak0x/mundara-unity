using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Narateur : MonoBehaviour
{

    public TMP_Text narrateurText;

    public GameObject background;
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
            background.SetActive(false);
        }
        else
        {
            if (showBackground == true)
            {
                background.SetActive(true);
            }
        }

        // display text
        narrateurText.text = text;
    }
}
