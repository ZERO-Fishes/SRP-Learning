using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline Asset")]
public class CustomRPAsset : RenderPipelineAsset
{
    [SerializeField]
    private bool useDynamicBatching = true, useGPUInstancing = true, useSRPBatcher = true;

    [SerializeField] 
    private ShadowSettings shadowSettings = default;
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(useDynamicBatching,useGPUInstancing,useSRPBatcher,shadowSettings);
    }
}
