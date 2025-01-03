using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EggSpawner : MonoBehaviour
{
    [SerializeField] private bool isEggComplete;
    [SerializeField] private TMP_Text timeText;
    
    // 시간 관련
    private UserDataDateTime lastEggTime; // 마지막으로 보상을 받은 시간
    private TimeSpan time;
    private TimeSpan span;
    private TimeSpan remainingTime; // 보상 충전까지 남은 시간
    [SerializeField] private int rewardTime;
    [SerializeField] private int rewardMinute;
    [SerializeField] private int rewardSeconds;

    // 코루틴 
    private Coroutine timerCoroutine;

    private void Start()
    {
        // 보상 충전 소요시간
        // 필요하면 나중에 변수화 해서 인스펙터에서 수정하도록 변경
        span = new TimeSpan(rewardTime, rewardMinute, rewardSeconds);

    }

    private void OnEnable() => StartEggTimer();

    private void StartEggTimer()
    {
        isEggComplete = false;
        lastEggTime = GameManager.UserData.PlayData.EggGainTimestamp;
        timerCoroutine = StartCoroutine(TimerTextCo());
        lastEggTime.onValueChanged += LastEggTime_onValueChanged;
    }

    private void OnDisable()
    {
        lastEggTime.onValueChanged -= LastEggTime_onValueChanged;
    }

    public void GetEgg()
    {
        if (isEggComplete)
        {
            // TODO: 임시로 100골드 획득시킴
            ItemData gold = GameManager.TableData.GetItemData(1);
            int gain = 100;

            GameManager.UserData.StartUpdateStream()
                .SetDBTimestamp(lastEggTime)
                .AddDBValue(gold.Number, gain)
                .Submit(result =>
                {
                    if (false == result)
                    {
                        Debug.LogWarning("요청 전송에 실패했습니다");
                        return;
                    }

                    Debug.Log("용 부화기 받기 성공!");
                    GameManager.OverlayUIManager.PopupItemGain(new List<ItemGain>()
                    {
                        new ItemGain() { item = gold, gain = gain }
                    });
                });
        }
        else
        {
            // TODO: 못받을때 할 행동들 추가적으로 있으면 작성
            Debug.Log("부화기 받기 실패.");
        }
    }

    private void LastEggTime_onValueChanged(long arg0)
    {
        isEggComplete = false;
        timerCoroutine = StartCoroutine(TimerTextCo());
    }

    IEnumerator TimerTextCo()
    {
        WaitForSeconds wait1Sec = new WaitForSeconds(1f);
        while (true)
        {
            // 실시간 남은시간 표시
            time = DateTime.Now - lastEggTime.Value;
            remainingTime = span - time;
            timeText.text = $"{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}";

            // 준비 완료 상태라면
            if (remainingTime <= TimeSpan.Zero)
            {
                timeText.text = "수령가능";
                isEggComplete = true;
                timerCoroutine = null;
                yield break;
            }
            yield return wait1Sec;
        }
    }
}