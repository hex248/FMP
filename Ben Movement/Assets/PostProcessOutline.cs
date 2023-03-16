using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//[Serializable, VolumeComponentMenuForRenderPipeline("Binjabin/PostProcessOutline", typeof(UniversalRenderPipeline))]
public class PostProcessOutline : VolumeComponent, IPostProcessComponent
{
    public IntParameter scale = new IntParameter(value: 1, overrideState: true );
    public FloatParameter depthThreshold = new FloatParameter(value: 0.2f, overrideState: true);
    [Range(0, 1)]
    public FloatParameter normalThreshold = new FloatParameter( value: 0.4f );
    [Range(0, 1)]
    public FloatParameter depthNormalThreshold = new FloatParameter( value: 0.5f );
    public FloatParameter depthNormalThresholdScale = new FloatParameter( value: 7 );
    public ColorParameter color = new ColorParameter( value: Color.white );

    public bool IsActive() => scale.value > 0;
    public bool IsTileCompatible() => true;
}
/*
public sealed class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Binjabin/Outline Post Process"));
        sheet.properties.SetFloat("_Scale", settings.scale);
        sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
        sheet.properties.SetFloat("_NormalThreshold", settings.normalThreshold);

        sheet.properties.SetFloat("_DepthNormalThreshold", settings.depthNormalThreshold);
        sheet.properties.SetFloat("_DepthNormalThresholdScale", settings.depthNormalThresholdScale);
        sheet.properties.SetColor("_Color", settings.color);
        
        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, true).inverse;
        sheet.properties.SetMatrix("_ClipToView", clipToView);
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
*/