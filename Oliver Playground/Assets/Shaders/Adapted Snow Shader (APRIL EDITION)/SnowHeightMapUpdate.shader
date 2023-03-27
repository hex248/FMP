Shader "InteractiveSnow/SnowHeightMapUpdate"
{
	Properties
	{
		_DrawPosition("Drawpos", Vector) = (-1,-1,0,0)
		_DrawBrush("Brush", 2D) = "white" {}
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
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


			TEXTURE2D(_DrawBrush);
			SAMPLER(sampler_DrawBrush);

			CBUFFER_START(UnityPerMaterial)
				float4 _DrawPosition;
				float _DrawAngle, _RestoreAmount, _Offset;
				CBUFFER_END


			float4 frag(v2f_customrendertexture IN) : SV_Target
			{
				float4 previousColor = SAMPLE_TEXTURE2D(_SelfTexture2D, sampler_SelfTexture2D, IN.localTexcoord.xy);
				float2 pos = IN.localTexcoord.xy - _DrawPosition;

				float2x2 rot = float2x2(cos(_DrawAngle), -sin(_DrawAngle),
										sin(_DrawAngle), cos(_DrawAngle));
				pos = mul(rot, pos);
				pos /= _Offset;
				pos += float2(0.5, 0.5);

				float4 drawColor = SAMPLE_TEXTURE2D(_DrawBrush, sampler_DrawBrush, pos);

				return min(previousColor, drawColor);
			}

			ENDHLSL
		}
	}
}