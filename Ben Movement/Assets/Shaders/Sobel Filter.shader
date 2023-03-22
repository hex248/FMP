Shader "Kazi/Sobel Filter" 
{

	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Threshold("Line Clip Threshold", Float) = 0.5
		_Color("Color", Color) = (0.0, 0.0, 0.0, 1.0)
	}
		SubShader
		{
			Tags {"RenderPipeline" = "UniversalPipeline"}
			HLSLINCLUDE

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionHCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float _DeltaX;
			float _DeltaY;
			float _Threshold;
			float4 _Color;
			float4 _MainTex_ST;

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			float sobel(Texture2D tex, float2 uv, float deltaX, float deltaY)
			{
				float2 delta = float2(deltaX / _ScreenParams.x, deltaY / _ScreenParams.y);

				float4 hr = float4(0, 0, 0, 0);
				float4 vt = float4(0, 0, 0, 0);

				hr += tex.Sample(sampler_MainTex, (uv + float2(-1.0, -1.0) * delta)) * 1.0;
				hr += tex.Sample(sampler_MainTex, (uv + float2(0.0, -1.0) * delta)) * 0.0;
				hr += tex.Sample(sampler_MainTex, (uv + float2(1.0, -1.0) * delta)) * -1.0;
				hr += tex.Sample(sampler_MainTex, (uv + float2(-1.0,  0.0) * delta)) * 2.0;
				hr += tex.Sample(sampler_MainTex, (uv + float2(0.0,  0.0) * delta)) * 0.0;
				hr += tex.Sample(sampler_MainTex, (uv + float2(1.0,  0.0) * delta)) * -2.0;
				hr += tex.Sample(sampler_MainTex, (uv + float2(-1.0,  1.0) * delta)) * 1.0;
				hr += tex.Sample(sampler_MainTex, (uv + float2(0.0,  1.0) * delta)) * 0.0;
				hr += tex.Sample(sampler_MainTex, (uv + float2(1.0,  1.0) * delta)) * -1.0;

				vt += tex.Sample(sampler_MainTex, (uv + float2(-1.0, -1.0) * delta)) * 1.0;
				vt += tex.Sample(sampler_MainTex, (uv + float2(0.0, -1.0) * delta)) * 2.0;
				vt += tex.Sample(sampler_MainTex, (uv + float2(1.0, -1.0) * delta)) * 1.0;
				vt += tex.Sample(sampler_MainTex, (uv + float2(-1.0,  0.0) * delta)) * 0.0;
				vt += tex.Sample(sampler_MainTex, (uv + float2(0.0,  0.0) * delta)) * 0.0;
				vt += tex.Sample(sampler_MainTex, (uv + float2(1.0,  0.0) * delta)) * 0.0;
				vt += tex.Sample(sampler_MainTex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
				vt += tex.Sample(sampler_MainTex, (uv + float2(0.0,  1.0) * delta)) * -2.0;
				vt += tex.Sample(sampler_MainTex, (uv + float2(1.0,  1.0) * delta)) * -1.0;

				return sqrt(dot(hr, hr) + dot(vt, vt));
			}

			Varyings vert(Attributes IN)
			{
				Varyings OUT;
				OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				return OUT;
			}

			ENDHLSL
			Pass 
			{
				HLSLPROGRAM
				half4 frag(Varyings IN) : SV_TARGET
				{
					float s = sobel(_MainTex, IN.uv,_DeltaX,_DeltaY);
					half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
					if (s > _Threshold)
					{
						return 1.0;
					}
					else
					{
						return 0.0;
					}
				}
				ENDHLSL
			}
			Pass
			{
				HLSLPROGRAM
				half4 frag(Varyings IN) : SV_TARGET
				{
					half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
					clip(col.a - 0.5);
					return col * _Color;
				}
				ENDHLSL
			}
	}
}