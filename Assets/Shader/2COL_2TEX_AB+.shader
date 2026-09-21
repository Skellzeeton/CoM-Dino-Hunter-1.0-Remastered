Shader "GGYY/Model/2COL_2TEX_AB+"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _MainTex ("MainTex(RGB)", 2D) = "" {}
        _SkinColor ("Skin Color", Color) = (1,1,1,1)
        _SkinTex ("SkinTex(RGB)", 2D) = "" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            sampler2D _MainTex, _SkinTex;
            fixed4 _MainColor, _SkinColor;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = v.texcoord0;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color1 = tex2D(_MainTex, i.texcoord0) * _MainColor;
                fixed4 color2 = tex2D(_SkinTex, i.texcoord0) * _SkinColor;
                fixed4 combined = (color1 * color2 * 4.0 * color1.a * color2.a) + color1;
                combined = saturate(combined);
                UNITY_APPLY_FOG(i.fogCoord, combined.rgb);
                return combined;
            }
            ENDCG
        }
        Pass
        {
            Tags { "LightMode"="ForwardAdd" }
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            sampler2D _MainTex, _SkinTex;
            fixed4 _MainColor, _SkinColor;
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord0 : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = v.texcoord0;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color1 = tex2D(_MainTex, i.texcoord0) * _MainColor;
                fixed4 color2 = tex2D(_SkinTex, i.texcoord0) * _SkinColor;
                fixed4 baseColor = (color1 * color2 * 4.0 * color1.a * color2.a) + color1;
                fixed3 albedo = saturate(baseColor).rgb;
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                float distance = length(_WorldSpaceLightPos0.xyz - i.worldPos);
                float atten = 0.5 / (0.5 + distance * distance);
                float NdotL = max(0, dot(normal, lightDir));
                float3 lightContribution = _LightColor0.rgb * NdotL * atten;
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float fresnelTerm = pow(1 - abs(dot(normal, viewDir)), 1.0);
                float3 reflection = lightContribution * fresnelTerm * 0.75;
                float3 finalColor = (albedo * lightContribution) + reflection;
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return fixed4(finalColor, baseColor.a);
            }
            ENDCG
        }
    }
}