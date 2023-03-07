Shader "Kazi/TextureNormal"
{
	Properties
	{
		_NormalTex("Texture", 2D) = "white" {}
		_Type("Type", Int) = 0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }

			Pass
			{
				Tags { "LightMode" = "ForwardBase"}

				CGPROGRAM
				#pragma multi_compile_fwdbase
				#pragma multi_compile_instancing

				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : Normal;
				float2 uv : TEXCOORD0;
			};

			sampler2D _NormalTex;

			float4 _NormalTex_ST;
			uint _Type;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _NormalTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;
				if (_Type == 0) 
				{
					float val = dot(normalize(i.normal), _WorldSpaceLightPos0);
					col.rgb = val;
					col.a = 1.0;
				}
				else 
				{
					float3 tex = tex2D(_NormalTex, i.uv) / 0.5 - 0.5;
					float val = dot(normalize(tex), _WorldSpaceLightPos0);
					col.rgb = val;
					col.a = 1.0;
				}
				return col;
			}
		ENDCG
		}
		}
}