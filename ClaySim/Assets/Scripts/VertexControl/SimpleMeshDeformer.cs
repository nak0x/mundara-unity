using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(Collider))]
public class SimpleMeshDeformer : MonoBehaviour
{
    [Header("Required Components")]
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    [Header("Deformation Settings")]
    public float deformationRadius = 0.5f;
    public float deformationStrength = 0.1f;
    public bool debug = true;
    public float colliderUpdateDelay = 0.2f;
    
    [Header("Deformation Constraints")]
    public float maxDeformation = 0.2f;

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
        deformingMesh = meshFilter.mesh;
        deformingMesh = Instantiate(deformingMesh); // Make mesh editable
        meshFilter.mesh = deformingMesh;

        originalVertices = deformingMesh.vertices;
        modifiedVertices = (Vector3[])originalVertices.Clone();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (debug) {
            Debug.Log("Collision detected with: " + collision.gameObject.name);
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            if (debug) {
                Debug.DrawRay(contact.point, contact.normal, Color.red, 4f);
            }
            ApplyDeformation(contact.point, contact.normal, collision.relativeVelocity);
        }

        // Update the mesh with the modified vertices
        deformingMesh.vertices = modifiedVertices;
        deformingMesh.RecalculateNormals();
        deformingMesh.RecalculateBounds();

        // Defer the collider update
        lastDeformTime = Time.time;
        pendingColliderUpdate = true;

        // Update the collider 
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = deformingMesh;
        }

    }

    void LateUpdate()
    {
        if (pendingColliderUpdate && Time.time - lastDeformTime > colliderUpdateDelay)
        {
            if (meshCollider != null)
            {
                // Force update mesh
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = deformingMesh;
            }

            pendingColliderUpdate = false;
        }
    }

    void ApplyDeformation(Vector3 worldPoint, Vector3 normal, Vector3 velocity)
    {
        Vector3 meshCenter = Vector3.zero;
        foreach (var v in modifiedVertices)
            meshCenter += v;

        // Compute mesh centroid
        meshCenter /= modifiedVertices.Length;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity).normalized;
        if (debug) {
            Debug.DrawRay(worldPoint, localVelocity * 0.5f, Color.cyan, 1f);
            Debug.DrawRay(worldPoint, normal * 0.5f, Color.yellow, 1f);
        }

        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            Vector3 vertex = modifiedVertices[i];
            float distance = Vector3.Distance(vertex, worldPoint);

            if (distance < deformationRadius)
            {
                float falloff = SmoothClampedParabola.Evaluate(
                    distance,
                    curveSharpness,
                    curveSkew,
                    curvePeak,
                    clampSoftness,
                    intensityScale
                );

                // Normal displacement
                Vector3 push = localVelocity * deformationStrength * falloff;

                // Add mirrored defromation
                Vector3 toCenter = (vertex - meshCenter).normalized;
                Vector3 antiPush = toCenter * deformationStrength * 0.5f * falloff;

                Vector3 final = vertex + push + antiPush;

                // Clamp deformation to prevent over-stretching
                float maxDisplacement = maxDeformation;
                float delta = (final - originalVertices[i]).magnitude;

                if (delta > maxDisplacement)
                {
                    final = originalVertices[i] + (final - originalVertices[i]).normalized * maxDisplacement;
                }

                modifiedVertices[i] = final;
                if (debug) {
                    Debug.DrawRay(vertex, final - vertex, Color.green, 1f);
                }

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
