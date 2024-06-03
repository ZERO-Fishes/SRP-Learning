#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED
//进行所有光照计算
#include "Assets/CustomRP/ShaderLibrary/SurfaceData.hlsl"
#include "Assets/CustomRP/ShaderLibrary/BRDF.hlsl"
#include "Assets/CustomRP/ShaderLibrary/Light.hlsl"

//计算光照结果
float3 CalculateSingleLight(SurfaceData surface_data,BRDF brdf,Light light)
{
    return saturate(dot(surface_data.normal,light.direction))*light.color*
        DirectBRDF(surface_data,brdf,light);//光源数据乘表面BRDF颜色
    //return DirectBRDF(surface_data,brdf,light);
    //return pow(saturate(dot(surface_data.normal,normalize(surface_data.viewDirection+light.direction))),40);
    //return normalize(surface_data.viewDirection+light.direction);
}
//计算所有光照结果
float3 GetLighting(SurfaceData surface_data,BRDF brdf)
{
    float3 col=0.0;
    //遍历所有的直接光，将光照结果累加
    for (int i=0;i<GetDirectionalLightCount();i++)
    {
        col+=CalculateSingleLight(surface_data,brdf,GetDirectionalLight(i));
    }
    return col;
}


#endif