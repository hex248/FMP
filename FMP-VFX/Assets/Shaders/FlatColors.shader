Shader "Kazi/FlatColors" {

	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader{
			Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

			CGINCLUDE

			#include "UnityCG.cginc"

			sampler2D _MainTex;

			float4 frag(v2f_img IN) : COLOR
			{
				float4 image = tex2D(_MainTex, IN.uv);
				return image;
			}

			ENDCG

			Pass
			{
				Name "Forward"
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				ENDCG
			}

		}
			FallBack "Universal Render Pipeline/Lit"
}