#ifndef CUSTOM_COMMON_INCLUDED
#define CUSTOM_COMMON_INCLUDED

//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"


#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"//转换real
#include "Assets/CustomRP/ShaderLibrary/UnityInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"//空间变换
// float3 TransformObjectToWorld (float3 positionOS) {
//     return mul(unity_ObjectToWorld, float4(positionOS, 1.0)).xyz;
// }
//
// float4 TransformWorldToHClip (float3 positionWS) {
//     return mul(unity_MatrixVP, float4(positionWS, 1.0));
// }

float Square(float v)
{
    return v*v;
}

#endif