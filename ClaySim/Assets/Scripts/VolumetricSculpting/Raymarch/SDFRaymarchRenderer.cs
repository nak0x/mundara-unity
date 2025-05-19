using UnityEngine;

[ExecuteInEditMode]
public class SDFRaymarchRenderer : MonoBehaviour
{
    public Material raymarchMat;
    public SDFVolume sdfVolume;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (raymarchMat == null || sdfVolume == null) {
            Graphics.Blit(src, dest);
            return;
        }

        raymarchMat.SetTexture("_VolumeTex", sdfVolume.GetTexture());
        raymarchMat.SetMatrix("_CameraInvViewMatrix", Camera.main.cameraToWorldMatrix);
        raymarchMat.SetMatrix("_CameraInvProjMatrix", Camera.main.projectionMatrix.inverse);
        Graphics.Blit(null, dest, raymarchMat);
    }
}
