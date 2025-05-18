using UnityEngine;
public static class SmoothClampedParabola
{
    public static float Evaluate(
        float distance,
        float curveSharpness = -0.2f,
        float curveSkew = 0f,
        float curvePeak = 10.2f,
        float clampSoftness = 5f,
        float intensityScale = 1f)
    {
        float f = curveSharpness * distance * distance + curveSkew * distance + curvePeak;
        float inner = Mathf.Log(1f + Mathf.Exp(-clampSoftness * f));
        float middle = -inner + 1.46f;
        float outer = Mathf.Log(1f + Mathf.Exp(clampSoftness * middle));
        return outer / clampSoftness * intensityScale;
    }
}