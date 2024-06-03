Shader "CustomRP/LitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor("Color",COLOR)=(0,0,0,0)
        _Metallic("Metallic",Range(0,1))=0
        _SmoothnessTex("Smoothness Texture",2D)="black"{}
        
        _Smoothness("Smoothness",Range(0,1))=0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="CustomLit"}
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma multi_compile_instancing
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Assets/CustomRP/Shaders/LitPass.hlsl"
            
            ENDHLSL
        }
    }
}
