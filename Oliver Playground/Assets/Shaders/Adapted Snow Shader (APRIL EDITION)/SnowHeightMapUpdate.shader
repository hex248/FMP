Shader "InteractiveSnow/SnowHeightMapUpdate"
{
	Properties
	{
		_DrawPosition("Draw Position", Vector) = (-1,-1,0,0)
		_DrawAngle("Draw Angle", float) = 0.05
		_DrawBrush("Brush", 2D) = "white"
		_PreviousTexture("PreviousTexture", 2D) = "white"
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
				TEXTURE2D(_PreviousTexture);

				SAMPLER(sampler_DrawBrush);
				SAMPLER(sampler_PreviousTexture);

				CBUFFER_START(UnityPerMaterial)
					float4 _DrawPosition;
					float _DrawAngle;
					float _RestoreAmount;
					float _Offset;
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
					//return IN.color;
					float2 pos = IN.uv.xy - _DrawPosition;

					float2x2 rot = float2x2(cos(_DrawAngle), -sin(_DrawAngle),
											sin(_DrawAngle), cos(_DrawAngle));
					pos = mul(rot, pos);
					pos /= _Offset;
					pos += float2(0.5, 0.5);

					float4 drawColor = SAMPLE_TEXTURE2D(_DrawBrush, sampler_DrawBrush, pos);

					//return drawColor;

					float4 previousColor = SAMPLE_TEXTURE2D(_PreviousTexture, sampler_PreviousTexture, IN.uv.xy);
					//return previousColor;
					// return the darkest of the two values - ie only overwrite colour if it is darker than it was previously
					return min(previousColor, drawColor);
				}

				ENDHLSL
			}
		}
}