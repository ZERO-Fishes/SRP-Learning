using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Shadows
{
    private const string bufferName = "Shadows";
    private CommandBuffer buffer = new CommandBuffer{name=bufferName};

    private ScriptableRenderContext context;
    private CullingResults cullingResults;
    private ShadowSettings shadowSettings;
    private static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");

    //计数当前已有的阴影光源数量
    private int ShadowedDirectionalLightCount = 0;

    //最大可用阴影的直接光数量
    private const int maxShadowDirectionalLightCount = 1;
    //场景中可能会有很多直接光，所以要知道哪个直接光有阴影，需要保存相应直接光源的索引
    struct ShadowedDirectionalLight
    { 
        public int shadowedDirLightIndex;
    }
    //新建一个直接光的数组，用最大阴影直接光数量限制数组大小
    private ShadowedDirectionalLight[] shadowedDirectionalLights =
        new ShadowedDirectionalLight[maxShadowDirectionalLightCount];

    //判断一个直接光是否有阴影，如果有就保存到脚本中
    public void ReserveDirectionalShadows(Light light,int visibleLightIndex)
    {
        if (ShadowedDirectionalLightCount<maxShadowDirectionalLightCount&&
            light.shadows!=LightShadows.None&&
            light.shadowStrength>0f&&
            cullingResults.GetShadowCasterBounds(visibleLightIndex,out Bounds b))
        {
            shadowedDirectionalLights[ShadowedDirectionalLightCount] = 
                new ShadowedDirectionalLight(){shadowedDirLightIndex = visibleLightIndex};
            ShadowedDirectionalLightCount++;
        }
    }

    private void RenderDirectionalShadows()
    {
        int altasSize = (int)shadowSettings._DirLightShadowSetting.atlasSize;
        buffer.GetTemporaryRT(dirShadowAtlasId,altasSize,altasSize,
            32,FilterMode.Bilinear,RenderTextureFormat.Shadowmap);
        
        buffer.SetRenderTarget(dirShadowAtlasId,
            RenderBufferLoadAction.DontCare,
            RenderBufferStoreAction.Store
        );
        buffer.ClearRenderTarget(true,false,Color.clear);
        ExecuteAndClearCommandBuffer();
    }
    public void Render()
    {
        if (ShadowedDirectionalLightCount>0)//有阴影光源
        {
            RenderDirectionalShadows();
        }
        else
        {
            buffer.GetTemporaryRT(dirShadowAtlasId,1,1,
                32,FilterMode.Bilinear,RenderTextureFormat.Shadowmap);
        }
    }

    public void Cleanup()
    {
        buffer.ReleaseTemporaryRT(dirShadowAtlasId);
        ExecuteAndClearCommandBuffer();
    }
    
    /// 传入SRP中的工具及数据
    public void Setup(ScriptableRenderContext context,CullingResults cullingResults,
        ShadowSettings shadowSettings)
    {
        this.context = context;
        this.cullingResults = cullingResults;
        this.shadowSettings = shadowSettings;
        this.ShadowedDirectionalLightCount = 0;

    }

    private void ExecuteAndClearCommandBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
