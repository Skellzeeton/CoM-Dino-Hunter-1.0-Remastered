Shader "Unlit/Transparent Colored TwoTexture Vertex Color(HardClip)"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _BackTex ("BackTex", 2D) = "black" {}
        _ColorB ("Back Color", Color) = (1,1,1,1)
        _Rect ("Screen Rect", Vector) = (-1,-1,0,0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 200
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BackTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _ColorB;
            float4 _Rect;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 clipRect : TEXCOORD1;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                o.pos = clipPos;

                // UV transform
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                // Hard clip rect calculation
                float2 ndc = clipPos.xy / clipPos.w;
                o.clipRect.x = ndc.x - _Rect.x;
                o.clipRect.y = _Rect.z - ndc.x;
                o.clipRect.z = ndc.y - _Rect.y;
                o.clipRect.w = _Rect.w - ndc.y;

                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (min(i.clipRect.x, i.clipRect.y) < 0 || min(i.clipRect.z, i.clipRect.w) < 0)
                    discard;

                fixed4 mainCol = tex2D(_MainTex, i.uv) * _Color * i.color;
                fixed4 backCol = tex2D(_BackTex, i.uv) * _ColorB;
                return lerp(backCol, mainCol, mainCol.a);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}