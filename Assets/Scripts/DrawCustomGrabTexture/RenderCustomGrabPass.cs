using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Downsampling = RenderCustomGrabTexture.RenderCustomGrabFeature.Downsampling;

namespace RenderCustomGrabTexture
{
    /// <summary>
    /// 特定のレンダリング結果を 指定したShaderに渡す処理を行う
    /// </summary>
    public class RenderCustomGrabPass : ScriptableRenderPass
    {
        private CommandBuffer cmd;
        private RenderTargetIdentifier currentTarget;
        private Downsampling textureDownsampling;
        private string textureName;
        private int customGrabTextureID;
        private int tempTargetId = Shader.PropertyToID("_TempTarget");

        public RenderCustomGrabPass(string textureName, Downsampling textureDownsampling, RenderPassEvent renderPassEvent)
        {
            this.textureName = textureName;
            this.textureDownsampling = textureDownsampling;
            this.renderPassEvent = renderPassEvent;
            
            customGrabTextureID  = Shader.PropertyToID(this.textureName);
        }
        
        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            cmd = CommandBufferPool.Get($"Render{textureName}Pass");

            int width = renderingData.cameraData.camera.scaledPixelWidth;
            int height = renderingData.cameraData.camera.scaledPixelHeight;
            
            if (textureDownsampling == Downsampling._2x)
            {
                width /= 2;
                height /= 2;
            }
            else if (textureDownsampling == Downsampling._4x)
            {
                width /= 4;
                height /= 4;
            }
            
            //tmpにRT生成
            cmd.GetTemporaryRT(tempTargetId, width, height, 0,  textureDownsampling == Downsampling.None ? FilterMode.Point : FilterMode.Bilinear);
            //tmpに現在の出力をコピー
            cmd.Blit(currentTarget, tempTargetId);
            //shaderにtmpを入力
            cmd.SetGlobalTexture(customGrabTextureID, tempTargetId);
            //ターゲット切り替え
            cmd.SetRenderTarget(currentTarget);
            //解放
            cmd.ReleaseTemporaryRT(tempTargetId);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}