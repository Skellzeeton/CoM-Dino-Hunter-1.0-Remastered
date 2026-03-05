Shader "Triniti/Scene/COL_LM"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _LightMap ("Lightmap (RGB)", 2D) = "white" {}
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
            #pragma target 2.0
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            sampler2D _LightMap;
            float4 _MainTex_ST;
            float4 _LightMap_ST;
            fixed4 _Color;
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

                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed4 lightMap = tex2D(_LightMap, i.uv2);

                fixed3 albedo = mainTex.rgb * (_Color.rgb * 0.05);
                fixed3 baked = lightMap.rgb * 20;

                fixed3 finalColor = albedo * baked;

                return fixed4(finalColor, lightMap.a * _Color.a);
            }
            ENDCG
        }
    }
}