using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace WipeCirclePPS
{
    /// <summary>
    /// 円形のフェードを行うエフェクトを描画する処理を定義する
    /// </summary>
    public class WipeCircleRenderFeature : ScriptableRendererFeature
    {
        WipeCirclePass wipeCirclePass;

        public override void Create()
        {
            wipeCirclePass = new WipeCirclePass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            wipeCirclePass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(wipeCirclePass);
        }
    }
}