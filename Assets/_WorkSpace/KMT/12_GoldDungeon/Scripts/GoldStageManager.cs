using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GoldStageManager : StageManager, IDamageAddable
{
    [Header("Gold Stage Info")]
    [SerializeField]
    Slider leftTimeBar;
    [SerializeField]
    float timeLimit;

    [SerializeField]
    int clearRewardGold;

    float maxTimeLimit;
    float gainGold;

    bool isTimeOver;

    bool isRewarded = false;

    protected override void StartGame()
    {
        base.StartGame();

        maxTimeLimit = timeLimit;
        gainGold = 0;
        isTimeOver = false;

        StartCoroutine(StartTimerCO());
    }

    public void IDamageAdd(float damage)
    {
        if (isTimeOver)
            return;

        gainGold += damage;
    }
    
    IEnumerator StartTimerCO()
    {
        while (timeLimit > 0)
        {
            leftTimeBar.value = timeLimit / maxTimeLimit;
            timeLimit = Mathf.Clamp(timeLimit - Time.deltaTime, -0.01f, maxTimeLimit);
            yield return null;
        }

        Debug.Log("타임 오버!");
        isTimeOver = true;

        //초과데미지를 주었더라도 최대 보상보다는 적게 주도록 강제
        Rewarding(System.Math.Min(clearRewardGold, (int)gainGold));

    }


    void Rewarding(int rewardGold)
    {
        if (isRewarded)
        { 
            Debug.Log("이미 보상처리가 들어감."); 
            return; 
        }

        isRewarded = true;

        Debug.Log("클리어!");

        ItemGain reward = new ItemGain()
        {
            item = GameManager.TableData.GetItemData(1),
            gain = rewardGold
        };

        var stream = GameManager.UserData.StartUpdateStream();
        stream.
            AddDBValue(reward.item.Number, rewardGold)
            .Submit(result =>
            {
                if (false == result)
                {
                    Debug.Log("요청 전송에 실패했습니다");
                    return;
                }

                Debug.Log("와! 골드!");

                // 아이템 획득 팝업 + 확인 클릭시 메인 화면으로
                ItemGainPopup popupInstance = Instantiate(ItemGainPopupPrefab, GameManager.PopupCanvas);
                popupInstance.Initialize(new List<ItemGain>() { reward });
                popupInstance.Title.text = "와! 골드!";
                popupInstance.onPopupClosed += GameManager.Instance.LoadMainScene;
            });

    }

    protected override void OnClear()
    {
        Rewarding(clearRewardGold);
    }
}
