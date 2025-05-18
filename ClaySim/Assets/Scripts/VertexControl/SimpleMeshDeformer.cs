using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(Collider))]
public class SimpleMeshDeformer : MonoBehaviour
{
    [Header("Deformation Settings")]
    public float deformationRadius = 0.5f;
    public float deformationStrength = 0.1f;
    public bool debug = true;
    public float colliderUpdateDelay = 0.2f;

    [Header("Smooth Falloff Settings")]
    public float curveSharpness = -1f;
    public float curveSkew = 0f;
    public float curvePeak = 1f;
    public float clampSoftness = 4f;
    public float intensityScale = 1f;


    private Mesh deformingMesh;
    private Vector3[] originalVertices, modifiedVertices;
    private float lastDeformTime = -Mathf.Infinity;
    private bool pendingColliderUpdate = false;


    void Start()
    {
        if (debug) {
            Debug.Log("SimpleMeshDeformer started on: " + gameObject.name);
        }
        if (GetComponent<MeshFilter>() == null) {
            Debug.LogError("SimpleMeshDeformer requires a MeshFilter component.");
        }
        deformingMesh = GetComponent<MeshFilter>().mesh;
        deformingMesh = Instantiate(deformingMesh); // Make mesh editable
        GetComponent<MeshFilter>().mesh = deformingMesh;

        originalVertices = deformingMesh.vertices;
        modifiedVertices = deformingMesh.vertices;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (debug) {
            Debug.Log("Collision detected with: " + collision.gameObject.name);
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            if (debug) {
                Debug.DrawRay(contact.point, contact.normal, Color.red, 2f);
            }
            ApplyDeformation(contact.point, contact.normal, collision.relativeVelocity);
        }

        deformingMesh.vertices = modifiedVertices;
        deformingMesh.RecalculateNormals();
        deformingMesh.RecalculateBounds();

        // Defer the collider update
        lastDeformTime = Time.time;
        pendingColliderUpdate = true;

        // Update the collider 
        MeshCollider col = GetComponent<MeshCollider>();
        if (col != null)
        {
            col.sharedMesh = null;
            col.sharedMesh = deformingMesh;
        }

    }

    void LateUpdate()
    {
        if (pendingColliderUpdate && Time.time - lastDeformTime > colliderUpdateDelay)
        {
            MeshCollider col = GetComponent<MeshCollider>();
            if (col != null)
            {
                col.sharedMesh = null;
                col.sharedMesh = deformingMesh;
            }

            pendingColliderUpdate = false;
        }
    }



    void ApplyDeformation(Vector3 worldPoint, Vector3 normal, Vector3 velocity)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        if (debug) {
            Debug.DrawRay(worldPoint, velocity.normalized * 0.5f, Color.cyan, 1f);
            Debug.DrawRay(worldPoint, normal * 0.5f, Color.yellow, 1f);
        }

        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            Vector3 vertex = modifiedVertices[i];
            float distance = Vector3.Distance(vertex, localPoint);

            if (distance < deformationRadius)
            {

                if (distance > deformationRadius)
                    continue;

                float falloff = SmoothClampedParabola.Evaluate(
                    distance,
                    curveSharpness,
                    curveSkew,
                    curvePeak,
                    clampSoftness,
                    intensityScale
                );

                modifiedVertices[i] += localVelocity.normalized * deformationStrength * falloff;
            }
        }
    }


    void OnDrawGizmos()
    {
        if (!Application.isPlaying || deformingMesh == null || !debug)
            return;

        Gizmos.color = Color.green;

        Vector3[] verts = deformingMesh.vertices;
        int[] tris = deformingMesh.triangles;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 a = transform.TransformPoint(verts[tris[i]]);
            Vector3 b = transform.TransformPoint(verts[tris[i + 1]]);
            Vector3 c = transform.TransformPoint(verts[tris[i + 2]]);

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, a);
        }
    }

}
