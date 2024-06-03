#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED

#include "Assets/CustomRP/ShaderLibrary/Common.hlsl"
#include "Assets/CustomRP/ShaderLibrary/SurfaceData.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"

#define MIN_REFLECTIVITY 0.04//非金属的平均F0值

/**
 * \brief 漫反射系数
 * \param matallic 金属度
 * \return 
 */
float oneMinusReflectivity(float matallic)
{
    //反射率越高，漫反射越少
    //因此可以用(1-反射率)来作为漫反射的系数
    //金属的反射率是1，非金属可以近似为0.04
    //因此用金属度在0-0.96之间插值获取漫反射系数
    float range=1-MIN_REFLECTIVITY;
    return range*(1-matallic);
}

/**
 * \brief 计算光照必须的数据
 */
struct BRDF
{
    float3 diffuse;//漫反射颜色
    float3 specular;//高光颜色
    float roughness;
};
BRDF GetBRDF(SurfaceData surface_data)
{
    BRDF brdf;

    //金属度会影响漫反射颜色
    brdf.diffuse=surface_data.color*oneMinusReflectivity(surface_data.metallic);
    //非金属的高光颜色是白色(?)金属的高光颜色是表面颜色
    brdf.specular=lerp(MIN_REFLECTIVITY,surface_data.color,surface_data.metallic);
    //将光滑度转换为粗糙度
    //人感知的粗糙度平方之后才是实际Disney光照模型中的粗糙度
    float perceptualRoughness=
        PerceptualRoughnessToPerceptualSmoothness(surface_data.smoothness);
    brdf.roughness=PerceptualRoughnessToRoughness(perceptualRoughness);
    
    return brdf;
}

/**
 * \brief 用Minimalist CookTorrance BRDF的一种变体计算高光强度
 * \param surface_data 表面数据
 * \param brdf brdf数据
 * \param light 光照数据
 * \return 
 */
float SpecularStrength(SurfaceData surface_data,BRDF brdf,Light light)
{
    //半程向量
    float3 h=SafeNormalize(light.direction+surface_data.viewDirection);
    //(L·H)^2
    float lh2=Square(saturate(dot(light.direction,h)));
    //(N·H)^2
    float nh2=Square(saturate(dot(surface_data.normal,h)));
    //r^2
    float r2=Square(brdf.roughness);
    //d=((N·H)^2)((r^2)-1)+1.0001
    float d2=Square(nh2*(r2-1)+1.0001);
    //n=4r+2  归一化补偿项
    float n=4.0*brdf.roughness+2.0;
    return r2/(d2*max(0.1,lh2)*n);
    
}

//通过BRDF计算光照结果
float3 DirectBRDF(SurfaceData surface_data,BRDF brdf,Light light)
{
    return SpecularStrength(surface_data,brdf,light)*brdf.specular+brdf.diffuse ;
    //return SpecularStrength(surface_data,brdf,light);
    
}
#endif