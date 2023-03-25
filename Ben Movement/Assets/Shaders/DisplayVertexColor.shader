Shader "Kazi/Display Vertex Colors" {
    Properties
    {
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
    }
        SubShader{
            Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
            Pass {
                Name "Forward"
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_instancing
        #include "UnityCG.cginc"
        sampler2D _BaseMap;
    float4 _BaseMap_ST;
    half4 _BaseColor;
    struct v2f {
        float4 pos : SV_POSITION;
        float4 color : COLOR0;
        float2 uv : TEXCOORD0;
    };

    v2f vert(appdata_full v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.color = v.color;
        o.uv = v.texcoord;
        return o;
    }

    float4 frag(v2f i) : COLOR
    {
        i.color.a = tex2D(_BaseMap, i.uv).a;
        clip(i.color.a - 0.5);
        return i.color;
    }
    ENDCG

        }
    }
        Fallback "Universal Render Pipeline/Lit"
}
