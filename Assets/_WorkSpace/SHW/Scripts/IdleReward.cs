using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IdleReward : MonoBehaviour
{
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
    private TimeSpan maxTimer;
    // 지난시간
    private TimeSpan spanTime;
    
    private Coroutine timerCoroutine;

    private void Start()
    {
        GameManager.UserData.TryInitDummyUserAsync(28, () =>
        {
            Debug.Log("완료");
            StartIdleReward();
        });
        
        // 최대 보상 수령 시간 지정
        maxTimer = new TimeSpan(rewardHour, rewardMinute, rewardSecond);
    }

    private void StartIdleReward()
    {
        lastRewardTime = GameManager.UserData.PlayData.IdleRewardTimestamp;
        timerCoroutine = StartCoroutine(TimerTextCo());
        lastRewardTime.onValueChanged += lastRewardTime_onValueChanged;
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
        spanTime = maxTimer;
        timeText.text = "테스트 가동중!! 즉시 수령 가능!!";
        isIdleReward = true;
    }
    
    // 보상의 받는 부분
    public void GetReward()
    {
        // 보상을 언제든 받을 수 있기에 받을때 한번 코루틴 초기화?
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        // TODO: 시간*100골로 임시 계산
        UserDataInt gold = GameManager.TableData.GetItemData(1).Number;

        int reward = spanTime.Hours * 1000 + spanTime.Minutes * 100 + spanTime.Seconds * 10;
        rewardText.text = $"골드 {reward}수령";
        
        GameManager.UserData.StartUpdateStream()
            .SetDBTimestamp(lastRewardTime)
            .SetDBValue(gold, gold.Value + reward)
            .Submit(result =>
            {
                if (false == result)
                {
                    Debug.LogWarning("요청 전송에 실패했습니다");
                    return;
                }

                Debug.Log("방치 보상 수령 성공");
                // 보상 수령 확인용 팝업 코루틴
                StartCoroutine(RewardPopupCo());
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
        while (true)
        {
            // 시간 타이머 관련 및 여기서 코루틴 정지 및 시간 정리 해야함
            spanTime = DateTime.Now - lastRewardTime.Value;
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