// AdaptiveMeshDerformer.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class AdaptiveClay : MonoBehaviour
{
    public float deformationRadius = 0.5f;
    public float maxEdgeLength = 0.3f;
    public float deformationStrength = 1f;

    public float a = -0.2f, b = 0f, c = 10.2f;
    public float softness = 5f;
    public float scale = 1f;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        // Clone the original mesh so we can modify it
        mesh = Instantiate(mesh);
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new List<Vector3>(mesh.vertices);
        triangles = new List<int>(mesh.triangles);
    }

    void UpdateMesh()
    {
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }


    public void DeformAtPoint(Vector3 worldPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

        List<int> affectedTriangles = new List<int>();

        for (int i = 0; i < triangles.Count; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            Vector3 centroid = (v0 + v1 + v2) / 3f;
            if ((centroid - localPoint).sqrMagnitude < deformationRadius * deformationRadius)
            {
                TrySubdivideTriangle(i);
                DeformVertices(i, localPoint);
            }
        }

        UpdateMesh();
    }

    void TrySubdivideTriangle(int index)
    {
        int i0 = triangles[index];
        int i1 = triangles[index + 1];
        int i2 = triangles[index + 2];

        Vector3 v0 = vertices[i0];
        Vector3 v1 = vertices[i1];
        Vector3 v2 = vertices[i2];

        float l01 = Vector3.Distance(v0, v1);
        float l12 = Vector3.Distance(v1, v2);
        float l20 = Vector3.Distance(v2, v0);

        if (l01 < maxEdgeLength && l12 < maxEdgeLength && l20 < maxEdgeLength)
            return;

        Vector3 m01 = (v0 + v1) * 0.5f;
        Vector3 m12 = (v1 + v2) * 0.5f;
        Vector3 m20 = (v2 + v0) * 0.5f;

        int m01Index = vertices.Count; vertices.Add(m01);
        int m12Index = vertices.Count; vertices.Add(m12);
        int m20Index = vertices.Count; vertices.Add(m20);

        triangles[index] = i0; triangles[index + 1] = m01Index; triangles[index + 2] = m20Index;
        triangles.Add(m01Index); triangles.Add(i1); triangles.Add(m12Index);
        triangles.Add(m12Index); triangles.Add(i2); triangles.Add(m20Index);
        triangles.Add(m01Index); triangles.Add(m12Index); triangles.Add(m20Index);
    }

    void DeformVertices(int triIndex, Vector3 contactPoint)
    {
        for (int j = 0; j < 3; j++)
        {
            int vi = triangles[triIndex + j];
            Vector3 v = vertices[vi];
            float d = Vector3.Distance(v, contactPoint);

            if (d < deformationRadius)
            {
                float depth = SmoothClampedParabola.Evaluate(
                    d, a, b, c, softness, scale);

                Vector3 dir = (v - contactPoint).normalized;
                vertices[vi] -= dir * depth * deformationStrength;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (vertices == null) return;
        Gizmos.color = Color.red;
        foreach (var v in vertices)
        {
            Gizmos.DrawSphere(transform.TransformPoint(v), 0.01f);
        }
    }
}