Shader "Triniti/Scene/COL_LM_Anim"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _LightMap ("Lightmap (RGB)", 2D) = "white" {}
        _Multiplier ("Multiplier", Vector) = (0,-0.1,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _LightMap;
            float4 _LightMap_ST;
            fixed4 _Color;
            fixed4 _Multiplier;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _LightMap);

                o.uv += _Time * _Multiplier;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed4 lm = tex2D(_LightMap, i.uv2);
                fixed4 finalCol = col * lm;
                finalCol.rgb *= 2.0;
                return finalCol;
            }
            ENDCG
        }
    }
}
