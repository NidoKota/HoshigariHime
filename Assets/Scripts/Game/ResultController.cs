using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

namespace HoshigariHime
{
    /// <summary>
    /// リザルトシーン(Result.scene)を制御する
    /// </summary>
    public class ResultController : MonoBehaviour
    {
        [SerializeField] UIButton restertButton = default;     //リスタートボタン
        [SerializeField] UIButton reload = default;            //ランキングリロードボタン
        [SerializeField] UIButton backToTitleButton = default; //タイトルに戻るボタン

        void Start()
        {
            //前回プレイした点数をランキングに送る
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking(GameController.lastPoint);

            //リスタートボタンを押したらゲームシーン(Game.scene)をロードする
            restertButton.ClickEvent += (button) =>
            {
                Fade.FadeIn(unscaledTime : true).Subscribe(x =>
                {
                    GameController.first = true;
                    SceneManager.LoadScene("Game");
                });
            };

            //ランキングリロードボタンを押したらシーンをリロードする
            reload.ClickEvent += (button) =>
            {
                Fade.FadeIn(unscaledTime : true).Subscribe(x =>
                {
                    GameController.first = true;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                });
            };

            //タイトルに戻るボタンを押したらタイトルシーン(Title.scene)をロードする
            backToTitleButton.ClickEvent += (button) =>
            {
                Fade.FadeIn(unscaledTime : true).Subscribe(x =>
                {
                    GameController.first = true;
                    SceneManager.LoadScene("Title");
                });
            };
        }
    }
}
