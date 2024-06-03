using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class Lighting
{
    private const string bufferName = "Lighting";
    private CommandBuffer buffer = new CommandBuffer() {name = bufferName};
    private const int maxDirectionalLightCount=4;
    //要传入shader的光源信息
    private static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
    private static int dirLightColorId = Shader.PropertyToID("_DirectionalLightColor");
    private static int dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
    //存储所有的光源信息
    private static Vector4[] dirLightColorArray = new Vector4[maxDirectionalLightCount];
    private static Vector4[] dirLightDirectionArray = new Vector4[maxDirectionalLightCount];
    private Shadows shadows = new Shadows();
    private CullingResults cullingResults;
    

    //提交获取光源的命令
    public void Setup(ScriptableRenderContext context,CullingResults cullingResults,ShadowSettings shadowSettings)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        shadows.Setup(context,cullingResults,shadowSettings);
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    //将光源信息传入到shader
    void SetupLights()
    {
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
        float dirLightCount = 0;
        //光源信息保存到数组
        for (int i = 0; i < visibleLights.Length; i++)
        {
            VisibleLight light = visibleLights[i];
            //遍历导入所有直接光数据
            if (light.lightType==LightType.Directional)
            {
                SetupDirectionalLight(i,ref light);
                dirLightCount++;
                if (dirLightCount>=maxDirectionalLightCount) {
                    break;
                }
            }
        }
        /*for (int i = 0; i < dirLightDirectionArray.Length; i++)
        {
            Debug.Log("Vector4[" + i + "]: " + dirLightDirectionArray[i]);
        }*/
        //将数组信息传入shader
        buffer.SetGlobalInt(dirLightCountId,visibleLights.Length);
        buffer.SetGlobalVectorArray(dirLightColorId,dirLightColorArray);
        buffer.SetGlobalVectorArray(dirLightDirectionId,dirLightDirectionArray);
    }
    
    /// <summary>
    /// 导入保存单个Directional Light的封装
    /// </summary>
    /// <param name="index">自然数顺序索引</param>
    /// <param name="visibleLight">从相机culling result中获取的可见光</param>
    void SetupDirectionalLight(int index,ref VisibleLight visibleLight)
    {
        //保存颜色
        dirLightColorArray[index] = visibleLight.finalColor;
        //保存方向
        dirLightDirectionArray[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
        //保存(该光源是否有)阴影
        shadows.ReserveDirectionalShadows(visibleLight.light, index);
    }

    public void Cleanup()
    {
        shadows.Cleanup();
    }
}
