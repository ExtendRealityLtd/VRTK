Shader "Skybox/Procedural_Color"
{
	Properties
	{
		_SkyColor1("Top Color", Color) = (0.37, 0.52, 0.73, 0)
		_SkyExponent1("Top Exponent", Float) = 2
		_SkyColor2("Horizon Color", Color) = (0.89, 0.96, 1, 0)
		_SkyIntensity("Sky Intensity", Float) = 1.75
		_MoonVector("Moon Vector", Vector) = (0.269, 0.615, 0.740, 0)
		_SunColor("Sun Color", Color) = (1, 0.99, 0.87, 1)
		_SunIntensity("Sun Intensity", Range(0.0,20.0)) = 10.0
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 position : POSITION;
		float3 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float3 texcoord : TEXCOORD0;
	};

	half3 _SkyColor1;
	half _SkyExponent1;
	half3 _SkyColor2;
	half _SkyIntensity;
	half3 _MoonVector;
	half3 _SunColor;
	half _SunIntensity;

	v2f vert(appdata v)
	{
		v2f o;
		o.position = UnityObjectToClipPos(v.position);
		o.texcoord = v.texcoord;
		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		float3 v = normalize(i.texcoord);

		float p = v.y;
		float p1 = 1 - pow(min(1, 1 - p), pow(0.5, v.x*v.z));
		float p2 = 1 - p1;

		half3 c_sky = _SkyColor1 * p1 + _SkyColor2 * p2;
		half3 c_sun = _SunColor * min(pow(max(0, dot(v, _WorldSpaceLightPos0.xyz)), 550), 1);
		half3 c_moon = pow(max(0, dot(v, -_WorldSpaceLightPos0.xyz)), 550);

		return half4(c_sky * _SkyIntensity + c_sun * _SunIntensity + c_moon , 0);
	}

	ENDCG

	SubShader
	{
		Tags{ "RenderType" = "Skybox" "Queue" = "Background" }
		Pass
		{
			ZWrite Off
			Cull Off
			Fog { Mode Off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}