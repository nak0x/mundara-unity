using TMPro;
using UnityEngine;

public class Narateur : MonoBehaviour
{

    public TMP_Text narrateurText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        narrateurText.text = text;
    }
}
