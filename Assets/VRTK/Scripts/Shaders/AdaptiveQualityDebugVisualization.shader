// Adapted from The Lab Renderer's vr_quality_vis.shader, available at
// https://github.com/ValveSoftware/the_lab_renderer/blob/ae64c48a8ccbe5406aba1e39b160d4f2f7156c2c/Assets/TheLabRenderer/Resources/vr_quality_vis.shader
// For The Lab Renderer's license see THIRD_PARTY_NOTICES.
Shader "VRTK/AdaptiveQualityDebugVisualization"
{
	SubShader
	{
		Tags { "Queue" = "Overlay" "PreviewType" = "Plane" }

		Pass
		{
			Cull Off
			ZWrite Off
			ZTest Always

			CGPROGRAM
				#pragma only_renderers d3d11

				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				v2f_img vert(appdata_img i)
				{
					v2f_img o = vert_img(i);
					o.uv.y = 1.0 - i.texcoord.y;

					return o;
				}

				CBUFFER_START(AdaptiveQualityDebugVisualization)
					uint _LevelsCount = 10;
					uint _DefaultLevel = 6;
					uint _CurrentLevel = 5;
					uint _LastFrameIsInBudget = 1;
				CBUFFER_END

				fixed4 frag(v2f_img i) : SV_TARGET0
				{
					fixed4 o;
					o.rgba = fixed4(0.0, 0.0, 0.0, 1.0);

					uint nLevel = i.uv.x * _LevelsCount;
					
					// Thin bar showing colors
					if (i.uv.y <= 0.1)
					{
						if (nLevel == 0)
							o.rgb = fixed3(0.5, 0.0, 0.0);
						else if (nLevel < _DefaultLevel)
							o.rgb = fixed3(0.5, 0.5, 0.0);
						else
							o.rgb = fixed3(0.0, 0.5, 0.0);
					}
					// Current level
					else if (nLevel == _CurrentLevel)
					{
						if (_LastFrameIsInBudget == 0)
							o.rgb = fixed3(0.5, 0.0, 0.0);
						else if (nLevel < _DefaultLevel)
							o.rgb = fixed3(0.5, 0.5, 0.0);
						else
							o.rgb = fixed3(0.0, 0.5, 0.0);
					}
					// Gray levels
					else
					{
						if (nLevel & 0x1)
							o.rgb = fixed3(0.02, 0.02, 0.02);
						else
							o.rgb = fixed3(0.03, 0.03, 0.03);
					}

					return o;
				}
			ENDCG
		}
	}
}
