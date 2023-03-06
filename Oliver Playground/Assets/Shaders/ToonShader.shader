Shader "Kazi/ToonShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "black" {}
        _ClipThreshold("Clip Threshold", FLOAT) = 0.5
    }

        // Universal Render Pipeline subshader. If URP is installed this will be used.
        SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass
        {
            Name "StandardLit"
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_fog
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normal       : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float3 normal       : TEXCOORD1;
                float4 shadowCoord  : TEXCOORD2;
                float4 positionHCS  : SV_POSITION;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _NormalMap_ST;
            float _ClipThreshold;
            half4 _BaseColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normal = IN.normal;
                OUT.shadowCoord = GetShadowCoord(vertexInput);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                float height, width;
                _NormalMap.GetDimensions(height, width);
                Light mainLight = GetMainLight(IN.shadowCoord);
                if (width < 32.0 || height < 32.0)
                {
                    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(IN.normal);
                    IN.normal = vertexNormalInput.normalWS.xyz;
                    float value = dot(IN.normal, mainLight.direction.xyz) * mainLight.shadowAttenuation;
                    if (value > _ClipThreshold)
                    {
                        value = 1.0;
                    }
                    else
                    {
                        value = 0.5;
                    }
                    return value * _BaseColor * float4(mainLight.color, 1.0) * _BaseMap.Sample(sampler_BaseMap, IN.uv);
                }
                else 
                {
                    float3 normal = (_NormalMap.Sample(sampler_NormalMap, IN.uv) / 0.5) - 0.5;
                    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(float3(normal.x, normal.z, -normal.y));
                    IN.normal = vertexNormalInput.normalWS.xyz;
                    float value = dot(IN.normal, mainLight.direction.xyz) * mainLight.shadowAttenuation;
                    if (value > _ClipThreshold)
                    {
                        value = 1.0;
                    }
                    else 
                    {
                        value = 0.5;
                    }
                    return value * _BaseColor * float4(mainLight.color, 1.0) * _BaseMap.Sample(sampler_BaseMap, IN.uv);
                }
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}
