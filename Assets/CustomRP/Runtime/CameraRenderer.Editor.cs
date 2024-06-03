using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;
using UnityEditor;

public partial class CameraRenderer
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD//仅在编辑器和开发包中显示
    static ShaderTagId[] legacyShaderTagIds = {//废弃的Shader的Tag
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    private static Material errorMaterial;
    
    /// <summary>
    /// 绘制废弃的shader
    /// </summary>
    partial void DrawUnsupportedShaders()
    {
        if (errorMaterial==null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        //筛选出不支持的ShaderName
        var drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(camera))
        {
            overrideMaterial = errorMaterial
        };
        var filteringSettings = FilteringSettings.defaultValue;
        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i,legacyShaderTagIds[i]);
        }
        context.DrawRenderers(cullingResults,ref drawingSettings,ref filteringSettings);
    }
    partial void PrepareBufferName()
    {
        Profiler.BeginSample("编辑器读取相机名字分配内存");
        //buffer名字和采样器名字一样，都是相机名字
        buffer.name = samplerName = camera.name;
        Profiler.EndSample();
    }
#else
    partial void PrepareBufferName()
    {
        samplerName=bufferName;
    }
#endif
    
#if UNITY_EDITOR    //仅编辑器下
    /// <summary>
    /// 绘制控制器
    /// </summary>
    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera,GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera,GizmoSubset.PostImageEffects);
        }
    }

    /// <summary>
    /// UI转换成场景中几何体
    /// </summary>
    partial void PrepareForSceneWindow()
    {
        if (camera.cameraType==CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }

#endif
}
