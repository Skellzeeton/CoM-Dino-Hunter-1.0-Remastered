Shader "GGYY/Model/2COL_2TEX_AB"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _MainTex ("MainTex(RGB)", 2D) = "" {}
        _SkinColor ("Skin Color", Color) = (1,1,1,1)
        _SkinTex ("SkinTex(RGB)", 2D) = "" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            sampler2D _MainTex, _SkinTex;
            fixed4 _MainColor, _SkinColor, _Color;

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
                combined *= _Color;
                UNITY_APPLY_FOG(i.fogCoord, combined.rgb);
                return combined;
            }
            ENDCG
        }
    }
}