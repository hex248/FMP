Shader "Kazi/ToonShader"
{
    Properties
    {
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _Comp("Stencil Compare", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _Depth("Z Write Compare", Float) = 2
        [Enum(UnityEngine.Rendering.StencilOp)] _Pass("Stencil Pass", Float) = 0
        [Enum(None,0,Alpha,1,Red,8,Green,4,Blue,2,RGB,14,RGBA,15)] _ColorMask("Writing Color Mask", Int) = 15
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 1.0
        _ClipThreshold("Clip Threshold", Range(0.0, 1.0)) = 0.5
        _DissolveScale("DissolveScale", Float) = 50
        _DissolveAmount("DissolveAmount", Range(0, 1)) = 0.0
        _DissolveWidth("DissolveWidth", Float) = 0.02
        [HDR]_DissolveColor("DissolveColor", Color) = (0.2094241, 0.670157, 7.999999, 1)
    }
        SubShader
    {
        Cull [_Cull]
        ZWrite [_Depth]
        ColorMask [_ColorMask]
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel" = "4.5"}
        Stencil
        {
            Ref[_StencilID]
            Comp [_Comp]
            Pass [_Pass]
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
            float _DissolveAmount;
            float _DissolveScale;
            float _DissolveWidth;
            float4 _DissolveColor;
            float4 _BaseMap_ST;
            float4 _NormalMap_ST;
            float _ClipThreshold;
            float _Smoothness;
            half4 _BaseColor;
            CBUFFER_END

                inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
            {
                return (1.0 - t) * a + (t * b);
            }


            inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                return t;
            }
            float Unity_SimpleNoise_float(float2 UV, float Scale)
            {
                float t = 0.0;

                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3 - 0));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3 - 1));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3 - 2));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                return t;
            }

            float Unity_Step_float(float Edge, float In)
            {
                return step(Edge, In);
            }

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
                float3 positionWS = IN.positionWSAndFogFactor.xyz;
                float4 color = float4(0.0, 0.0, 0.0, 1.0);
                float value;
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
                color.rgb += Unity_Step_float(Unity_SimpleNoise_float(IN.uv, _DissolveScale), _DissolveAmount + _DissolveWidth) * _DissolveColor;
                color.rgb = MixFog(color.rgb, IN.positionWSAndFogFactor.w);
                color.a *= 1 - Unity_Step_float(Unity_SimpleNoise_float(IN.uv, _DissolveScale), _DissolveAmount);
                clip(color.a * _BaseColor.a * _BaseMap.Sample(sampler_BaseMap, IN.uv).a - 0.5f);
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
