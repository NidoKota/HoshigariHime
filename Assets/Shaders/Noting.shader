//何もしないシェーダー
Shader "Custom/Noting"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline"
        }
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off
        Pass
        {
            
        }
    }
}