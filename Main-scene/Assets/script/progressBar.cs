using UnityEngine;
using TMPro;

public class progressBar : MonoBehaviour
{

    [SerializeField]
    public TMP_Text temporaryText;

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
        temporaryText.text = step.ToString();
    }
}
