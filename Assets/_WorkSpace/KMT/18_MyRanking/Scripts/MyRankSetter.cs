using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRankSetter : MonoBehaviour
{
    [SerializeField]
    RankBlock rankBlock;

    public void SetRankBlock()
    {

        GameManager.Database.RootReference
            .Child("boss")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {

                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("랭킹 정보 불러오기 실패");
                    return;
                }

                //기록이 있는 경우
                if (task.Result.HasChild(UserData.myUid))
                {

                    GameManager.Database.RootReference
                        .Child("boss")
                        .OrderByChild("score")
                        .StartAt((long)task.Result.Child(UserData.myUid).Child("score").Value)
                        .GetValueAsync()
                        .ContinueWithOnMainThread(task2 => {

                            if (task2.IsFaulted || task2.IsCanceled)
                            {
                                Debug.Log("랭킹 정보 불러오기 실패");
                                return;
                            }

                            rankBlock.SetCounter((int)task2.Result.ChildrenCount);
                            GameManager.UserData.StartUpdateStream()
                                .SetDBValue(GameManager.UserData.Profile.Rank, (int)task2.Result.ChildrenCount)
                                .Submit(result =>
                                {
                                    if (false == result)
                                        Debug.Log($"요청 전송에 실패함");
                                });

                            rankBlock.SetBlockInfo(task.Result.Child(UserData.myUid).Child("nickname").Value.ToString(),
                            (long)task.Result.Child(UserData.myUid).Child("score").Value);

                        });

                }
                else //기록이 없는 경우
                {

                    rankBlock.SetBlockInfo(UserData.myNickname, 0);
                }

            });

    }
}
