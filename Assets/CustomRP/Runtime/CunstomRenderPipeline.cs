using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    private CameraRenderer cameraRenderer = new CameraRenderer();
    bool useDynamicBatching, useGPUInstancing;
    private ShadowSettings shadowSettings;

    public CustomRenderPipeline(bool useDynamicBatching,bool useGPUInstancing,bool useSRPBatcher,
        ShadowSettings shadowSettings)
    {
        this.shadowSettings = shadowSettings;
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;//SRP Batcher开关
        GraphicsSettings.lightsUseLinearIntensity=true;//光源强度使用线性值
    }
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
    }

    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameraRenderer.Render(context,cameras[i],useDynamicBatching,useGPUInstancing,shadowSettings);
        }
    }
}
