using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class BossStageManager : StageManager, IDamageAddable
{
    [Header("Boss Stage Info")]
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    Slider leftTimeBar;

    [Header("Reward UI")]
    [SerializeField] ItemGainPopup itemGainPopupPrefab;

    float maxTimeLimit;
    float score;

    protected override void StartGame()
    {
        base.StartGame();

        maxTimeLimit = timeLimit;
        scoreText.text = "0";
        score = 0;
    }


    public void IDamageAdd(float damage)
    {
        if (isCombatEnd)
            return;

        score += damage;
        scoreText.text = score.ToString();
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
        
        RankApplier.ApplyRank("boss", UserData.myUid, GameManager.UserData.Profile.Name.Value, (long)(score + timeLimit), () =>
        {
            // 클리어 팝업 + 확인 클릭시 메인 화면으로
            ItemGainPopup popupInstance = Instantiate(itemGainPopupPrefab, GameManager.PopupCanvas);
            popupInstance.Title.text = "보스전 종료!";
            popupInstance.onPopupClosed += GameManager.Instance.LoadMainScene;

        });
    }

    protected override void OnClear()
    {
        // TODO: 보스 몬스터 처치 구현 필요시 여기서 결과 처리
        Debug.LogWarning("아직 정의되지 않은 동작");

        if (isCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        isCombatEnd = true;

        RankApplier.ApplyRank("boss", UserData.myUid, GameManager.UserData.Profile.Name.Value, (long)(score + timeLimit), () =>
        {
            // 클리어 팝업 + 확인 클릭시 메인 화면으로
            ItemGainPopup popupInstance = Instantiate(itemGainPopupPrefab, GameManager.PopupCanvas);
            popupInstance.Title.text = "보스전 종료!";
            popupInstance.onPopupClosed += GameManager.Instance.LoadMainScene;

        });
    }

}
