Shader "Triniti/Character/COL_2S" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("MainTex(RGB)", 2D) = "" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha
  Color [_Color]
  Cull Off
  SetTexture [_MainTex] { combine texture * primary }
 }
}
}