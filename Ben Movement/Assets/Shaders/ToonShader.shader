Shader "Kazi/ToonShader"
{
    Properties
    {
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _Comp("Compare", Float) = 0
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "black" {}
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 1.0
        _ClipThreshold("Clip Threshold", Range(0.0, 1.0)) = 0.5
    }
        SubShader
    {
        Cull [_Cull]
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel" = "4.5"}
        Stencil
        {
            Ref[_StencilID]
            Comp [_Comp]
            Pass Replace
        }
        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ SHADOWS_SHADOWMASK
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
            float _Smoothness;
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
                    if (value > (1 - _Smoothness))
                    {
                        value = 1.5;
                    }
                    else
                    {
                        value = 1.0;
                    }
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
                        if (value > (1 - _Smoothness)) 
                        {
                            value = 1.5;
                        }
                        else 
                        {
                            value = 1.0;
                        }
                    }
                    else
                    {
                        value = 0.0;
                    }
                    color.rgb += value * light.color;
                }
                color.rgb = MixFog(color.rgb, IN.positionWSAndFogFactor.w);
                clip(color.a* _BaseColor.a* _BaseMap.Sample(sampler_BaseMap, IN.uv).a - 0.5f);
                return color * _BaseColor * _BaseMap.Sample(sampler_BaseMap, IN.uv);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
        UsePass "Universal Render Pipeline/Lit/Meta"
    }
}
