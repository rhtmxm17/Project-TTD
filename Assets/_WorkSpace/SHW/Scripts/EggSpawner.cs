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
    private DateTime lastTime;
    DateTime currentTime;
    private TimeSpan time;
    private TimeSpan span;
    private TimeSpan remainingTime;
    [SerializeField] private int rewardTime;
    
    // 코루틴 
    private Coroutine timerCoroutine;

    private void Start()
    {
        UserDataManager.InitDummyUser(8);
        
        // 초기 시간이 없을 경우 -> 현재시간으로 초기화
        // TODO: DB에서 시간 가져올 것.
        lastTime = Convert.ToDateTime("2024-12-24 00:00:00");
        
        // 최종 보상 수령 가능 시간
        // 필요하면 나중에 변수화 해서 인스펙터에서 수정하도록 변경
        span = new TimeSpan(rewardTime, 0, 0);

        timerCoroutine = StartCoroutine(TimerTextCo());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Tester();
        }
        
        // 6시간 이하
        if (time <span )
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

    public void GetEgg()
    {
        if (isEggComplete)
        {
            Debug.Log("용 부화기 받기 성공!");
            lastTime = DateTime.Now;
            // TODO: 최종 받은 시간 DB에 보내기
            // 단, 이시간은 최종접속이랑은 따로 돌아가야하는 값
            isEggComplete = false;
            timerCoroutine = StartCoroutine(TimerTextCo());
            // TODO: 뭐든 받아갈 거 받아가시라요
            
            // 초기화(임시)
            time = new TimeSpan(0, 0, 0);
        }
        else
        {
            // TODO: 못받을때 할 행동들 추가적으로 있으면 작성
            Debug.Log("부화기 받기 실패.");
        }
    }

    IEnumerator TimerTextCo()
    {
        while (true)
        {
            // 실시간 남은시간 표시
            currentTime = DateTime.Now;
            time = currentTime - lastTime;
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