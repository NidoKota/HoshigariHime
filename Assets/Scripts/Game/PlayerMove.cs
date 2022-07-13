using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshigariHime
{
    /// <summary>
    /// プレイヤーを動かす
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] AnimationCurve moveXCurve = default;          //プレイヤーの座標を滑らかに次のレーンのX座標に変更する
        [SerializeField] AnimationCurve addSpriteYPosCurve = default;  //プレイヤーの子供のSpriteのY座標を滑らかに変更する(加算)
        [SerializeField] AnimationCurve spriteAddScaleCurve = default; //プレイヤーの子供のSpriteの拡大率を滑らかに変更する(X軸方向へは加算、Y軸方向へは減算する)
        [SerializeField] int firstLaneIndex = 0;                       //初めにいるレーンのIndex
        [SerializeField] float nextMoveTime = 0;                       //移動が始まった後 次の移動を許可する秒数
        [SerializeField] float moveSpeedMultiply = 1;                  //移動速度の乗数
        [SerializeField] bool highSpeedMove = false;                   //押しっぱなしでも移動できるようになる

        /// <summary>
        /// プレイヤーが接地しているか(レーンへの移動が完了しているか)
        /// </summary>
        public bool IsGround { get; private set; }

        GameController gameController;
        ClacLanePositions lanePositions;
        ItemController itemController;
        MenuController menuController;
        Rigidbody2D rb;
        Transform spriteBone;
        SpriteRenderer sprite;
        float initSpriteYPos;
        float movingTime;
        float maxAnimationTime;
        int nextLaneIndex;
        int input;
        bool up;

        void Start()
        {
            gameController = FindObjectOfType<GameController>();
            itemController = FindObjectOfType<ItemController>();
            menuController = FindObjectOfType<MenuController>();
            lanePositions = FindObjectOfType<ClacLanePositions>();
            rb = GetComponent<Rigidbody2D>();
            spriteBone = transform.Find("SpriteBone");
            sprite = spriteBone.Find("Sprite").GetComponent<SpriteRenderer>();

            //一番長いアニメーションの時間を計算する
            maxAnimationTime = new[] { moveXCurve.keys[moveXCurve.length - 1].time, addSpriteYPosCurve.keys[addSpriteYPosCurve.length - 1].time, spriteAddScaleCurve.keys[spriteAddScaleCurve.length - 1].time }.Max();

            movingTime = maxAnimationTime;
            nextLaneIndex = firstLaneIndex;
            initSpriteYPos = spriteBone.localPosition.y;
        }

        void Update()
        {
            //レーンを増やしている時はレーンの位置から動かないようにする
            //(Time.timeScaleが0なので物理演算が動かない よってtransform.positionを使う)
            if (gameController.IsLaneIncrease)
            {
                transform.position = new Vector2(lanePositions.LaneXPositions[nextLaneIndex], rb.position.y);
            }

            //ゲームオーバーや各種演出 メニューを開いていない時
            if (!gameController.IsGameOver && gameController.firstStartTimelineFinished && !gameController.IsLaneIncrease && !menuController.IsDisplay)
            {
                input = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;

                movingTime += Time.deltaTime * moveSpeedMultiply;

                movingTime = Mathf.Clamp(movingTime, 0, maxAnimationTime);
                nextLaneIndex = Mathf.Clamp(nextLaneIndex, 0, lanePositions.LaneXPositions.Count - 1);

                //滑らかに移動 spriteBoneの上下移動 spriteBoneの拡大縮小を行う
                rb.MovePosition(new Vector2(Mathf.Lerp(rb.position.x, lanePositions.LaneXPositions[nextLaneIndex], moveXCurve.Evaluate(movingTime)), rb.position.y));
                spriteBone.localPosition = new Vector2(0, initSpriteYPos + addSpriteYPosCurve.Evaluate(movingTime));
                spriteBone.localScale = new Vector2(1 + spriteAddScaleCurve.Evaluate(movingTime), 1 - spriteAddScaleCurve.Evaluate(movingTime));

                if (IsGround)
                {
                    //一度移動キーを離した または離さなくてよい設定の時
                    if (up && !highSpeedMove || highSpeedMove)
                    {
                        //移動できる範囲内で移動キーを押した時
                        if (input > 0 && nextLaneIndex < lanePositions.LaneXPositions.Count - 1 || input < 0 && nextLaneIndex > 0)
                        {
                            //キャラを反転させる
                            sprite.flipX = input < 0 ? false : input > 0 ? true : sprite.flipX;

                            rb.MovePosition(new Vector2(lanePositions.LaneXPositions[nextLaneIndex], rb.position.y));
                            spriteBone.localPosition = new Vector2(0, initSpriteYPos);
                            spriteBone.localScale = new Vector2(1, 1);

                            //次のレーンを設定し アニメーションの時間をリセットする
                            nextLaneIndex += input;
                            movingTime = 0;

                            up = false;
                        }
                    }
                }

                //演出中に動けてしまうバグが出るのでIsGroundは使用してから判定する
                if (movingTime >= nextMoveTime) IsGround = true;
                else IsGround = false; 

                if (input == 0) up = true;
            }

            if (gameController.IsGameOver) spriteBone.localScale = new Vector2(1, 1);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            //ゲームオーバーや各種演出 メニューを開いていない時
            if (!gameController.IsGameOver && gameController.firstStartTimelineFinished && !menuController.IsDisplay)
            {
                //衝突したアイテムを非表示にしてItemControllerに渡す
                collision.gameObject.SetActive(false);
                itemController.AcquiredItem(collision.GetComponent<ItemMove>());
            }
        }
    }
}