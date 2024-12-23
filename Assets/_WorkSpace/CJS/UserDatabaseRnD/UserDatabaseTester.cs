using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDatabaseTester : MonoBehaviour
{
    // 테스트 목적의 가상 UID
    // DB 권한 설정 완료시 작동 불가하니 유의
    [SerializeField] string dummyUID = "Dummy";

    [Header("출력용")]
    [SerializeField] float storedTime;
    [SerializeField] float gold;

    private const string TimerReferenceKey = "TestTimer";
    private const string GoldReferenceKey = "TestGold";

    private DatabaseReference dummyUserDBRef;

    private IEnumerator Start()
    {
        // Database 초기화 대기
        yield return new WaitWhile(() => GameManager.Database == null);

        dummyUserDBRef = GameManager.Database.RootReference.Child($"Test/{dummyUID}");

        // 실제 DB 구성시에는 유저 생성시 서버시간 사용할 것으로 예상
        // Firebase.Database.ServerValue.Timestamp;
        dummyUserDBRef.Child(TimerReferenceKey).SetValueAsync(Time.time);

        gold = UnityEngine.Random.Range(0f, 10000f);
        dummyUserDBRef.Child(GoldReferenceKey).SetValueAsync(gold);

        StartCoroutine(UserDataManager.InitDummyUser(0));

        GameManager.Input.actions["Touch"].started += GetUserDataTest;
        GameManager.UserData.onLoadUserDataCompleted.AddListener(TestLog);
    }

    private void GetUserDataTest(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        TestLog();
    }

    private void TestLog()
    {
        Debug.Log(GameManager.UserData.Profile.Name.Value);
        Debug.Log(GameManager.UserData.Profile.Level.Value);
        Debug.Log(GameManager.UserData.Profile.IconIndex.Value);
        Debug.Log(GameManager.UserData.Profile.Introduction.Value);

        Debug.Log(GameManager.Data.GetCharacterData(2).Level.Value);
        Debug.Log(GameManager.Data.GetCharacterData(2).Enhancement.Value);
    }

    [ContextMenu("누적된 보상 수령 테스트")]
    private void GetRewardTest()
    {
        dummyUserDBRef
            .RunTransaction(mutableData =>
            {
                Dictionary<string, object> userDatas = mutableData.Value as Dictionary<string, object>;

                // 마지막 보상 수령으로부터 지난 시간
                float timeInterval;
                if (userDatas.TryGetValue(TimerReferenceKey, out object storedTimeGet))
                {
                    timeInterval = Time.time - ((float)(double)storedTimeGet);
                }
                else
                {
                    Debug.Log("타이머 기록 없음, TODO: 현재 시간 기록");
                    return TransactionResult.Abort(); // 갱신 취소
                }

                userDatas[TimerReferenceKey] = Time.time;

                if (userDatas.TryGetValue(GoldReferenceKey, out object goldGet))
                {
                    // 해당 재화를 이미 갖고있을 경우 누적
                    userDatas[GoldReferenceKey] = timeInterval + ((float)(double)goldGet);
                }
                else
                {
                    // 최초 획득시 키값 또한 추가
                    userDatas[GoldReferenceKey] = timeInterval;
                }

                // 갱신 반영
                Debug.Log("데이터 갱신 요청됨");
                mutableData.Value = userDatas;
                return TransactionResult.Success(mutableData);
            })
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log($"데이터 갱신 실패: {task.Exception}");
                    return;
                }

                Debug.Log($"데이터 갱신 성공");
                Dictionary<string, object> userDatas = task.Result.Value as Dictionary<string, object>;

                storedTime = ((float)(double)userDatas[TimerReferenceKey]);
                gold = ((float)(double)userDatas[GoldReferenceKey]);
            });
    }

    [ContextMenu("출석 보상 수령 테스트")]
    private void TryGetDailyAttendanceReward()
    {
        // 서버에서 날짜를 갱신해서 열어두는 쪽이 훨씬 좋겠지만...
        // 당장은 날짜 확인을 위해 스스로 갱신하자
        dummyUserDBRef.Child("Profile/AttendanceDate").SetValueAsync(ServerValue.Timestamp).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("요청 실패");
                return;
            }

            dummyUserDBRef.Child("Profile").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("요청 실패");
                    return;
                }

                Dictionary<string, object> profile = task.Result.Value as Dictionary<string, object>;
                DateTime attendanceDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime rewardedDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                attendanceDate = attendanceDate.AddMilliseconds((long)profile["AttendanceDate"]);
                Debug.Log(attendanceDate.Date);

                if (profile.ContainsKey("RewardedDate"))
                {
                    rewardedDate = rewardedDate.AddMilliseconds((long)profile["RewardedDate"]);
                }

                // 마지막으로 보상을 수령한 날짜와 현재 날짜가 다르다면
                if (attendanceDate.Date != rewardedDate.Date)
                {

                    Dictionary<string, object> update = new Dictionary<string, object>()
                    {
                        { "Profile/RewardedDate", profile["AttendanceDate"] },
                        { GoldReferenceKey, UnityEngine.Random.Range(100, 1000) } // 갱신 확인용
                    };
                    dummyUserDBRef.UpdateChildrenAsync(update).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            Debug.Log("요청 실패");
                            return;
                        }

                        Debug.Log("일일 보상 획득 성공");
                    });
                }
                else
                {
                    Debug.Log("이미 오늘의 일일 보상을 획득함");
                }
            });
        });
    }
}
