using UnityEngine;

public class lookatcamera : MonoBehaviour
{

    [SerializeField]
    public Transform target;
    public float smoothSpeed = 0.125f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
        // Lissage de la rotation pour un effet de "LookAt" plus fluide
        //Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
    }
}
