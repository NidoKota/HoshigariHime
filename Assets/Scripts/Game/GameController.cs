using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using UniRx;
using Cinemachine;
using TMPro;

namespace HoshigariHime
{
    /// <summary>
    /// ゲーム全般の処理
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [SerializeField] Transform timelines = default;         //全てのTimelineの親オブジェクト
        [Space]
        [SerializeField] TextMeshProUGUI timeText = default;    //時間を表示するTMP
        [SerializeField] TextMeshProUGUI pointText = default;   //ポイントを表示するTMP
        [SerializeField] SpriteRenderer lane = default;         //レーンを表示するSpriteRenderer
        [SerializeField] AudioSource bGM = default;             //BGM
        [SerializeField] AudioSource itemGetGoodSE = default;   //命令通りの時のSE
        [SerializeField] AudioSource itemGetNormalSE = default; //命令通りでない時のSE
        [SerializeField] AudioSource laneIncreaseSE = default;  //レーンを増やすイベントのSE
        [Space]
        [SerializeField] float limitTime = 0;                   //ゲームが終了するまでの時間
        [SerializeField] int laneIncreasePoint6 = 0;            //レーンが6個になるのに必要なポイント
        [SerializeField] int laneIncreasePoint7 = 0;            //レーンが7個になるのに必要なポイント
        [SerializeField] int badToGoodPoint = 0;                //ゲーム終了時にこのポイントを超えたら帝が喜ぶ
        [Space]
        [SerializeField] float pointSmooth = 0;                 //ポイントが増えたときに数字をアニメーションする定数
        [SerializeField] float bGMFadeSpeed = 0;                //BGMをフェードする速さ
        [Space]
        [SerializeField] float laneSeparate = 0;                //レーンの数(アニメーションでこの数値を変更する)

        /// <summary>
        /// 現在のポイント
        /// </summary>
        public int Point { get; private set; }

        /// <summary>
        /// ゲームオーバーかどうか
        /// </summary>
        public bool IsGameOver { get; private set; }

        /// <summary>
        /// レーンが増える演出中かどうか
        /// </summary>
        public bool IsLaneIncrease { get; private set; }

        /// <summary>
        /// 初めの演出が終わったかどうか
        /// </summary>
        public bool firstStartTimelineFinished { get; private set; }

        /// <summary>
        /// 初めの演出が終わった時に発火するイベント
        /// </summary>
        public event Action<GameController> firstStartTimelineFinishedEvent;

        /// <summary>
        /// ゲームオーバー時に発火するイベント
        /// </summary>
        public event Action<GameController> GameOverEvent;

        /// <summary>
        /// レーンが増える演出が始まる時に発火するイベント
        /// </summary>
        public event Action<GameController> laneIncreaseStartEvent;

        /// <summary>
        /// レーンが増える演出が終わる時に発火するイベント
        /// </summary>
        public event Action<GameController> laneIncreaseFinishEvent;

        /// <summary>
        /// trueの場合 ゲーム開始時に長い方の演出になる
        /// </summary>
        public static bool first = true;

        /// <summary>
        /// 最後にプレイした時のポイント
        /// </summary>
        public static int lastPoint;

        PlayerMove playerController;
        ClacLanePositions lanePositions;
        ItemController itemController;
        PlayableDirector firstTimeline;
        PlayableDirector startTimeline;
        PlayableDirector laneIncreaseTimeline6;
        PlayableDirector laneIncreaseTimeline7;
        PlayableDirector gameOverGoodTimeline;
        PlayableDirector gameOverBadTimeline;

        float liTimer;
        float liTimerInt;
        float pointMoveTime;
        float initBGMVolume;
        int separateChecker = -1;
        int pointMove;

        void Start()
        {
            playerController = FindObjectOfType<PlayerMove>();
            lanePositions = FindObjectOfType<ClacLanePositions>();
            itemController = FindObjectOfType<ItemController>();
            firstTimeline = timelines.Find("FirstTimeline").GetComponent<PlayableDirector>();
            startTimeline = timelines.Find("StartTimeline").GetComponent<PlayableDirector>();
            laneIncreaseTimeline6 = timelines.Find("LaneIncreaseTimeline6").GetComponent<PlayableDirector>();
            laneIncreaseTimeline7 = timelines.Find("LaneIncreaseTimeline7").GetComponent<PlayableDirector>();
            gameOverBadTimeline = timelines.Find("GameOverBadTimeline").GetComponent<PlayableDirector>();
            gameOverGoodTimeline = timelines.Find("GameOverGoodTimeline").GetComponent<PlayableDirector>();

            liTimer = limitTime;

            //リスタートした時は初めの演出を短いものにする
            if (first)
            {
                firstTimeline.Play();
                first = false;
            }
            else startTimeline.Play();

            initBGMVolume = bGM.volume;
            Fade.FadeCustom(bGM, bGMFadeSpeed, 0, initBGMVolume);

            ApplyLaneCount();
        }

        void Update()
        {
            if (IsGameOver)
            {
                pointMove = Point;
            }
            else
            {
                liTimerInt = Mathf.Clamp(Mathf.FloorToInt(liTimer), 0, Mathf.Infinity);
                if (firstStartTimelineFinished)
                {
                    if (liTimerInt <= 10) timeText.color = Color.red;

                    //時間がまだある時
                    if (liTimer > 0)
                    {
                        liTimer -= Time.deltaTime;
                        timeText.SetText("{0:0}", liTimerInt);
                    }
                    //時間切れになった時
                    else
                    {
                        timeText.SetText("0");
                        Time.timeScale = 0;

                        //ゲームオーバーの演出を実行
                        if (Point >= badToGoodPoint) gameOverGoodTimeline.Play();
                        else gameOverBadTimeline.Play();

                        Fade.FadeCustom(bGM, bGMFadeSpeed, bGM.volume, 0, unscaledTime: true);
                        GameOverEvent?.Invoke(this);
                        IsGameOver = true;
                    }
                }
                else timeText.SetText("{0:0}", liTimerInt);

                //ポイントの数字を滑らかにアニメーションする
                if (pointMoveTime < 1) pointMoveTime += Time.deltaTime * pointSmooth / 100000;
                pointMove = Mathf.CeilToInt(Mathf.Lerp(pointMove, Point - 0.1f, pointMoveTime));
            }

            pointText.SetText("{0:0}", pointMove);

            //レーンが増える演出を行う
            if (playerController.IsGround && !IsGameOver && !IsLaneIncrease)
            {
                if (laneIncreasePoint6 <= Point && laneSeparate == 5)
                {
                    laneIncreaseStartEvent?.Invoke(this);
                    laneIncreaseTimeline6.Play();
                    IsLaneIncrease = true;
                    laneIncreaseSE.Play();
                }
                else if (laneIncreasePoint7 <= Point && laneSeparate == 6)
                {
                    laneIncreaseStartEvent?.Invoke(this);
                    laneIncreaseTimeline7.Play();
                    IsLaneIncrease = true;
                    laneIncreaseSE.Play();
                }
            }

            if (IsLaneIncrease) ApplyLaneCount();
        }

        //レーンの変更を適応する
        void ApplyLaneCount()
        {
            //処理するレーンが増えたらListに要素を追加
            if (separateChecker != Mathf.CeilToInt(laneSeparate))
            {
                separateChecker = Mathf.CeilToInt(laneSeparate);

                lanePositions.SetLaneCount(separateChecker);
                itemController.SetLaneCount(separateChecker);
            }
            lanePositions.SetLaneSeparate(laneSeparate);

            //シェーダーにも値を送る
            lane.material.SetFloat("_Separate", laneSeparate);
        }

        /// <summary>
        /// ポイントを加算しSEを鳴らす
        /// </summary>
        public void AddPoint(int point)
        {
            Point += point;
            pointMoveTime = 0;

            //10ポイント増えるかどうかでSEを変える
            if (point == 10) itemGetGoodSE.PlayOneShot(itemGetGoodSE.clip);
            else itemGetNormalSE.PlayOneShot(itemGetNormalSE.clip);
        }

        /////////以下の関数はUnityEditor上で各種Timelineが再生終了したときに呼び出すよう設定する/////////

        /// <summary>
        /// UnityEvent用
        /// </summary>
        public void FirstStartTimelineFinished()
        {
            firstStartTimelineFinishedEvent?.Invoke(this);
            firstStartTimelineFinished = true;
        }

        /// <summary>
        /// UnityEvent用
        /// </summary>
        public void LaneIncreaseTimelineFinished()
        {
            laneIncreaseFinishEvent?.Invoke(this);
            IsLaneIncrease = false;
        }

        /// <summary>
        /// UnityEvent用
        /// </summary>
        public void GameOverTimelineFinished()
        {
            lastPoint = Point;
            Time.timeScale = 1;
            SceneManager.LoadScene("Result");
        }
    }
}
