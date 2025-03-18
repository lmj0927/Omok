Shader"UI/InkEffect"
{
    Properties {
        _MainTex("Texture", 2D) = "white" {}
        _Threshold("Reveal Threshold", Range(0,1)) = 0.0
        _Blur("Blur Amount", Range(0,1)) = 0.05
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Threshold;
            float _Blur;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.texcoord, center);
                // _Threshold 내부는 완전 보이고, _Threshold부터 _Threshold+_Blur 구간에서 부드럽게 투명해짐
                float alpha = 1.0 - smoothstep(_Threshold, _Threshold + _Blur, dist);
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}
