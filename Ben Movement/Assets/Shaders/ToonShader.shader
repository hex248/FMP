Shader "Kazi/ToonShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "black" {}
        _ClipThreshold("Clip Threshold", FLOAT) = 0.5
    }
        SubShader
    {
        Tags {"RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Geometry"}

        Pass
        {
            Name "StandardLit"
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
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
                float4 positionWSAndFogFactor   : TEXCOORD3;
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
                float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                OUT.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float height, width;
                float3 positionWS = IN.positionWSAndFogFactor.xyz;
                float4 color = float4(0.0, 0.0, 0.0, 1.0);
                float value;
                _NormalMap.GetDimensions(height, width);
                Light mainLight = GetMainLight(IN.shadowCoord);
                int additionalLightsCount = GetAdditionalLightsCount();
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(IN.normal);
                IN.normal = vertexNormalInput.normalWS.xyz;
                value = dot(IN.normal, mainLight.direction.xyz) * mainLight.shadowAttenuation;
                if (value > _ClipThreshold)
                {
                    value = 1.0;
                }
                else
                {
                    value = 0.5;
                }
                color += value * float4(mainLight.color, 1.0);
                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    Light light = GetAdditionalLight(i, positionWS);
                    value = dot(IN.normal, light.direction.xyz) * light.distanceAttenuation * light.shadowAttenuation;
                    if (value > _ClipThreshold)
                    {
                        value = 1.0;
                    }
                    else
                    {
                        value = 0.0;
                    }
                    color.rgb += value * light.color;
                }
                /*if (width < 32.0 || height < 32.0)
                {
                    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(IN.normal);
                IN.normal = vertexNormalInput.normalWS.xyz;
                value = dot(IN.normal, mainLight.direction.xyz) * mainLight.shadowAttenuation;
                if (value > _ClipThreshold)
                {
                    value = 1.0;
                }
                else
                {
                    value = 0.5;
                }
                color += value * float4(mainLight.color, 1.0);
                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    Light light = GetAdditionalLight(i, positionWS);
                    value = dot(IN.normal, light.direction.xyz) * light.distanceAttenuation * light.shadowAttenuation;
                    if (value > _ClipThreshold)
                    {
                        value = 1.0;
                    }
                    else
                    {
                        value = 0.0;
                    }
                    color.rgb += value * light.color;
                }
                }
                else
                {
                    float3 normal = normalize((_NormalMap.Sample(sampler_NormalMap, IN.uv) * 2.0) - 0.5);
                    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(float3(normal.x, normal.y, normal.z));
                    IN.normal = vertexNormalInput.normalWS.xyz;
                    value = dot(IN.normal, mainLight.direction.xyz) * mainLight.shadowAttenuation;
                    if (value > _ClipThreshold)
                    {
                        value = 1.0;
                    }
                    else
                    {
                        value = 0.5;
                    }
                    color += value * float4(mainLight.color, 1.0);
                    for (int i = 0; i < additionalLightsCount; ++i)
                    {
                        Light light = GetAdditionalLight(i, positionWS);
                        value = dot(IN.normal, light.direction.xyz) * light.shadowAttenuation;
                        if (value > _ClipThreshold)
                        {
                            value = 1.0;
                        }
                        else
                        {
                            value = 0.0;
                        }
                        color.rgb += value * light.color;
                    }
                }*/
                color.rgb = MixFog(color.rgb, IN.positionWSAndFogFactor.w);
                return color * _BaseColor * _BaseMap.Sample(sampler_BaseMap, IN.uv);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/Meta"
    }
}
