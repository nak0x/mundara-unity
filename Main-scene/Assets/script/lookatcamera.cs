using UnityEngine;

public class lookatcamera : MonoBehaviour
{

    [SerializeField]
    public Transform target;
    Vector3 finalTarget;
    public float smoothSpeed = 0.125f;

    [Range(0.000f, 10.000f)]
    public float translateY = 0f; // save 1.91f

    [Range(-10.000f, 10.000f)]
    public float translateX = 0f;

    [Range(-10.000f, 10.000f)]
    public float translateZ = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.LookAt(target.transform);
    }

    // Update is called once per frame
    void Update()
    {
        finalTarget = target.position + new Vector3(translateX, translateY, translateZ);//position.z = finalTarget.position.z + translateZ;
        transform.LookAt(finalTarget, Vector3.up);
        // Lissage de la rotation pour un effet de "LookAt" plus fluide
        //Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        // Définir la couleur du Gizmo
        Gizmos.color = Color.blue;

        // Dessiner la sphère à la position spécifiée
        Gizmos.DrawSphere(finalTarget, 0.2f);

    }

}
