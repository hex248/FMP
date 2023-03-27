// This shader fills the mesh shape with a color predefined in the code.
Shader "CustomShaders/InteractiveSnow/Snow"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        _HeightMap("Height Map", 2D) = "white" {}
        _NormalMap("Normal", 2D) = "white" {}
        _NormalMapAmount("Normal Map Amount", float) = 1
        _HeightAmount("Height Amount", float) = 0.5
        _TessellationAmount("Tessellation Amount", Range(1,32)) = 8
        _BottomColor("Bottom Color", Color) = (.8,.8,1,1)
    }

    // The SubShader block containing the Shader code. 
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags {"RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque"}
        LOD 200

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader. 
            #pragma vertex vert
            // This line defines the name of the fragment shader. 
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
                float3 normal       : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionCS   : SV_POSITION;
                float3 normal       : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _NormalMap_ST;
            half4 _BaseColor;
            CBUFFER_END

            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous space

                //OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                //OUT.positionCS = float4(positionInputs.positionCS.x, positionInputs.positionCS.y + 0.01, positionInputs.positionCS.z, 0.0);
                

                // float3 vertexPos = IN.positionOS.xyz;
                // vertexPos = float3(vertexPos.x, vertexPos.y + 0.01, vertexPos.z);
                // OUT.positionHCS = TransformObjectToHClip(vertexPos);


                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normal = IN.normal;
                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.            
            half4 frag(Varyings IN) : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}