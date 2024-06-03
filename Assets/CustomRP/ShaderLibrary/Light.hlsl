#ifndef CUSTOM_LIGHT_INCLUDED
#define CUSTOM_LIGHT_INCLUDED

#include "Assets/CustomRP/ShaderLibrary/SurfaceData.hlsl"
#define MAX_DIRECTIONAL_LIGHT_COUNT 4
CBUFFER_START(_CustomLight)
    float _DirectionalLightCount;
    float4 _DirectionalLightColor[MAX_DIRECTIONAL_LIGHT_COUNT];
    float4 _DirectionalLightDirection[MAX_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

int GetDirectionalLightCount()
{
    return _DirectionalLightCount;
}

struct Light
{
    float3 color;
    float3 direction;
};
//通过索引获取指定方向光
Light GetDirectionalLight(int index)
{
    Light light;
    light.color=_DirectionalLightColor[index].rgb;
    light.direction=normalize(_DirectionalLightDirection[index].rgb);
    return light;
}

#endif