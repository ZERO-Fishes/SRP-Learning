#ifndef CUSTOM_LIT_PASS_INCLUDED 
#define CUSTOM_LIT_PASS_INCLUDED


#include "Assets/CustomRP/ShaderLibrary/Common.hlsl"
#include "Assets/CustomRP/ShaderLibrary/SurfaceData.hlsl"
#include "Assets/CustomRP/ShaderLibrary/Light.hlsl"
#include "Assets/CustomRP/ShaderLibrary/BRDF.hlsl"
#include "Assets/CustomRP/ShaderLibrary/Lighting.hlsl"

SurfaceData surface_data;

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
TEXTURE2D(_SmoothnessTex);
SAMPLER(sampler_SmoothnessTex);
/*CBUFFER_START(UnityPerMaterial)
    float4 _BaseColor;
CBUFFER_END*/
UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4,_MainTex_ST)
    UNITY_DEFINE_INSTANCED_PROP(float4,_SmoothnessTex_ST)
    UNITY_DEFINE_INSTANCED_PROP(float4,_BaseColor)
    UNITY_DEFINE_INSTANCED_PROP(float,_Metallic)
    UNITY_DEFINE_INSTANCED_PROP(float,_Smoothness)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attributes
{
    float4 positionOS:POSITION;
    float4 uv:TEXCOORD0;
    float4 normalOS:NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct Varyings
{
    float4 positionCS:SV_POSITION;
    float3 positionWS:VAR_POSITIONWS;
    float2 uv:VAR_BASE_UV;
    float2 uv2:VAR_UV_SMOOTHNESS;
    float3 normalWS:VAR_NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
Varyings LitPassVertex (Attributes IN){
    Varyings OUT;
    UNITY_SETUP_INSTANCE_ID(IN)
    UNITY_TRANSFER_INSTANCE_ID(IN,OUT);
    float4 SmoothTexST=UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_SmoothnessTex_ST);
    OUT.uv2=IN.uv*SmoothTexST.xy+SmoothTexST.zw;
    OUT.positionWS=TransformObjectToWorld(IN.positionOS.xyz);
    OUT.positionCS=TransformWorldToHClip(OUT.positionWS);
    
    float4 MainTexST=UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_MainTex_ST);
    OUT.uv=IN.uv*MainTexST.xy+MainTexST.zw;

    OUT.normalWS=TransformObjectToWorldNormal(IN.normalOS.xyz);
    return OUT;
}

float4 LitPassFragment (Varyings IN):SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(IN)
    float4 baseCol=UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseColor);

    //初始化SurfaceData
    SurfaceData surface_data;
    surface_data.color=baseCol.xyz;
    surface_data.viewDirection=normalize(_WorldSpaceCameraPos-IN.positionWS);//看向物体，一定要记得归一化!
    surface_data.alpha=baseCol.a;
    surface_data.normal=normalize(IN.normalWS);
    surface_data.metallic=UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_Metallic);
    //float smoothTex=SAMPLE_TEXTURE2D(_SmoothnessTex,sampler_SmoothnessTex,IN.uv2).r*0.3;
    surface_data.smoothness=saturate(UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_Smoothness));

    //初始化BRDF数据
    BRDF brdf=GetBRDF(surface_data);

    //计算光照
    float3 col=GetLighting(surface_data,brdf);
    //col.rgb=surface_data.viewDirection;
    float4 mainTexCol=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,IN.uv);
    return float4(col,surface_data.alpha);
    //return _BaseColor;
}
#endif