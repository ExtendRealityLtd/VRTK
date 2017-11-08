Shader "VRTK/VRTK_TunnelEffect"
{
    Properties
    {
        _Color("Color Tint", Color) = (0,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
        _AngularVelocity ("Angular Velocity", Float) = 0
        _FeatherSize ("FeatherSize", Float) = 0.1
    }

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _AngularVelocity;
            float _FeatherSize;
            half4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
                fixed4 col = tex2D(_MainTex, uv);

                float2 coords = (i.uv - 0.5) * 2;
                float radius = length(coords) / 1.414214;
                float avMin = (1 - _AngularVelocity) - _FeatherSize;
                float avMax = (1 - _AngularVelocity) + _FeatherSize;
                float t = saturate((radius - avMin) / (avMax - avMin));
                return lerp(col, _Color, t);
            }

            ENDCG
        }
    }
}