using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshigariHime
{
    /// <summary>
    /// アイテムを動かす(オブジェクトプール(InstanseController)の使用を想定)
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class ItemMove : MonoBehaviour
    {
        [SerializeField] float downSpeed = 0;   //落ちる速さ
        [SerializeField] float destroyYPos = 0; //非表示にする高さ

        /// <summary>
        /// このアイテムが属するレーンのIndex
        /// </summary>
        [NonSerialized]
        public int laneIndex;

        /// <summary>
        /// このアイテムのIndex
        /// </summary>
        [NonSerialized]
        public int itemIndex;

        Rigidbody2D rb;
        ClacLanePositions lanePositions;
        GameController gameController;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            lanePositions = FindObjectOfType<ClacLanePositions>();
            gameController = FindObjectOfType<GameController>();
        }

        void Update()
        {
            //レーンを増やしている時は落ちずにレーンの位置から動かないようにする
            //(Time.timeScaleが0なので物理演算が動かない よってtransform.positionを使う)
            if (gameController.IsLaneIncrease)
            {
                transform.position = new Vector2(lanePositions.LaneXPositions[laneIndex], rb.position.y);
            }

            //一定の位置を下回ったらこのオブジェクトを非表示にする
            if (destroyYPos > rb.position.y)
            {
                gameObject.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            rb.MovePosition(new Vector2(lanePositions.LaneXPositions[laneIndex], rb.position.y) + Vector2.down * downSpeed);
        }
    }
}
