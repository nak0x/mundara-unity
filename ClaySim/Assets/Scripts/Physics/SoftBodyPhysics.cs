using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
class SoftBodyPhysics : MonoBehaviour
{
    [Range(0.05f, 0.5f)]
    public float softness = 0.05f;

    [Range(0.7f, 1.1f)]
    public float damping = 0.8f;

    [Range(0.2f, 0.5f)]
    public float stiffness = 0.2f;

    public bool useGravity = true;

    [Range(0.01f, 0.05f)]
    public float selfCollisionDistance = 0.01f;

    [Range(0.6f, 1.0f)]
    public float friction = 0.6f;

    public SkinnedMeshRenderer skinnedMeshRenderer;

    private void Start()
    {
        CreateSoftBody();    
    }

    void CreateSoftBody()
    {
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer is not assigned.");
            return;
        }

        Cloth cloth = gameObject.AddComponent<Cloth>();
        cloth.useGravity = useGravity;
        cloth.damping = damping; // high = less bounce
        cloth.bendingStiffness = stiffness;
        cloth.stretchingStiffness = 0.1f; // low for softness
        cloth.friction = 0.8f;
        cloth.collisionMassScale = 1.5f;
        cloth.selfCollisionDistance = 0.03f;
        cloth.selfCollisionStiffness = 0.5f;


        cloth.coefficients = GenerateClothCoefficients(skinnedMeshRenderer.sharedMesh.vertices.Length);
    }

    private ClothSkinningCoefficient[] GenerateClothCoefficients(int vertexCount)
    {
        ClothSkinningCoefficient[] coefficients = new ClothSkinningCoefficient[vertexCount];
        Vector3[] vertices = skinnedMeshRenderer.sharedMesh.vertices;

        // Compute center of the mesh
        Vector3 center = Vector3.zero;
        for (int i = 0; i < vertexCount; i++)
            center += vertices[i];
        center /= vertexCount;

        // Compute maximum distance from center
        float maxDist = 0f;
        for (int i = 0; i < vertexCount; i++)
        {
            float dist = (vertices[i] - center).magnitude;
            if (dist > maxDist) maxDist = dist;
        }

        // Assign maxDistance per vertex: firm center (min), soft edges (max)
        for (int i = 0; i < vertexCount; i++)
        {
            float dist = (vertices[i] - center).magnitude;
            float t = dist / maxDist; // 0 (center) → 1 (edge)

            // Optionally ease: makes transition smoother
            t = Mathf.SmoothStep(0f, 1f, t);

            coefficients[i].maxDistance = Mathf.Lerp(0.01f, softness, t);
            coefficients[i].collisionSphereDistance = 0f;
        }

        return coefficients;
    }
}