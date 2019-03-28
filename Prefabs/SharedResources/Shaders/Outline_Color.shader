// UNITY_SHADER_NO_UPGRADE
Shader "VRTK/Outline_Color"
{
	Properties
	{
		_OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
		_Thickness("Thickness", float) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		// Fill the stencil buffer
		Pass
		{
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
				ZFail Replace
			}

			ColorMask 0
		}

		// Draw the outline
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off // On (default) = Ignore lights etc. Should this be a property?
			Stencil
			{
				Ref 0
				Comp Equal
			}

			CGPROGRAM
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag

				#include "UnityCG.cginc"
				half4 _OutlineColor;
				float _Thickness;

				struct appdata
				{
					float4 vertex : POSITION;
				};

				struct v2g
				{
					float4 pos : SV_POSITION;
				};

				v2g vert(appdata IN)
				{
					v2g OUT;
					OUT.pos = UnityObjectToClipPos(IN.vertex);
					return OUT;
				}

				void geom2(v2g start, v2g end, inout TriangleStream<v2g> triStream)
				{
					float width = _Thickness / 100;
					float4 parallel = (end.pos - start.pos) * width;
					float4 perpendicular = normalize(float4(parallel.y, -parallel.x, 0, 0)) * width;
					float4 v1 = start.pos - parallel;
					float4 v2 = end.pos + parallel;
					v2g OUT;
					OUT.pos = v1 - perpendicular;
					triStream.Append(OUT);
					OUT.pos = v1 + perpendicular;
					triStream.Append(OUT);
					OUT.pos = v2 - perpendicular;
					triStream.Append(OUT);
					OUT.pos = v2 + perpendicular;
					triStream.Append(OUT);
				}

				[maxvertexcount(12)]
				void geom(triangle v2g IN[3], inout TriangleStream<v2g> triStream)
				{
					geom2(IN[0], IN[1], triStream);
					geom2(IN[1], IN[2], triStream);
					geom2(IN[2], IN[0], triStream);
				}

				half4 frag(v2g IN) : COLOR
				{
					_OutlineColor.a = 1;
					return _OutlineColor;
				}
			ENDCG
		}
	}
	FallBack "Diffuse"
}