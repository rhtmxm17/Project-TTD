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
    //private DateTime lastTime;
    private UserDataDateTime lastEggTime;
    DateTime currentTime;
    private TimeSpan time;
    private TimeSpan span;
    private TimeSpan remainingTime;
    [SerializeField] private int rewardTime;
    [SerializeField] private int rewardMinute;
    [SerializeField] private int rewardSeconds;

    // 코루틴 
    private Coroutine timerCoroutine;

    private void Start()
    {
        UserDataManager.Instance.onLoadUserDataCompleted.AddListener(OnUserDataLoaded);
        StartCoroutine(UserDataManager.InitDummyUser(8));

        // 최종 보상 수령 가능 시간
        // 필요하면 나중에 변수화 해서 인스펙터에서 수정하도록 변경
        span = new TimeSpan(rewardTime, rewardMinute, rewardSeconds);

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Tester();
        }
        
        // 6시간 이하
        if (time < span)
        {
            isEggComplete = false;
        }
        else
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
            timeText.text = "수령가능";
            isEggComplete = true;
        }
    }

    private void OnUserDataLoaded()
    {
        // 초기 시간이 없을 경우 -> 현재시간으로 초기화
        lastEggTime = GameManager.UserData.PlayData.EggGainTimestamp;
        timerCoroutine = StartCoroutine(TimerTextCo());
        lastEggTime.onValueChanged += LastEggTime_onValueChanged;
    }

    public void GetEgg()
    {
        if (isEggComplete)
        {
            // TODO: 임시로 100골드 획득시킴
            UserDataInt gold = GameManager.TableData.GetItemData(1).Number;

            GameManager.UserData.StartUpdateStream()
                .SetDBTimestamp(lastEggTime)
                .SetDBValue(gold, gold.Value + 100)
                .Submit(result =>
                {
                    if (false == result)
                    {
                        Debug.LogWarning("요청 전송에 실패했습니다");
                        return;
                    }

                    Debug.Log("용 부화기 받기 성공!");


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
        while (true)
        {
            // 실시간 남은시간 표시
            currentTime = DateTime.Now;
            time = currentTime - lastEggTime.Value;
            remainingTime = span - time;
            timeText.text = $"{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}";
            yield return new WaitForSeconds(1f);
        }
    }

    public void Tester()
    {
        StopCoroutine(timerCoroutine);
        time += TimeSpan.FromHours(1);
        Debug.Log($"누적시간:{time.Hours}");
    }
}