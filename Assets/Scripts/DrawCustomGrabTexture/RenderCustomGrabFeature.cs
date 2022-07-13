using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderCustomGrabTexture
{
    /// <summary>
    /// 特定のレンダリング結果を 指定したShaderに渡す処理を定義する
    /// </summary>
    public class RenderCustomGrabFeature : ScriptableRendererFeature
    {
        public string textureName = "_CustomGrabTexture";
        public Downsampling textureDownsampling;
        public RenderPassEvent renderPassEvent;

        private RenderCustomGrabPass renderCustomGrabPass;

        public override void Create()
        {
            renderCustomGrabPass = new RenderCustomGrabPass(textureName, textureDownsampling, renderPassEvent);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderCustomGrabPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(renderCustomGrabPass);
        }

        /// <summary>
        /// レンダリング時の解像度を指定
        /// </summary>
        public enum Downsampling
        {
            None,
            _2x,
            _4x
        }
    }
}
    