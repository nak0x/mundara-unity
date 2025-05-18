using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class HybridSoftBodyDeformer : MonoBehaviour
{
    [Header("References")]
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public MeshFilter outputFilter;
    public MeshCollider meshCollider;

    [Header("Deformation")]
    public float deformationRadius = 0.5f;
    public float deformationStrength = 0.2f;

    [Header("Influence Curve")]
    public float curveSharpness = -1f;
    public float curveSkew = 0f;
    public float curvePeak = 1f;
    public float clampSoftness = 4f;
    public float intensityScale = 1f;

    [Header("Bidirectional Deformation")]
    public float mirrorPushFactor = 0.5f;

    [Header("Deformation Constraints")]
    public float maxDeformationDistance = 0.2f;

    private Mesh deformMesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;

    void Start()
    {
        if (!skinnedMeshRenderer || !outputFilter)
        {
            Debug.LogError("Missing skinnedMeshRenderer or outputFilter.");
            return;
        }

        deformMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(deformMesh);
        outputFilter.mesh = Instantiate(deformMesh);

        originalVertices = outputFilter.mesh.vertices;
        modifiedVertices = (Vector3[])originalVertices.Clone();

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = outputFilter.mesh;
        }

        // Optional: hide skinned mesh visuals
        skinnedMeshRenderer.enabled = true;
    }

    void LateUpdate()
    {
        Debug.Log("Pushing updated vertices into deformMesh");

        if (deformMesh == null) return;

        // Re-bake the cloth state into our deformable mesh (optional)
        skinnedMeshRenderer.BakeMesh(deformMesh);

        // Copy current vertex state
        deformMesh.vertices.CopyTo(modifiedVertices, 0);

        // Apply any additional deformation here (optional or triggered externally)

        // Push back to mesh
        deformMesh.vertices = modifiedVertices;
        deformMesh.RecalculateNormals();

        // Sync collider if needed
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = deformMesh;
        }
    }


    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            ApplyDeformation(contact.point, collision.relativeVelocity);
        }
    }

    void ApplyDeformation(Vector3 worldPoint, Vector3 impactVelocity)
    {
        Vector3 localPoint = outputFilter.transform.InverseTransformPoint(worldPoint);
        Vector3 localVelocity = outputFilter.transform.InverseTransformDirection(impactVelocity).normalized;
        Debug.DrawRay(worldPoint, impactVelocity.normalized * 0.3f, Color.red, 1f);
        Debug.Log("Deformation applied at: " + worldPoint);


        // Compute mesh centroid
        Vector3 meshCenter = Vector3.zero;
        for (int i = 0; i < modifiedVertices.Length; i++)
            meshCenter += modifiedVertices[i];
        meshCenter /= modifiedVertices.Length;

        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            Vector3 vertex = modifiedVertices[i];
            float distance = Vector3.Distance(vertex, localPoint);

            if (distance < deformationRadius)
            {
                float falloff = SmoothClampedParabola.Evaluate(
                    distance, curveSharpness, curveSkew, curvePeak, clampSoftness, intensityScale
                );

                // Main deformation
                Vector3 push = localVelocity * deformationStrength * falloff;

                // Mirrored push (based on vertex's direction from mesh center)
                Vector3 toEdge = (vertex - meshCenter).normalized;
                Vector3 mirroredPush = toEdge * deformationStrength * falloff * mirrorPushFactor;

                Vector3 proposed = vertex + push + mirroredPush;

                // Clamp deformation to prevent over-stretching
                Vector3 original = originalVertices[i];
                float delta = (proposed - original).magnitude;
                if (delta > maxDeformationDistance)
                {
                    proposed = original + (proposed - original).normalized * maxDeformationDistance;
                }

                modifiedVertices[i] = proposed;
            }
        }
    }

}
