using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExpStageManager : StageManager
{
    [Header("Exp Stage Info")]
    [SerializeField]
    Slider leftTimeBar;

    string curLevel;
    float maxTimeLimit;

    Combatable bossCharacters;
    AdvencedPopupInCombatResult_Dungeon resultPopupInDungeon;

    protected override void Awake()
    {
        base.Awake();
        resultPopupInDungeon = (AdvencedPopupInCombatResult_Dungeon)resultPopupWindow;
    }

    public override void Initialize(StageSceneChangeArgs sceneChangeArgs)
    {
        base.Initialize(sceneChangeArgs);
        curLevel = sceneChangeArgs.dungeonLevel.ToString();
    }


    protected override void StartGame()
    {
        base.StartGame();
        maxTimeLimit = timeLimit;
    }


    protected override void OnDefeat()
    {
        if (IsCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        IsCombatEnd = true;

        Rewarding(false);

    }

    protected override IEnumerator StartTimerCO()
    {
        TimeSpan leftTimeSpan = TimeSpan.FromSeconds(timeLimit);
        leftTimeText.text = $"{leftTimeSpan.Minutes:D2} : {leftTimeSpan.Seconds:D2}";

        while (timeLimit > 0)
        {
            leftTimeBar.value = timeLimit / maxTimeLimit;

            yield return new WaitForSeconds(1);
            timeLimit -= 1;
            leftTimeSpan = TimeSpan.FromSeconds(timeLimit);
            leftTimeText.text = $"{leftTimeSpan.Minutes:D2} : {leftTimeSpan.Seconds:D2}";
        }

        if (IsCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            yield break;
        }

        IsCombatEnd = true;
        Debug.Log("타임 오버!");

        //클리어 여부만 반환.
        Rewarding(false);// => 보상 로직

    }

    //=>보상 로직
    void Rewarding(bool isClear)
    {

        int waveCount = Math.Clamp(curWave, 1, maxWave);//1~max(3)로 고정시킴.

        float resultRate = (waveCount - 1) * (1 / (float)maxWave);//현재 웨이브(max - 1) * (1/max) => 0, 1/3, 2/3 3개중에 하나
        long resultLong = (int)(100 * resultRate);//현재 웨이브(3 - 1) * 33% + 1

        long prevResultLong = 0;

        if (GameManager.UserData.PlayData.GoldDungeonClearRate.Value.ContainsKey(curLevel))
            prevResultLong = GameManager.UserData.PlayData.GoldDungeonClearRate.Value[curLevel];

        if (isClear)//클리어인경우, 클리어률을 100으로 지정.
        {
            resultLong = 100;
            resultRate = 1;
        }
        else if (resultLong == 0)//웨이브를 아예 못 민 경우, 최대 보상의 20%
        {
            resultRate = 0.2f;
        }
        else//클리어가 아닌 경우라면 99%로 명시.
        {
            resultLong = Math.Min(resultLong, 99);
        }

        int rewardDragonFruit = (int)(stageDataOnLoad.Reward[0].gain * resultRate);

        Debug.Log("클리어!");

        ItemGain reward = new ItemGain()
        {
            item = GameManager.TableData.GetItemData(5),
            gain = rewardDragonFruit
        };

        var stream = GameManager.UserData.StartUpdateStream();

        var expClearRateDic = GameManager.UserData.PlayData.ExpDungeonClearRate.Value;

        if (!expClearRateDic.ContainsKey(curLevel))//첫도전인 경우
        {
            stream.SetDBDictionaryInnerValue(GameManager.UserData.PlayData.ExpDungeonClearRate, curLevel, resultLong);
        }
        else if (expClearRateDic[curLevel] < resultLong)//재도전인데 이전 클리어률보다 큰 경우
        {
            stream.SetDBDictionaryInnerValue(GameManager.UserData.PlayData.ExpDungeonClearRate, curLevel, resultLong);
        }

        List<CharacterData> chDataL = new List<CharacterData>(batchDictionary.Values);
        int randIdx = UnityEngine.Random.Range(0, chDataL.Count);

        resultPopupInDungeon.OpenTripleButtonWithResult(
            stageDataOnLoad.StageName,
            new List<ItemGain>() { reward },
            "획득하기", () => { //아이템 획득

                stream
                .AddDBValue(reward.item.Number, rewardDragonFruit)
                .AddDBValue(DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number, -1)
                .Submit(result => {

                    if (false == result)
                    {
                        Debug.Log("요청 전송에 실패했습니다");
                        return;
                    }

                    LoadPreviousScene();

                });

            },

            "재도전", () =>//재도전.
            {

                GameManager.OverlayUIManager.OpenDoubleInfoPopup("재도전하시겠습니까?", "그만두기", "다시 도전하기", null,
                () =>
                {
                    //TODO : 용도에 따라서 지우거나 이용
                    //prevSceneData.stageData = prevSceneData.stageData;
                    //prevSceneData.prevScene = prevSceneData.prevScene;
                    GameManager.Instance.LoadBattleFormationScene(prevSceneData);
                });

            },

            "다음 스테이지로", () => {

                if (stageDataOnLoad.NextStageId == -1)
                {
                    GameManager.OverlayUIManager.OpenSimpleInfoPopup("다음 단계가 없습니다.", "닫기", null);
                }
                else
                {
                    StageData nextStageData = DataTableManager.Instance.GetStageData(stageDataOnLoad.NextStageId);

                    if (nextStageData == null)
                    {
                        Debug.Log("전달된 다음 인덱스 스테이지 정보가 존재하지 않음.");
                        return;
                    }

                    GameManager.OverlayUIManager.OpenDoubleInfoPopup($"다음 스테이지로 가시겠습니까? \n {nextStageData.StageName}", "그만두기", "도전하기", null,
                        () => {

                            if (DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number.Value <= 1)//보상정산용도 + 다음 입장 용도
                            {
                                GameManager.OverlayUIManager.OpenSimpleInfoPopupByCreate(
                                    "골드 티켓이 부족합니다.",
                                    "확인",
                                    null
                                );
                            }
                            else if (prevResultLong < 100 && resultLong < 100)
                            {
                                GameManager.OverlayUIManager.OpenSimpleInfoPopupByCreate(
                                    "현재 스테이지의 진행률을 \n 100%로 채워야 합니다..",
                                    "확인",
                                    null
                                );
                            }
                            else
                            {

                                stream
                                    .AddDBValue(reward.item.Number, rewardDragonFruit)
                                    .AddDBValue(DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number, -1)
                                    .Submit(result => {

                                        if (false == result)
                                        {
                                            Debug.Log("요청 전송에 실패했습니다");
                                            return;
                                        }

                                        prevSceneData.stageData = nextStageData;
                                        prevSceneData.dungeonLevel += 1;
                                        //TODO : 용도에 따라서 지우거나 이용
                                        //prevSceneData.prevScene = prevSceneData.prevScene;
                                        GameManager.Instance.LoadBattleFormationScene(prevSceneData);

                                    });

                            }
                        },
                        true, DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number.Value > 1 && (prevResultLong >= 100 || resultLong >= 100));
                }

            }/*다음 스테이지 로직*/,
            false, false, false,
            $"{resultLong}%", chDataL[randIdx].FaceIconSprite,
            isClear ? AdvencedPopupInCombatResult.ColorType.VICTORY : AdvencedPopupInCombatResult.ColorType.DEFEAT
            );

    }


    protected override void OnClear()
    {
        if (IsCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        IsCombatEnd = true;

        Debug.Log("클리어!");

        //클리어 여부만 반환
        Rewarding(true);//보상로직.
    }
}
