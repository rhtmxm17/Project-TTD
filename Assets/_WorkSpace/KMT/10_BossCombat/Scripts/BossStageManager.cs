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
    [SerializeField]
    float timeLimit;

    float maxTimeLimit;
    float score;

    bool isTimeOver;

    protected override void StartGame()
    {
        base.StartGame();

        maxTimeLimit = timeLimit;
        scoreText.text = "0";
        score = 0;
        isTimeOver = false;

        StartCoroutine(StartTimerCO());

    }

    public void IDamageAdd(float damage)
    {
        if (isTimeOver)
            return;

        score += damage;
        scoreText.text = score.ToString();
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

        Debug.Log(UserData.myUid);
        Debug.Log(UserData.myNickname);

        RankApplier.ApplyRank("boss", UserData.myUid, GameManager.UserData.Profile.Name.Value, (long)(score + timeLimit), () =>
        {
            // 클리어 팝업 + 확인 클릭시 메인 화면으로
            ItemGainPopup popupInstance = Instantiate(ItemGainPopupPrefab, GameManager.PopupCanvas);
            popupInstance.Title.text = "보스전 종료!";
            popupInstance.onPopupClosed += GameManager.Instance.LoadMainScene;

        });
    }

    protected override void OnClear()
    {
        // TODO: 보스 몬스터 처치 구현 필요시 여기서 결과 처리
        Debug.LogWarning("아직 정의되지 않은 동작");

        base.OnClear();
    }

}
