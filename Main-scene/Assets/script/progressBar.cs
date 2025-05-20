using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class progressBar : MonoBehaviour
{

    [SerializeField]
    public TMP_Text temporaryText;

    public Image imageHolder;
    public List<Sprite> stepSprites;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateState(int step)
    {
        imageHolder.sprite = stepSprites[step];
    }
}
