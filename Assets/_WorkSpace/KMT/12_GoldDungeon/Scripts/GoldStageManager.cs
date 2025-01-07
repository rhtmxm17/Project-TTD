using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldStageManager : StageManager, IDamageAddable
{
    [Header("Gold Stage Info")]
    [SerializeField]
    Slider leftTimeBar;

    [SerializeField]
    int clearRewardGold;

    float maxTimeLimit;
    float gainGold;

    protected override void StartGame()
    {
        base.StartGame();

        maxTimeLimit = timeLimit;
        gainGold = 0;
    }


    public void IDamageAdd(float damage)
    {
        if (isCombatEnd)
            return;

        gainGold += damage;
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

        if (isCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            yield break;
        }

        isCombatEnd = true;
        Debug.Log("타임 오버!");

        //초과데미지를 주었더라도 최대 보상보다는 적게 주도록 강제
        Rewarding(System.Math.Min(clearRewardGold, (int)gainGold));

    }


    void Rewarding(int rewardGold)
    {

        Debug.Log("클리어!");

        ItemGain reward = new ItemGain()
        {
            item = GameManager.TableData.GetItemData(1),
            gain = rewardGold
        };

        var stream = GameManager.UserData.StartUpdateStream();
        stream
            .AddDBValue(reward.item.Number, rewardGold)
            .AddDBValue(DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number, -1)
            .Submit(result =>
            {
                if (false == result)
                {
                    Debug.Log("요청 전송에 실패했습니다");
                    return;
                }

                Debug.Log("와! 골드!");

                // 아이템 획득 팝업 + 확인 클릭시 메인 화면으로
                ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(new List<ItemGain>() { reward });
                popupInstance.Title.text = "와! 골드!";
                popupInstance.onPopupClosed += GameManager.Instance.LoadMainScene;
            });

    }

    protected override void OnClear()
    {
        if (isCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        isCombatEnd = true;

        Debug.Log("클리어!");

        Rewarding(clearRewardGold);
    }
}
