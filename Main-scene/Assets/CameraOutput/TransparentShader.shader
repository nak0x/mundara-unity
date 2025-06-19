Shader "Custom/RemoveWhiteBackgroundWithFade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeStart ("Fade Start (distance to white)", Range(0, 1)) = 0.05
        _FadeEnd ("Fade End (distance to white)", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FadeStart;
            float _FadeEnd;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Préserver les pixels déjà transparents
                if (col.a < 0.01)
                    return col;

                float3 white = float3(1.0, 1.0, 1.0);
                float dist = distance(col.rgb, white);

                // Si la couleur est très proche du blanc, rendre totalement transparent
                if (dist <= _FadeStart)
                {
                    col.a = 0;
                }
                // Si la couleur est entre _FadeStart et _FadeEnd, appliquer un fondu
                else if (dist < _FadeEnd)
                {
                    float t = saturate((dist - _FadeStart) / (_FadeEnd - _FadeStart));
                    col.a *= t;
                }

                return col;
            }
            ENDCG
        }
    }
}
