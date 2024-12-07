Shader "Custom/GlowLine"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Emission ("Glow Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            float _Emission;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Emission;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(i.color.rgb, 1.0);
            }
            ENDCG
        }
    }
}
