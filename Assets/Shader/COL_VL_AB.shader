Shader "Triniti/Character/COL_VL_AB"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("MainTex (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf CustomLambert fullforwardshadows noambient
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        struct Input
        {
            float2 uv_MainTex;
        };
        half4 LightingCustomLambert(SurfaceOutput s, half3 lightDir, half atten)
        {
            half NdotL = max(0, dot(s.Normal, lightDir));
            half3 diffuse = s.Albedo * _LightColor0.rgb * NdotL * atten;
            return half4(diffuse, 1);
        }
        void CustomLambert_GI(SurfaceOutput s, UnityGIInput data, inout UnityGI gi)
        {
            gi.indirect.diffuse = 0;
            gi.indirect.specular = 0;
        }
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Emission = c.rgb * 1;
        }
        ENDCG
    }
}
