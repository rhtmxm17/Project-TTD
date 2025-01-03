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
    public static void ApplyRank(string bossName, string uid, string nickname, long score, UnityAction onComplteCallback)
    {

        DatabaseReference bossRef = GameManager.Database.RootReference.Child(bossName);

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
            }
            else 
            {

                long prvScore = (long)task.Result.Child($"{uid}/score").Value;
                if (prvScore > score)
                {
                    Debug.Log("기록 경신 실패");
                    GameManager.Instance.StopShortLoadingUI();
                    onComplteCallback?.Invoke();
                    return;
                }

                update[$"{uid}/nickname"] = nickname;
                update[$"{uid}/score"] = score;

            }

            bossRef.UpdateChildrenAsync(update).ContinueWithOnMainThread(t => {
                if (t.IsFaulted || t.IsCanceled)
                {
                    Debug.Log("입력 실패");
                    return;
                }

                GameManager.Instance.StopShortLoadingUI();
                onComplteCallback?.Invoke();
            });

        });

    }

}
