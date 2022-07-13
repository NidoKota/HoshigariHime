using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HoshigariHime
{
    /// <summary>
    /// アイテム生成や点数に関する処理
    /// </summary>
    public class ItemController : MonoBehaviour
    {
        [SerializeField] InstanseController itemInstanseController = default;     //アイテムを生成する時に使うInstanseController
        [SerializeField] GameObject itemObject = default;                         //アイテムのPrefab
        [SerializeField] Sprite[] itemSprites = default;                          //アイテムのSprite
        [SerializeField] float itemInstansePosY = 0;                              //アイテムを生成する位置のY座標
        [SerializeField] float itemInstanseMinimumTime = 0;                       //アイテムを生成する時間の最小値
        [SerializeField] Vector2 itemInstanseTimeRandom = default;                //アイテムを生成する時間のランダム
        [Space]
        [SerializeField] InstanseController addPointInstanseController = default; //点数表示のオブジェクトを生成する時に使うInstanseController
        [SerializeField] AddPointMove addPointObject = default;                   //点数表示のPrefab
        [SerializeField] Transform addPointObjectPosition = default;              //点数表示のオブジェクトを生成する座標
        [SerializeField] Color normalAddPointColor = default;                     //普通の点数表示のオブジェクトの色(命令通りなら赤)

        GameController gameController;
        MenuController menuController;
        CommandController commandController;
        GameObject instanceItem;
        TextMeshPro instanceAddPointTMP;
        ItemMove instanceItemMove;
        List<float> itemInstanseTimes = new List<float>();
        int getPoint;
        int itemInstanceIndex;

        void Start()
        {
            gameController = FindObjectOfType<GameController>();
            menuController = FindObjectOfType<MenuController>();
            commandController = FindObjectOfType<CommandController>();
        }

        void Update()
        {
            //ゲームオーバーや各種演出 メニューを開いていない時
            if (!gameController.IsGameOver && gameController.firstStartTimelineFinished && !gameController.IsLaneIncrease && !menuController.IsDisplay)
            {
                for (int i = 0; i < itemInstanseTimes.Count; i++)
                {
                    //アイテムを生成する時間を超えた時
                    if (Time.time > itemInstanseTimes[i])
                    {
                        //アイテムを選んで生成する
                        itemInstanceIndex = Random.Range(0, itemSprites.Length);
                        instanceItem = itemInstanseController.ReInstantiate(itemObject, new Vector3(0, itemInstansePosY, 0), Quaternion.identity);

                        //アイテムを配置するレーンとアイテムの内容をアイテム側に保存する
                        instanceItemMove = instanceItem.GetComponent<ItemMove>();
                        instanceItemMove.laneIndex = i;
                        instanceItemMove.itemIndex = itemInstanceIndex;
                        instanceItem.GetComponent<SpriteRenderer>().sprite = itemSprites[itemInstanceIndex];

                        //次の生成する時間を決定する
                        itemInstanseTimes[i] = Time.time + itemInstanseMinimumTime + Random.Range(itemInstanseTimeRandom.x, itemInstanseTimeRandom.y);
                    }
                }
            }
        }

        /// <summary>
        /// アイテムを生成するレーンの数を変更する(変更すると前の生成時間は全てリセットされる)
        /// </summary>
        public void SetLaneCount(int count)
        {
            itemInstanseTimes.Clear();
            for (int i = 0; i < count; i++)
            {
                itemInstanseTimes.Add(Time.time + itemInstanseMinimumTime + Random.Range(itemInstanseTimeRandom.x, itemInstanseTimeRandom.y));
            }
        }

        /// <summary>
        /// アイテムを取得したらこの関数を呼び出す(アイテムのポイント調べ 点数表示オブジェクトを生成 GameControllerのポイント加算 を行う)
        /// </summary>
        public void AcquiredItem(ItemMove itemObjct)
        {
            //現在のアイテムの点数を調べる
            if(commandController.State == CommandControllerState.SecondScenario || commandController.State == CommandControllerState.ThirdScenario)
            {
                getPoint = commandController.ItemColorIndex == itemObjct.itemIndex ? 10 : 1;
            }
            else getPoint = 1;

            //点数表示オブジェクトを生成
            instanceAddPointTMP = addPointInstanseController.ReInstantiate(addPointObject.gameObject, addPointObjectPosition.position, Quaternion.identity).GetComponent<TextMeshPro>();
            instanceAddPointTMP.SetText("+{0:0}", getPoint);
            if (getPoint == 10) instanceAddPointTMP.color = Color.red;
            else instanceAddPointTMP.color = normalAddPointColor;

            //GameControllerのポイント加算
            gameController.AddPoint(getPoint);
        }
    }
}
