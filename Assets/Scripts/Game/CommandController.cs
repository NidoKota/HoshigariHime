using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScenarioController;
using UniRx;

namespace HoshigariHime
{
    /// <summary>
    /// 姫の命令を決め 台詞を表示する
    /// </summary>
    public class CommandController : MonoBehaviour
    {
        [SerializeField] Animator himeAnimator = default;        //姫のAnimator

        [SerializeField] float minimumTime = 0;                  //姫が命令を出す最小の時間(下の○○Randomに加算する)
        [SerializeField] Vector2 firstScenarioRandom = default;  //初めの台詞を出す時間のランダムの範囲
        [SerializeField] Vector2 secondScenarioRandom = default; //2番目の台詞を出す時間のランダムの範囲
        [SerializeField] Vector2 thirdScenarioRandom = default;  //3番目台詞を出す時間のランダムの範囲
        [SerializeField] Vector2 noneScenarioRandom = default;   //台詞を出さない時間のランダムの範囲

        [SerializeField] ScenarioInput trouble = default;        //姫が初めに話す何気ないお言葉
        [SerializeField] ScenarioInput command = default;        //姫の命令
        [SerializeField] ScenarioInput chat = default;           //姫の命令したオブジェクトに関する話題
                                                                 //この話題は1つのオブジェクトに対し2つ設定する
                                                                 //0番目のオブジェクトは0か1番目の台詞が 1番目のオブジェクトには1か2番目の台詞がランダムで表示される

        /// <summary>
        /// 姫が命令した色の違いも含めたアイテムのIndex
        /// </summary>
        public int ItemColorIndex { get; private set; }

        /// <summary>
        /// 現在のステート
        /// </summary>
        public CommandControllerState State { get; private set; } = CommandControllerState.None;

        ScenarioDisplayBase scenarioDisplayBase;
        GameController gameController;
        MenuController menuController;
        Scenario firstScenario;
        Scenario secondScenario;
        Scenario thirdScenario;
        StringBuilder itemCmText = new StringBuilder();

        float nextTime;
        int itemThingIndex;
        int itemThingIndexBuffer;
        int cmIndex;

        void Start()
        {
            gameController = FindObjectOfType<GameController>();
            menuController = FindObjectOfType<MenuController>();
            scenarioDisplayBase = trouble.scenarioDisplayBase;

            //ScenarioControllerが非表示になったらステートをNoneにする
            scenarioDisplayBase.ScenarioStateChangeEvent += (state) =>
            {
                if (state == ScenarioDisplayState.Hide) State = CommandControllerState.None;
            };
        }

        void Update()
        {
            //ゲームオーバーや各種演出 メニューを開いていない時
            if (!gameController.IsGameOver && gameController.firstStartTimelineFinished && !gameController.IsLaneIncrease && !menuController.IsDisplay)
            {
                //次の行動に進む時間を超えた時
                if (Time.time > nextTime)
                {
                    //まだScenarioDisplayが非表示の時
                    if (scenarioDisplayBase.State == ScenarioDisplayState.Hide)
                    {
                        State = CommandControllerState.FirstScenario;

                        //最初のScenario(何気ないお言葉)をランダムで選ぶ
                        firstScenario = trouble.scenarios[UnityEngine.Random.Range(0, trouble.scenarios.Count)];

                        //2番目のScenario(命令)をランダムで選ぶ
                        cmIndex = UnityEngine.Random.Range(0, command.scenarios.Count);

                        //アイテムをランダムで選ぶ(色は考慮しない)(前に出した命令と同じアイテムなら選び直す)
                        while (itemThingIndexBuffer == itemThingIndex) itemThingIndex = UnityEngine.Random.Range(0, 4);
                        itemThingIndexBuffer = itemThingIndex;

                        //アイテムの色をランダムで選ぶ(3番目のScenario(命令したオブジェクトに関する話題)をランダムで選ぶ)
                        if (itemThingIndex == 0) ItemColorIndex = UnityEngine.Random.Range(0, 2);
                        if (itemThingIndex == 1) ItemColorIndex = UnityEngine.Random.Range(2, 4);
                        if (itemThingIndex == 2) ItemColorIndex = UnityEngine.Random.Range(4, 6);
                        if (itemThingIndex == 3) ItemColorIndex = UnityEngine.Random.Range(6, 8);

                        CommItemReplace();

                        scenarioDisplayBase.PlayScenario(firstScenario, secondScenario, thirdScenario);

                        nextTime = Time.time + minimumTime + UnityEngine.Random.Range(firstScenarioRandom.x, firstScenarioRandom.y);
                        if (firstScenario.stateData.name != "Normal") himeAnimator.SetBool(firstScenario.stateData.name, true);
                    }
                    //ScenarioDisplayの表示が終わって待機している時
                    else if (scenarioDisplayBase.State == ScenarioDisplayState.Wait)
                    {
                        //一度アニメーターの値をリセット
                        himeAnimator.SetBool("Smile", false);
                        himeAnimator.SetBool("Surprise", false);
                        himeAnimator.SetBool("CloseEyes", false);
                        
                        //最初のScenario(何気ないお言葉)が表示されている時
                        if (scenarioDisplayBase.scenarioIndex == 0)
                        {
                            State = CommandControllerState.SecondScenario;

                            scenarioDisplayBase.Next();

                            nextTime = Time.time + minimumTime + UnityEngine.Random.Range(secondScenarioRandom.x, secondScenarioRandom.y);
                            if (secondScenario.stateData.name != "Normal") himeAnimator.SetBool(secondScenario.stateData.name, true);
                        }
                        //2番目のScenario(命令)が表示されている時
                        else if (scenarioDisplayBase.scenarioIndex == 1)
                        {
                            State = CommandControllerState.ThirdScenario;

                            scenarioDisplayBase.Next();

                            nextTime = Time.time + minimumTime + UnityEngine.Random.Range(thirdScenarioRandom.x, thirdScenarioRandom.y);
                            if (thirdScenario.stateData.name != "Normal") himeAnimator.SetBool(thirdScenario.stateData.name, true);
                        }
                        //3番目のScenario(命令したオブジェクトに関する話題)が表示されている時
                        else
                        {
                            scenarioDisplayBase.Next();

                            nextTime = Time.time + minimumTime + UnityEngine.Random.Range(noneScenarioRandom.x, noneScenarioRandom.y);
                        }
                    }
                }
            }
        }

        //2と3番目の台詞に含まれる"(item)"という文字列を取り除き TMPで姫の命令したアイテムのSpriteを表示するコマンドに置き換える
        void CommItemReplace()
        {
            itemCmText.Clear();
            itemCmText.Append("<sprite=");
            itemCmText.Append(ItemColorIndex);
            itemCmText.Append(">");
            
            SetScenario(ref secondScenario, command.scenarios[cmIndex].stateData, command.scenarios[cmIndex].text.Replace("(item)", itemCmText.ToString()), command.scenarios[cmIndex].charTime, command.scenarios[cmIndex].animationData, command.scenarios[cmIndex].isOverrideName, command.scenarios[cmIndex].Name);
            SetScenario(ref thirdScenario, chat.scenarios[ItemColorIndex].stateData, chat.scenarios[ItemColorIndex].text.Replace("(item)", itemCmText.ToString()), chat.scenarios[ItemColorIndex].charTime, chat.scenarios[ItemColorIndex].animationData, chat.scenarios[ItemColorIndex].isOverrideName, chat.scenarios[ItemColorIndex].Name);
        }

        //Scenarioの内容を変更するだけ
        public void SetScenario(ref Scenario scenario, ScenarioCharacterStateData stateData, string text, float charTime, ScenarioTMPAnimationData animationData, bool isOverrideName, string name)
        {
            scenario.stateData = stateData;
            scenario.text = text;
            scenario.charTime = charTime;
            scenario.animationData = animationData;
            scenario.isOverrideName = isOverrideName;
            scenario.Name = name;
        }
    }

    /// <summary>
    /// CommandControllerのState
    /// </summary>
    public enum CommandControllerState
    {
        /// <summary>
        /// 初めのScenarioを再生している状態
        /// </summary>
        FirstScenario,

        /// <summary>
        /// 2番目のScenarioを再生している状態
        /// </summary>
        SecondScenario,

        /// <summary>
        /// 3番目のScenarioを再生している状態
        /// </summary>
        ThirdScenario,

        /// <summary>
        /// どんなScenarioも再生していない状態
        /// </summary>
        None,
    }

}
