Shader "InteractiveSnow/Snow"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (.8,.8,1,1)

		_BaseMap("Base Map", 2D) = "white"
		_HeightMap("Height Map", 2D) = "white"
		_Height("Height", float) = 1.0
		_NormalMap("Normal", 2D) = "white" {}
		_NormalMapAmount("Normal Map Amount", float) = 1
		
		_TessellationAmount("Tessellation Amount", Range(1,32)) = 8
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma tessellate tess

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

			TEXTURE2D(_BaseMap);
			TEXTURE2D(_HeightMap);
			SAMPLER(sampler_BaseMap);
			SAMPLER(sampler_HeightMap);

			CBUFFER_START(UnityPerMaterial)
				float4 _BaseColor;
				float4 _BottomColor;
				float4 _BaseMap_ST, _HeightMap_ST, _NormalMap_ST;
				half _Height, _NormalMapAmount;
				float _TessellationAmount;
			CBUFFER_END

			Varyings vert(Attributes IN)
			{
				Varyings OUT;
				OUT.uv = TRANSFORM_TEX(IN.uv, _HeightMap);
				half pos = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, IN.uv, 1).r * _Height;
				half4 newPos = IN.positionOS;
				newPos.y += pos;

				OUT.positionHCS = TransformObjectToHClip(newPos);

				OUT.color = half4(0, newPos.y, 0, 1);
				return OUT;
			}

			half4 frag(Varyings IN) : SV_Target
			{
				//return IN.color;
				half height = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, IN.uv).r;
				half4 heightColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * lerp(_BottomColor, _BaseColor, height) * _NormalMapAmount;

				return heightColor;

				half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
				return color;
			}

			float tess()
			{
				return _TessellationAmount;
			}

			ENDHLSL
		}
	}
}