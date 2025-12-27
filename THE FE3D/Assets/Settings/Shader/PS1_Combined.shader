Shader "Custom/PS1_FullCombined_Opaque"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _GridSize("Vertex Grid Size", Float) = 4.0
        _JitterAmount("Vertex Jitter Amount", Float) = 0.01
        _WarpAmount("Texture Warp Amount", Float) = 0.02
        _Brightness("Brightness", Float) = 0.0
        _Contrast("Contrast", Float) = 1.0
        _CrushColors("Enable PS1 Color Crush", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _GridSize;
            float _JitterAmount;
            float _WarpAmount;
            float _Brightness;
            float _Contrast;
            float _CrushColors;

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

            // Random function for vertex jitter
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;

                // --- Vertex snapping / jitter ---
                v.vertex.xy = floor(v.vertex.xy * _GridSize) / _GridSize;
                float jitter = (rand(v.vertex.xy) - 0.5) * _JitterAmount;
                v.vertex.xy += jitter;

                o.vertex = UnityObjectToClipPos(v.vertex);

                // --- Affine texture warping ---
                float wobbleX = sin(o.vertex.y * 0.05) * _WarpAmount;
                float wobbleY = cos(o.vertex.x * 0.05) * _WarpAmount;
                o.uv = v.uv + float2(wobbleX, wobbleY);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample texture
                fixed4 tex = tex2D(_MainTex, i.uv);

                // Apply tint
                tex.rgb *= _Color.rgb;

                // Adjust brightness & contrast
                tex.rgb = (tex.rgb - 0.5) * _Contrast + 0.5 + _Brightness;

                // Optional PS1 color crush
                if (_CrushColors > 0.5)
                    tex.rgb = floor(tex.rgb * 32.0) / 31.0;

                // Force fully opaque
                tex.a = 1.0;

                return tex;
            }
            ENDCG
        }
    }
}
