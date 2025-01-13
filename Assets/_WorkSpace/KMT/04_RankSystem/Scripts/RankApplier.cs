using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class RankApplier
{
    /// <summary>
    /// 점수 적용, 더 적은 점수일경우 적용하지 않음.
    /// </summary>
    /// <param name="bossName"></param>
    /// <param name="uid"></param>
    /// <param name="nickname"></param>
    /// <param name="score"></param>
    /// <param name="onComplteCallback">완료 후 콜백, 기록 경신일 경우 true를 전달, 현재 등수를 전달</param>
    public static void ApplyRank(string bossName, string uid, string nickname, long score, UnityAction<bool, int> onComplteCallback)
    {

        DatabaseReference bossRef = GameManager.Database.RootReference.Child(bossName);
        bool isNewRecord = false;

        GameManager.Instance.StartShortLoadingUI();
        bossRef.GetValueAsync().ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("랭킹 데이터 읽어오기 실패");
                GameManager.Instance.StopShortLoadingUI();
                return;
            }

            Dictionary<string, object> update = new Dictionary<string, object>();

            if (!task.Result.HasChild(uid))//첫 데이터 기록
            {
                update[$"{uid}/nickname"] = nickname;
                update[$"{uid}/score"] = score;
                isNewRecord = true;
            }
            else 
            {

                long prvScore = (long)task.Result.Child($"{uid}/score").Value;
                if (prvScore > score)
                {
                    Debug.Log("기록 경신 실패");
                    GameManager.Instance.StopShortLoadingUI();
                    isNewRecord = false;
                }
                else
                {
                    update[$"{uid}/nickname"] = nickname;
                    update[$"{uid}/score"] = score;
                    isNewRecord = true;
                }

            }

            if (isNewRecord)
            {
                bossRef.UpdateChildrenAsync(update).ContinueWithOnMainThread(task2 =>
                {
                    ;
                    if (task2.IsFaulted || task2.IsCanceled)
                    {
                        Debug.Log("입력 실패");
                        return;
                    }

                    ;



                    bossRef
                        .OrderByChild("score")
                        .StartAt(score)
                        .GetValueAsync()
                        .ContinueWithOnMainThread(task3 =>
                        {
                            ;
                            if (task3.IsFaulted || task3.IsCanceled)
                            {
                                Debug.Log("랭킹 정보 불러오기 실패");
                                return;
                            }
                            ;
                            onComplteCallback?.Invoke(isNewRecord, (int)task3.Result.ChildrenCount);
                            GameManager.Instance.StopShortLoadingUI();

                        });

                });
            }
            else
            {
                GameManager.Database.RootReference
                .Child(bossName)
                .OrderByChild("score")
                .StartAt((long)task.Result.Child(UserData.myUid).Child("score").Value)
                .GetValueAsync()
                .ContinueWithOnMainThread(task2 => {

                    if (task2.IsFaulted || task2.IsCanceled)
                    {
                        Debug.Log("랭킹 정보 불러오기 실패");
                        return;
                    }

                    onComplteCallback?.Invoke(isNewRecord, (int)task2.Result.ChildrenCount);
                    GameManager.Instance.StopShortLoadingUI();

                });
            }

        });

    }

}
