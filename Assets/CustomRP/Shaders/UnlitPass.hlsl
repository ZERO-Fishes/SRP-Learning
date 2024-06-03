#ifndef CUSTOM_UNLIT_PASS_INCLUDED 
#define CUSTOM_UNLIT_PASS_INCLUDED


#include "Assets/CustomRP/ShaderLibrary/Common.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
/*CBUFFER_START(UnityPerMaterial)
    float4 _BaseColor;
CBUFFER_END*/
UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4,_MainTex_ST)
    UNITY_DEFINE_INSTANCED_PROP(float4,_BaseColor)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attributes
{
    float4 positionOS:POSITION;
    float4 uv:TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct Varyings
{
    float4 positionCS:SV_POSITION;
    float2 uv:VAR_BASE_UV;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
Varyings UnlitPassVertex (Attributes IN){
    Varyings OUT;
    UNITY_SETUP_INSTANCE_ID(IN)
    UNITY_TRANSFER_INSTANCE_ID(IN,OUT);
    float4 MainTexST=UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_MainTex_ST);
    OUT.uv=IN.uv*MainTexST.xy+MainTexST.zw;
    float3 positionWS;
    positionWS=TransformObjectToWorld(IN.positionOS.xyz);
    OUT.positionCS=TransformWorldToHClip(positionWS);
    return OUT;
}

float4 UnlitPassFragment (Varyings IN):SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(IN)
    float4 baseCol=UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseColor);
    float4 mainTexCol=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,IN.uv);
    return baseCol*mainTexCol;
    //return _BaseColor;
}
#endif