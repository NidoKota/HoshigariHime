using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using NaughtyAttributes;

namespace WipeCirclePPS
{
    /// <summary>
    /// 円形のフェードを行うエフェクトを簡単に操作できるようになる
    /// </summary>
    [RequireComponent(typeof(Volume)), ExecuteInEditMode]
    public class WipeCircleController : MonoBehaviour
    {
        /// <summary>
        /// 制御する対象のWipeCircle
        /// </summary>
        public WipeCircle WipeCircle { get; private set; }

        /// <summary>
        /// 円の半径  
        /// </summary>
        [Min(0)] public float radius;

        /// <summary>
        /// 円と黒い部分の間の長さ
        /// </summary>
        public float fade;

        /// <summary>
        /// 指定したTransformを円の中心位置として使用するか
        /// </summary>
        public bool useTransformToPotition;

        /// <summary>
        /// 指定したTransformを円の中心位置にする
        /// </summary>
        [EnableIf("useTransformToPotition")] public Transform[] transforms;

        /// <summary>
        /// どのTransformを使用して円の中心位置にするか
        /// </summary>
        [Min(0), EnableIf("useTransformToPotition")]
        public int transformsIndex;

        /// <summary>
        /// 円の中心位置を手動設定
        /// </summary>
        [DisableIf("useTransformToPotition")] public Vector2 position;

        public bool blackoutOnInitializing;
        WipeCircle _WipeCircle;
        Camera cam;

        void OnEnable()
        {
            cam = Camera.main;

            GetComponent<Volume>().profile.TryGet(out _WipeCircle);
            WipeCircle = _WipeCircle;

            if (Application.isPlaying)
            {
                if (blackoutOnInitializing) radius = 0;
                else radius = 1;
            }
        }

        void Update()
        {
            WipeCircle.radius.value = radius;
            WipeCircle.fade.value = fade;

            //TransformをViewportに変換して円の中心位置にする
            if (useTransformToPotition && transformsIndex < transforms.Length && transforms[transformsIndex])
                WipeCircle.position.value = cam.WorldToViewportPoint(transforms[transformsIndex].position);
            else WipeCircle.position.value = position;
        }
    }
}