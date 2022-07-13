using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

namespace HoshigariHime
{
    /// <summary>
    /// ゲーム中に一時停止した時のメニュー
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuController : MonoBehaviour 
    {
        [SerializeField] UIButton menuButton = default;        //一時停止ボタン
        [SerializeField] UIButton backToTitleButton = default; //タイトルに戻るボタン
        [SerializeField] UIButton restertButton = default;     //リスタートボタン
        [SerializeField] UIButton backToGameButton = default;  //ゲームに戻るボタン

        /// <summary>
        /// メニューを表示しているかどうか
        /// </summary>
        public bool IsDisplay { get; private set; }

        GameController gameController;
        CanvasGroup canvasGroup;

        void Start()
        {
            gameController = FindObjectOfType<GameController>();
            canvasGroup = GetComponent<CanvasGroup>();

            //最初は全く動かないようにする
            canvasGroup.alpha = 0;
            SetButtonCanMove(false);
            canvasGroup.blocksRaycasts = false;

            //ボタンを押されたらメニューを表示
            menuButton.ClickEvent += (button) =>
            {
                canvasGroup.blocksRaycasts = true;
                Time.timeScale = 0;

                Fade.FadeIn(canvasGroup, unscaledTime: true).Subscribe(x =>
                {
                    SetButtonCanMove(true);
                    IsDisplay = true;
                });
            };

            //ボタンを押されたらシーンを再読み込み
            restertButton.ClickEvent += (button) =>
            {
                SetButtonCanMove(false);
                canvasGroup.blocksRaycasts = false;

                Fade.FadeIn(unscaledTime : true).Subscribe(x =>
                {
                    Time.timeScale = 1;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                });
            };

            //ボタンを押されたらメニューを消す
            backToGameButton.ClickEvent += (button) =>
            {
                SetButtonCanMove(false);
                canvasGroup.blocksRaycasts = false;

                Fade.FadeOut(canvasGroup, unscaledTime: true).Subscribe(x =>
                {
                    if (menuButton) menuButton.canMove = true;
                    Time.timeScale = 1;
                    IsDisplay = false;
                });
            };

            //ボタンを押されたらタイトルシーンを読み込み
            backToTitleButton.ClickEvent += (button) =>
            {
                SetButtonCanMove(false);
                canvasGroup.blocksRaycasts = false;

                Fade.FadeIn(unscaledTime: true).Subscribe(x =>
                {
                    Time.timeScale = 1;
                    SceneManager.LoadScene("Title");
                });
            };

            gameController.firstStartTimelineFinishedEvent += (gameController) =>
            {
                menuButton.canMove = true;
            };

            gameController.laneIncreaseStartEvent += (gameController) =>
            {
                menuButton.canMove = false;
            };

            gameController.laneIncreaseFinishEvent += (gameController) =>
            {
                menuButton.canMove = true;
            };
        }

        //全てのボタン有効/無効を切り替える
        void SetButtonCanMove(bool canMove)
        {
            menuButton.canMove = canMove;
            restertButton.canMove = canMove;
            backToGameButton.canMove = canMove;
            backToTitleButton.canMove = canMove;
        }
    }
}
