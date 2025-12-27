Shader "Custom/PS1_Affine_Unlit_Tint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Input from app
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            // Output to fragment shader
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float  w   : TEXCOORD1;
            };

            // Shader properties
            sampler2D _MainTex;
            fixed4 _Color;

            // Vertex shader
            v2f vert (appdata v)
            {
                v2f o;

                // Standard clip position
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                o.pos = clipPos;

                // Pass UV and depth (w) to fragment shader
                o.uv = v.uv;
                o.w  = clipPos.w;

                return o;
            }

            // Fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                // AFFINE TEXTURE MAPPING
                float2 affineUV = i.uv / i.w;

                // Sample texture
                fixed4 tex = tex2D(_MainTex, affineUV);

                // Apply tint color
                return tex * _Color;
            }

            ENDCG
        }
    }
}
