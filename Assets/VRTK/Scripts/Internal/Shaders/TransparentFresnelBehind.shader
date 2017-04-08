// UNITY_SHADER_NO_UPGRADE
Shader "Custom/TransparentFresnel"
{
	Properties{
		_Color("Rim Color", Color) = (0.5,0.5,0.5,0.5)
		_FPOW("FPOW Fresnel", Float) = 5.0
		_R0("R0 Fresnel", Float) = 0.05
		_MainTex("Bumpmap", 2D) = "bump" {}
	}

		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		SubShader{
		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _MainTex;
	fixed4 _Color;
	float _FPOW;
	float _R0;


	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	float4 _MainTex_ST;

	v2f vert(appdata_t v)
	{
		v2f o;
#if UNITY_VERSION >= 560
		o.vertex = UnityObjectToClipPos(v.vertex);
#else
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
#endif
		o.color = v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

		float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		half fresnel = saturate(1.0 - dot(v.normal, viewDir));
		fresnel = pow(fresnel, _FPOW);
		fresnel = _R0 + (1.0 - _R0) * fresnel;
		o.color *= fresnel;
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		return 2.0f * i.color * _Color * tex2D(_MainTex, i.texcoord);
	}
		ENDCG
	}
	}
	}
}