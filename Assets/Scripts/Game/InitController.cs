using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UniRx;

namespace HoshigariHime
{
    /// <summary>
    /// 初めのシーン(Init.scene)を制御する
    /// </summary>
    public class InitController : MonoBehaviour
    {
        [SerializeField] UIButton startButton = default; //スタートボタン
        [SerializeField] AudioSource bGM = default;      //BGM

        void Start()
        {
            startButton.canMove = false;
            GameController.first = true;

            //フェードが完了したらスタートボタンを押せるようにする
            Fade.FadeOut().Subscribe(x =>
            {
                startButton.canMove = true;
            });

            //スタートボタンを押したらゲームシーン(Game.scene)をロードする
            startButton.ClickEvent += (button) =>
            {
                startButton.canMove = false;

                Fade.FadeIn().Subscribe(x =>
                {
                    SceneManager.LoadScene("Game");
                });

                Fade.FadeCustom(bGM, Fade.defaultSpeed, bGM.volume, 0);
            };
        }
    }
}
