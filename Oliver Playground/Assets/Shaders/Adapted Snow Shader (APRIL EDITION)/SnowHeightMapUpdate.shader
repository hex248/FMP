Shader "InteractiveSnow/SnowHeightMapUpdate"
{
	Properties
	{
		_DrawPosition("Drawpos", Vector) = (-1,-1,0,0)
		_DrawBrush("Brush", 2D) = "white" {}
		_PreviousTexture("PreviousTexture", 2D) = "white" {}
		_Offset("Offset", float) = 0.05
	}

		SubShader
		{
			Lighting Off
			Blend One Zero
			Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
			LOD 200

			Pass
			{
				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

				struct Attributes
				{
					float4 positionOS : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float4 color : COLOR;
				};

				struct Varyings
				{
					float4 positionHCS : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float4 color : COLOR;
				};

				TEXTURE2D(_DrawBrush);
				SAMPLER(sampler_DrawBrush);
				TEXTURE2D(_PreviousTexture);
				SAMPLER(sampler_PreviousTexture);

				CBUFFER_START(UnityPerMaterial)
					float4 _DrawPosition;
					float _DrawAngle, _RestoreAmount, _Offset;
				CBUFFER_END

				Varyings vert(Attributes IN)
				{
					Varyings OUT;
					OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
					OUT.uv = IN.uv;
					OUT.normal = IN.normal;
					OUT.tangent = IN.tangent;
					OUT.color = IN.color;
					return OUT;
				}

				float4 frag(Varyings IN) : SV_Target
				{
					float4 previousColor = SAMPLE_TEXTURE2D(_PreviousTexture, sampler_PreviousTexture, IN.positionHCS.xy);
					float2 pos = IN.positionHCS.xy - _DrawPosition;

					float2x2 rot = float2x2(cos(_DrawAngle), -sin(_DrawAngle),
											sin(_DrawAngle), cos(_DrawAngle));
					pos = mul(rot, pos);
					pos /= _Offset;
					pos += float2(0.5, 0.5);

					float4 drawColor = SAMPLE_TEXTURE2D(_DrawBrush, sampler_DrawBrush, pos);
					return drawColor;
					return min(previousColor, drawColor);
				}

				ENDHLSL
			}
		}
}