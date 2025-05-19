// This shader is used to render the SDF volume using raymarching.
Shader "Custom/SDFRaymarch"
{
    Properties {
        _VolumeTex ("SDF Volume", 3D) = "" {}
        _CameraInvViewMatrix ("Inv View", Matrix) = [_World2Object]
        _CameraInvProjMatrix ("Inv Proj", Matrix) = [_Object2World]
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler3D _VolumeTex;
            float4x4 _CameraInvViewMatrix;
            float4x4 _CameraInvProjMatrix;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float sampleSDF(float3 p) {
                return tex3D(_VolumeTex, p).r;
            }

            float3 getRayDir(float2 uv) {
                float4 ndc = float4(uv * 2 - 1, 0, 1);
                float4 eye = mul(_CameraInvProjMatrix, ndc); eye.z = -1; eye.w = 0;
                return normalize(mul(_CameraInvViewMatrix, eye).xyz);
            }

            float4 frag(v2f i) : SV_Target {
                float3 ro = _WorldSpaceCameraPos;
                float3 rd = getRayDir(i.uv);

                float t = 0.0;
                const int MAX_STEPS = 128;
                const float MAX_DIST = 3.0;
                const float SURFACE_DIST = 0.001;
                float3 p;

                for (int j = 0; j < MAX_STEPS; ++j) {
                    p = ro + rd * t;
                    if (any(p < 0) || any(p > 1)) break; // Stay in [0,1]^3

                    float d = sampleSDF(p);
                    if (d < SURFACE_DIST) {
                        float3 n = normalize(float3(
                            sampleSDF(p + float3(0.001, 0, 0)) - sampleSDF(p - float3(0.001, 0, 0)),
                            sampleSDF(p + float3(0, 0.001, 0)) - sampleSDF(p - float3(0, 0.001, 0)),
                            sampleSDF(p + float3(0, 0, 0.001)) - sampleSDF(p - float3(0, 0, 0.001))
                        ));
                        float light = dot(n, normalize(float3(1,1,1))) * 0.5 + 0.5;
                        return float4(light.xxx, 1);
                    }

                    t += d;
                    if (t > MAX_DIST) break;
                }

                return float4(0, 0, 0, 1); // background
            }
            ENDHLSL
        }
    }
}
