#ifndef CUSTOM_SURFACE_DATA_INCLUDED
#define CUSTOM_SURFACE_DATA_INCLUDED

/**
 * \brief 计算光照所有需要的数据
 */
struct SurfaceData
{
    float3 normal;
    float3 viewDirection;
    float3 color;
    float alpha;
    float metallic;
    float smoothness;
};


#endif