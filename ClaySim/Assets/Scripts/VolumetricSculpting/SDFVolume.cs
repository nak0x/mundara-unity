// This script creates a 3D Signed Distance Field (SDF) texture using a compute shader.
using UnityEngine;

public class SDFVolume : MonoBehaviour
{
    public int resolution = 64;
    public ComputeShader sdfCompute;
    private RenderTexture sdfTexture;

    private int kernelInit, kernelAddSphere;

    void Start()
    {
        sdfTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RFloat);
        sdfTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        sdfTexture.volumeDepth = resolution;
        sdfTexture.enableRandomWrite = true;
        sdfTexture.wrapMode = TextureWrapMode.Clamp;
        sdfTexture.filterMode = FilterMode.Bilinear;
        sdfTexture.Create();

        kernelInit = sdfCompute.FindKernel("CSInit");
        kernelAddSphere = sdfCompute.FindKernel("CSAddSphere");

        sdfCompute.SetTexture(kernelInit, "Result", sdfTexture);
        sdfCompute.Dispatch(kernelInit, resolution / 8, resolution / 8, resolution / 8);

        // Add a sphere at the center
        sdfCompute.SetTexture(kernelAddSphere, "Result", sdfTexture);
        sdfCompute.SetVector("sphereCenter", new Vector3(0.5f, 0.5f, 0.5f));
        sdfCompute.SetFloat("sphereRadius", 0.25f);
        sdfCompute.Dispatch(kernelAddSphere, resolution / 8, resolution / 8, resolution / 8);
    }

    public RenderTexture GetTexture() => sdfTexture;
}
