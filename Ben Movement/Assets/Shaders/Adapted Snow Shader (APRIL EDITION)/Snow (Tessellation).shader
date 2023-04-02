Shader "InteractiveSnow/Snow (Tessellation)"
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
		
		_Tess("Tessellation", Range(1, 32)) = 20
		_MinTessDistance("Min Tess Distance", Range(0, 32)) = 20
		_MaxTessDistance("Max Tess Distance", Range(0, 32)) = 20
		_DrawPosition("Draw Position", Vector) = (0,0,0,0)
		_ShadingDetail("Shading Detail", int) = 5
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
		LOD 200

		Pass
		{
			HLSLPROGRAM

			#if defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE) || defined(SHADER_API_VULKAN) || defined(SHADER_API_METAL) || defined(SHADER_API_PSSL)
			#define UNITY_CAN_COMPILE_TESSELLATION 1
			#   define UNITY_domain                 domain
			#   define UNITY_partitioning           partitioning
			#   define UNITY_outputtopology         outputtopology
			#   define UNITY_patchconstantfunc      patchconstantfunc
			#   define UNITY_outputcontrolpoints    outputcontrolpoints
			#endif

			#pragma require tessellation
			#pragma vertex TessellationVertexProgram
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			Vector _DrawPositions[100];

			struct ControlPoint
			{
				float4 vertex : INTERNALTESSPOS;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float3 normal : NORMAL;
			};

			struct Attributes
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			TEXTURE2D(_BaseMap);
			TEXTURE2D(_HeightMap);
			SAMPLER(sampler_BaseMap);
			SAMPLER(sampler_HeightMap);

			CBUFFER_START(UnityPerMaterial)
				float4 _DrawPosition;
				float4 _BaseColor;
				float4 _BottomColor;
				float4 _BaseMap_ST, _HeightMap_ST, _NormalMap_ST;
				half _Height, _NormalMapAmount;
				float _Tess, _MinTessDistance, _MaxTessDistance;
				int _ShadingDetail;
			CBUFFER_END

			ControlPoint TessellationVertexProgram(Attributes v)
			{
				ControlPoint p;

				p.vertex = v.vertex;
				p.uv = v.uv;
				p.normal = v.normal;
				p.color = v.color;

				return p;
			}

			[UNITY_domain("tri")]
			[UNITY_outputcontrolpoints(3)]
			[UNITY_outputtopology("triangle_cw")]
			[UNITY_partitioning("fractional_odd")]
			[UNITY_patchconstantfunc("patchConstantFunction")]
			ControlPoint hull(InputPatch<ControlPoint, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
			{
				float3 worldPosition = mul(unity_ObjectToWorld, vertex).xyz;
				// distance from camera position
				//float dist = distance(worldPosition, GetCameraPositionWS());

				// distance from drawPosition
				float dist = 1000.0;
				// loop all draw positions
				for (float i = 0; i < _DrawPositions.Length; i++)
				{
					float newDist = distance(worldPosition, _DrawPositions[i]);

					// if this is closer, set this as the distance from closest player
					dist = min(dist, newDist);
				}
				//dist = distance(worldPosition, _DrawPosition);

				float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
				return (f);
			}

			TessellationFactors patchConstantFunction(InputPatch<ControlPoint, 3> patch)
			{
				// values for distance fading the tessellation
				float minDist = _MinTessDistance;
				float maxDist = _MaxTessDistance;

				TessellationFactors f;

				float edge0 = CalcDistanceTessFactor(patch[0].vertex, minDist, maxDist, _Tess);
				float edge1 = CalcDistanceTessFactor(patch[1].vertex, minDist, maxDist, _Tess);
				float edge2 = CalcDistanceTessFactor(patch[2].vertex, minDist, maxDist, _Tess);

				// make sure there are no gaps between different tessellated distances, by averaging the edges out
				f.edge[0] = (edge1 + edge2) / 2;
				f.edge[1] = (edge2 + edge0) / 2;
				f.edge[2] = (edge0 + edge1) / 2;
				f.inside = (edge0 + edge1 + edge2) / 3;
				return f;
			}


			Varyings vert(Attributes IN)
			{
				Varyings OUT;

				half4 newPos = IN.vertex;
				half pos = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, IN.uv, 1).r * _Height;
				newPos.y += pos;
				OUT.vertex = TransformObjectToHClip(newPos);

				
				OUT.color = half4(0, round(newPos.y-0.5), 0, 1);
				OUT.normal = IN.normal;
				OUT.uv = TRANSFORM_TEX(IN.uv, _HeightMap);

				return OUT;
			}

			[UNITY_domain("tri")]
			Varyings domain(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
			{
				Attributes v;

#define DomainCalc(fieldName) v.fieldName = \
				patch[0].fieldName * barycentricCoordinates.x + \
				patch[1].fieldName * barycentricCoordinates.y + \
				patch[2].fieldName * barycentricCoordinates.z;

				DomainCalc(vertex)
				DomainCalc(uv)
				DomainCalc(color)
				DomainCalc(normal)

					return vert(v);
			}

			half4 frag(Varyings IN) : SV_Target
			{
				half height = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, IN.uv).r;
				half4 heightColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * lerp(_BottomColor, _BaseColor, round(height * round(_ShadingDetail)) / round(_ShadingDetail)) * _NormalMapAmount;

				//return heightColor;

				half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
				return color * heightColor;
			}

			ENDHLSL
		}
	}
}