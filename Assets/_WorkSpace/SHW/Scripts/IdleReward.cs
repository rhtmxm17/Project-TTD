using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IdleReward : MonoBehaviour
{
    [Header("초기화 필드")]
    [SerializeField] private bool isIdleReward;
    [SerializeField] private TMP_Text timeText;
    // 보상 팝업 관련
    [SerializeField] private GameObject rewardPopup;
    [SerializeField] private TMP_Text rewardText;

    // 인스펙터 타이머 조절 변수
    [SerializeField] private int rewardHour;
    [SerializeField] private int rewardMinute;
    [SerializeField] private int rewardSecond;

    // 마지막 보상시간:DB
    private UserDataDateTime lastRewardTime;
    // 최대 보상 시간
    //[SerializeField] TimeSpan maxTimer;
    // 지난시간
    private TimeSpan spanTime;
    
    private Coroutine timerCoroutine;

    private void Start()
    {
        // 최대 보상 수령 시간 지정
        // maxTimer = new TimeSpan(rewardHour, rewardMinute, rewardSecond);
    }

    private void OnEnable() => StartIdleReward();

    private void StartIdleReward()
    {
        lastRewardTime = GameManager.UserData.PlayData.IdleRewardTimestamp;
        timerCoroutine = StartCoroutine(TimerTextCo());
        lastRewardTime.onValueChanged += lastRewardTime_onValueChanged;
    }

    private void OnDisable()
    {
        timerCoroutine = null;
        lastRewardTime.onValueChanged -= lastRewardTime_onValueChanged;
    }

    // 테스트용
    public void TestReward()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        lastRewardTime.onValueChanged -= lastRewardTime_onValueChanged;
        spanTime = new TimeSpan(rewardHour, rewardMinute, rewardSecond);
        timeText.text = "테스트 가동중!! 즉시 수령 가능!!";
        isIdleReward = true;
    }
    
    // 보상의 받는 부분
    public void GetReward()
    {
        // 분당 (골드150+용과1) + 스테이지 * (골드10+용과1)

        // 마지막 보상 수령으로부터 지난 분 수
        int timeRewardMult = (int)spanTime.TotalMinutes;

        if (timeRewardMult <= 0)
        {
            Debug.Log("아직 보상이 충전되지 않음");
            return;
        }

        // 보상 수령 전 코루틴 정지
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        // 클리어 한 스테이지 수
        int clearedStages = 0;
        for (int i = 1; true; i++)
        {
            StageData stage = GameManager.TableData.GetStageData(i);
            if ((stage == null) || stage.ClearCount.Value == 0)
            {
                break;
            }
            clearedStages++;
        }

        ItemGain goldGain = new ItemGain()
        {
            item = GameManager.TableData.GetItemData(1),
            gain = timeRewardMult * (150 + 15 * clearedStages) // 분당 골드: 150 + 15 * 스테이지
        };

        ItemGain yonggwaGain = new ItemGain()
        {
            item = GameManager.TableData.GetItemData(5),
            gain = timeRewardMult * (1 + clearedStages) // 분당 용과: 1 + 1 * 스테이지
        };

        rewardText.text = $"{timeRewardMult}분치 보상 수령";
        
        GameManager.UserData.StartUpdateStream()
            .SetDBTimestamp(lastRewardTime)
            .AddDBValue(goldGain.item.Number, goldGain.gain)
            .AddDBValue(yonggwaGain.item.Number, yonggwaGain.gain)
            .Submit(result =>
            {
                if (false == result)
                {
                    Debug.LogWarning("요청 전송에 실패했습니다");
                    return;
                }

                Debug.Log("방치 보상 수령 성공");
                GameManager.OverlayUIManager.PopupItemGain(new List<ItemGain>()
                {
                    goldGain, yonggwaGain
                });
                // 보상 수령 확인용 팝업 코루틴
                // StartCoroutine(RewardPopupCo());
            });

        if (isIdleReward)
        {
            lastRewardTime.onValueChanged += lastRewardTime_onValueChanged;
            isIdleReward = false;
        }
    }

    private void lastRewardTime_onValueChanged(long arg0)
    {
        isIdleReward = false;
        timerCoroutine = StartCoroutine(TimerTextCo());
    }

    IEnumerator TimerTextCo()
    {
        WaitForSeconds wait1Sec = new WaitForSeconds(1f);
        TimeSpan maxTimer = new TimeSpan(rewardHour, rewardMinute, rewardSecond);

        // 마지막 보상 수령 시점(초 단위 제거)
        DateTime laseReward = lastRewardTime.Value.AddSeconds(-lastRewardTime.Value.Second);
        while (true)
        {
            // 시간 타이머 관련 및 여기서 코루틴 정지 및 시간 정리 해야함
            spanTime = DateTime.Now - laseReward;
            timeText.text = $"{spanTime.Hours}:{spanTime.Minutes}:{spanTime.Seconds}";
            
            // 지난 시간이 최대시간에 도달할 경우
            if (spanTime >= maxTimer)
            {
                spanTime = maxTimer;
                timeText.text = "보상 MAX!";
                yield break;
            }
            yield return wait1Sec;
        }
    }

    IEnumerator RewardPopupCo()
    {
        // 팝업 활성화
        rewardPopup.SetActive(true);
        // 1초뒤
        yield return new WaitForSeconds(1f);
        // 팝업 비활성화
        rewardPopup.SetActive(false);
    }
}