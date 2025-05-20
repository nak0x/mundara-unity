using System.Collections.Generic;
using UnityEngine;

public class debugLine : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(gameObject.transform.position, Vector3.forward * 10);
    }
}
