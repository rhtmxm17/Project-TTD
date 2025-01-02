using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;

public static class DailyChecker
{

    const long BASE_TIME = 1735689600000;//한국기준 2025/1/1/ 00 : 00
    const long KOREA_OFFSET_9 = 32400000;//한국시간 동기화를 위한 9시간단위 밀리초
    const long ONE_DAY = 86400000;//24시간 밀리초

    public static void IsTodayFirstConnect(Action<bool> callback)
    {

        BackendManager.UserDataRef.Child(UserData.myUid)
            .Child("PlayData")
            .Child("EnterTime")
            .SetValueAsync(ServerValue.Timestamp)
            .ContinueWithOnMainThread(t1 =>
            {

                if (t1.IsCanceled || t1.IsFaulted)
                {
                    Debug.Log("타임스탬프 기록 실패");
                    return;
                }

                BackendManager.UserDataRef.Child(UserData.myUid)
                    .Child("PlayData")
                    .GetValueAsync()
                    .ContinueWithOnMainThread(t2 =>
                    {

                        if (t2.IsCanceled || t2.IsFaulted)
                        {
                            Debug.Log("타임스탬프 읽어오기 실패");
                            return;
                        }

                        long curTime;
                        long lastJoindDayCount = -1;

                        curTime = (long)t2.Result.Child("EnterTime").Value;
                        curTime += KOREA_OFFSET_9;//한국기준을 맞추기 위해 9시간을 더함

                        DateTime utcDateTime = DateTimeOffset.FromUnixTimeMilliseconds(curTime).UtcDateTime;
                        Debug.Log($"한국 시간: {utcDateTime:yyyy-MM-dd HH:mm:ss}");

                        Debug.Log((curTime - BASE_TIME) + " : 경과시간");//25년 1월 1일 0시 0분 기준 경과시간 계산
                        Debug.Log((curTime - BASE_TIME) / ONE_DAY + " : 경과일수");//하루 밀리초로 나눔 => 25/1/1 00 : 00부터 몇일 지났는지 계산

                        lastJoindDayCount = (curTime - BASE_TIME) / ONE_DAY;//마지막 접속 날짜 카운트 도출

                        //접속 기록이 있는경우.
                        if (t2.Result.HasChild("DayCount"))
                        {
                            //같은날에 이전 접속 기록이 있는 경우 false 콜백 호출
                            if ((long)t2.Result.Child("DayCount").Value >= lastJoindDayCount)
                            {
                                callback?.Invoke(false);
                                return;
                            }
                        }

                        //당일 첫 접속인경우 기록 및 true 콜백 호출
                        BackendManager.UserDataRef.Child(UserData.myUid)
                            .Child("PlayData")
                            .Child("DayCount")
                            .SetValueAsync(lastJoindDayCount)
                            .ContinueWithOnMainThread(t3 => {

                                if (t3.IsCanceled || t3.IsFaulted)
                                {
                                    Debug.Log("접속기록 작성 실패");
                                    return;
                                }

                                callback?.Invoke(true);

                            });

                    });

            });

    }
}
