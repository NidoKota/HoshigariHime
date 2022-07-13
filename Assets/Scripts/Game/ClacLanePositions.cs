using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshigariHime
{
    /// <summary>
    /// 自身の位置(画面の左端)からすべてのレーンの位置を計算する
    /// </summary>
    public class ClacLanePositions : MonoBehaviour
    {
        /// <summary>
        /// レーンのX座標(0番目の要素は一番左のレーン)
        /// </summary>
        public List<float> LaneXPositions { get; private set; } = new List<float>();

        float displayLength;

        void Start()
        {
            displayLength = Mathf.Abs(transform.position.x * 2);
        }

        /// <summary>
        /// 位置を計算するレーンの数を変更する(変更すると前の結果は全てリセットされる)
        /// </summary>
        public void SetLaneCount(int count)
        {
            LaneXPositions.Clear();
            for (int i = 0; i < count; i++)
            {
                LaneXPositions.Add(0);
            }
        }

        /// <summary>
        /// 画面に表示するレーンの数を変更する(小数点が含まれていても その割合分を表示する)
        /// </summary>
        public void SetLaneSeparate(float separate)
        {
            for (int i = 0; i < Mathf.CeilToInt(separate); i++)
            {
                LaneXPositions[i] = (displayLength / separate) * i + (displayLength / separate) / 2 - displayLength / 2;
            }
        }
    }
}
