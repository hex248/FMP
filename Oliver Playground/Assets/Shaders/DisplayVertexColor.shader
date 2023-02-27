Shader "Kazi/Display Vertex Colors" {
    SubShader{
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        Pass {
            Name "Forward"
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #include "UnityCG.cginc"

    struct v2f {
        float4 pos : SV_POSITION;
        float4 color : COLOR0;
    };

    v2f vert(appdata_full v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.color = v.color;
        return o;
    }

    float4 frag(v2f i) : COLOR
    {
        return i.color;
    }
    ENDCG

        }
    }
        Fallback "Universal Render Pipeline/Lit"
}
