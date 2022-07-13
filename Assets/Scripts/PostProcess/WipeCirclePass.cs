using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace WipeCirclePPS
{
    /// <summary>
    /// 円形のフェードを行うエフェクトを描画する
    /// </summary>
    public class WipeCirclePass : ScriptableRenderPass
    {
        static readonly string renderTag = "RenderWipeCircle";
        static readonly int tempTargetId = Shader.PropertyToID("_TempTargetWipeCircle");
        static readonly int radiusId = Shader.PropertyToID("_Radius");
        static readonly int fadeId = Shader.PropertyToID("_Fade");
        static readonly int positionID = Shader.PropertyToID("_Position");
        WipeCircle wipeCircle;
        Material wipeCircleMaterial;
        CommandBuffer cmd;
        RenderTargetIdentifier currentTarget;

        public WipeCirclePass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            wipeCircleMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Custom/WipeCircle"));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (wipeCircleMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            if (!renderingData.cameraData.postProcessEnabled) return;

            wipeCircle = VolumeManager.instance.stack.GetComponent<WipeCircle>();
            if (wipeCircle == null)
            {
                return;
            }

            if (!wipeCircle.IsActive())
            {
                return;
            }

            cmd = CommandBufferPool.Get(renderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }

        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            wipeCircleMaterial.SetFloat(radiusId, wipeCircle.radius.value);
            wipeCircleMaterial.SetFloat(fadeId, wipeCircle.fade.value);
            wipeCircleMaterial.SetVector(positionID, wipeCircle.position.value);

            cmd.GetTemporaryRT(tempTargetId, renderingData.cameraData.camera.scaledPixelWidth,
                renderingData.cameraData.camera.scaledPixelHeight, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(currentTarget, tempTargetId);
            cmd.Blit(tempTargetId, currentTarget, wipeCircleMaterial, 0);
            cmd.ReleaseTemporaryRT(tempTargetId);
        }
    }
}