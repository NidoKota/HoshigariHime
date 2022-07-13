using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HoshigariHime
{
    /// <summary>
    /// 点数表示を動かす(+1や+10など)
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class AddPointMove : MonoBehaviour
    {
        [SerializeField] float upSpeed = 0;     //上に上がる速さ
        [SerializeField] float destroyTime = 0; //非表示にする時間

        TextMeshPro tmp;
        float timer;

        void OnEnable()
        {
            if (!tmp) tmp = GetComponent<TextMeshPro>();

            tmp.alpha = 1;
            timer = 0;
        }

        void Update()
        {
            timer += Time.deltaTime;

            //表示が消える時間に近づくほど透明にする
            tmp.alpha = Mathf.Lerp(1, 0, timer / destroyTime);

            //表示が消える時間を超えたらこのオブジェクトを非表示にする(InstanseControllerに返す)
            if (timer > destroyTime)
            {
                gameObject.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            transform.position += Vector3.up * upSpeed;
        }
    }
}
