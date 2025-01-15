using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnforceStageManager : StageManager
{
    [Header("Enforce Stage Info")]
    [SerializeField]
    Slider leftTimeBar;

    string curLevel;
    float maxTimeLimit;

    Combatable bossCharacters;

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

        int rewardEnforceItem = (int)(stageDataOnLoad.Reward[0].gain * resultRate);

        Debug.Log("클리어!");

        ItemGain reward = new ItemGain()
        {
            item = GameManager.TableData.GetItemData(2),
            gain = rewardEnforceItem
        };

        var stream = GameManager.UserData.StartUpdateStream();

        var enforceClearRateDic = GameManager.UserData.PlayData.EnforceDungeonClearRate.Value;

        if (!enforceClearRateDic.ContainsKey(curLevel))//첫도전인 경우
        {
            stream.SetDBDictionaryInnerValue(GameManager.UserData.PlayData.EnforceDungeonClearRate, curLevel, resultLong);
        }
        else if (enforceClearRateDic[curLevel] < resultLong)//재도전인데 이전 클리어률보다 큰 경우
        {
            stream.SetDBDictionaryInnerValue(GameManager.UserData.PlayData.EnforceDungeonClearRate, curLevel, resultLong);
        }


        stream
            .AddDBValue(reward.item.Number, rewardEnforceItem)
            .AddDBValue(DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number, -1)
            .Submit(result =>
            {
                if (false == result)
                {
                    Debug.Log("요청 전송에 실패했습니다");
                    return;
                }

                Debug.Log("와! 골드!");

                List<CharacterData> chDataL = new List<CharacterData>(batchDictionary.Values);
                int randIdx = UnityEngine.Random.Range(0, chDataL.Count);

                string rightButtonText = string.Empty;
                UnityAction rightButtonAction = null;

                //클리어률 100인 경우 => 다음 스테이지 버튼
                if (GameManager.UserData.PlayData.EnforceDungeonClearRate.Value[curLevel] >= 100)
                {
                    rightButtonText = "다음 스테이지로";

                    //다음 스테이지로 갈수있는지 확인하는 로직
                    rightButtonAction = () => {

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

                                    if (DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number.Value <= 0)
                                    {
                                        GameManager.OverlayUIManager.OpenSimpleInfoPopupByCreate(
                                            "골드 티켓이 부족합니다.",
                                            "확인",
                                            null
                                        );
                                    }
                                    else
                                    {
                                        prevSceneData.stageData = nextStageData;
                                        prevSceneData.dungeonLevel += 1;
                                        //TODO : 용도에 따라서 지우거나 이용
                                        //prevSceneData.prevScene = prevSceneData.prevScene;
                                        GameManager.Instance.LoadBattleFormationScene(prevSceneData);
                                    }
                                },
                                true, DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number.Value > 0);
                        }

                    };


                }
                else//클리어률이 100가 아닌 경우 => 재시도버튼
                {
                    rightButtonText = "재시도";

                    //재시도가 가능한지 확인하는 로직
                    rightButtonAction = () =>
                    {

                        GameManager.OverlayUIManager.OpenDoubleInfoPopup("재도전하시겠습니까?", "그만두기", "다시 도전하기", null,
                        () =>
                        {
                            if (DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number.Value <= 0)
                            {
                                GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                                    "골드 티켓이 부족합니다.",
                                    "확인",
                                    null
                                );
                            }
                            else
                            {
                                //TODO : 용도에 따라서 지우거나 이용
                                //prevSceneData.stageData = prevSceneData.stageData;
                                //prevSceneData.prevScene = prevSceneData.prevScene;
                                GameManager.Instance.LoadBattleFormationScene(prevSceneData);
                            }
                        });

                    };
                }

                resultPopupWindow.OpenDoubleButtonWithResult(
                    stageDataOnLoad.StageName,
                    new List<ItemGain>() { reward },
                    "확인", LoadPreviousScene,
                    rightButtonText, rightButtonAction,
                    true, false,
                    isClear ? "승리!" : "패배", chDataL[randIdx].FaceIconSprite,
                    isClear ? AdvencedPopupInCombatResult.ColorType.VICTORY : AdvencedPopupInCombatResult.ColorType.DEFEAT
                );

            });

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
