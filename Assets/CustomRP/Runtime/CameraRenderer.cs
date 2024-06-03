using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    private ScriptableRenderContext context;
    private Camera camera;
    private const string bufferName = "CommandBuffer_RenderCamera";
    private string samplerName { get; set; }
    private CommandBuffer buffer=new CommandBuffer(){name=bufferName};
    private CullingResults cullingResults;
    private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    private static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");

    private Lighting lighting = new Lighting();

    public void Render(ScriptableRenderContext context, Camera camera,bool useDynamicBatching,
        bool useGPUInstancing,ShadowSettings shadowSettings)
    {
        this.context = context;
        this.camera = camera;
        PrepareBufferName();
        PrepareForSceneWindow();
        if (!Cull(shadowSettings.maxDistance))
        {
            return;
        }
        Setup();
        lighting.Setup(context,cullingResults,shadowSettings);
        DrawVisiableGeometry(useDynamicBatching,useGPUInstancing);
        DrawUnsupportedShaders();
        DrawGizmos();
        lighting.Cleanup();
        Submit();

    }

    partial void PrepareBufferName();
    partial void PrepareForSceneWindow();

    void Setup()
    {
        //设置相机数据需要在清空render target之前
        context.SetupCameraProperties(camera);
        CameraClearFlags cameraClearFlags = camera.clearFlags;
        //自带一个sample，需要放在采样器之外
        buffer.ClearRenderTarget(cameraClearFlags<=CameraClearFlags.Depth,
            cameraClearFlags<=CameraClearFlags.Color,Color.clear,1.0f);
        buffer.BeginSample(samplerName);
        ExecuteAndClearBuffer();//需要先将这个采样器插入到context
    }
    /// <summary>
    /// 绘制所有可见几何体
    /// </summary>
    void DrawVisiableGeometry(bool useDynamicBatching,bool useGPUInstancing)
    {
        var sortingSettings = new SortingSettings(camera)
        {
            criteria =SortingCriteria.CommonOpaque
        };
        var drawingSettings = new DrawingSettings(unlitShaderTagId,sortingSettings)
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing
        };
        drawingSettings.SetShaderPassName(1,litShaderTagId);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        
        context.DrawRenderers(cullingResults,ref drawingSettings,ref filteringSettings);
        context.DrawSkybox(camera);
        //渲染不透明物体
        sortingSettings.criteria = SortingCriteria.CommonTransparent;//更新渲染顺序
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange=RenderQueueRange.transparent;//更新渲染范围
        context.DrawRenderers(cullingResults,ref drawingSettings,ref filteringSettings);

    }

    /// <summary>
    /// 绘制废弃的shader
    /// </summary>
    partial void DrawUnsupportedShaders();

    partial void DrawGizmos();

    void Submit()
    {
        buffer.EndSample(samplerName);
        ExecuteAndClearBuffer();
        context.Submit();
    }

    //将CommandBuffer中的命令传入到context中，并清空buffer
    void ExecuteAndClearBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    bool Cull(float maxShadowDistance)
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            //阴影剔除距离不能超过相机远截面
            p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }
    
    
}
