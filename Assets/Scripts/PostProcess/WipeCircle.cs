using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace WipeCirclePPS
{
    /// <summary>
    /// 円形のフェードを行うエフェクトのパラメーター
    /// </summary>
    public class WipeCircle : VolumeComponent, IPostProcessComponent
    {
        /// <summary>
        /// 円の半径  
        /// </summary>
        public MinFloatParameter radius = new MinFloatParameter(-1f, 0f);

        /// <summary>
        /// 円と黒い部分の間の長さ
        /// </summary>
        public FloatParameter fade = new FloatParameter(0f);

        /// <summary>
        /// 円の中心位置
        /// </summary>
        public Vector2Parameter position = new Vector2Parameter(new Vector2(0.5f, 0.5f));

        public bool IsActive() => radius.value >= 0f;
        public bool IsTileCompatible() => false;
    }
}