using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingBoard : BaseUI
{
    [SerializeField]
    RankBlock rankBlockPrefab;
    [SerializeField]
    int displayRankblockCnt;
    [SerializeField]
    Transform contentTransform;

    List<RankBlock> rankBlocks = new List<RankBlock>();

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < displayRankblockCnt; i++)
        {
            rankBlocks.Add(Instantiate(rankBlockPrefab, contentTransform).SetCounter(i + 1));
        }
    }

    private void OnEnable()
    {
        GameManager.Database.RootReference.Child("boss").ValueChanged += Refresh;
    }

    private void OnDisable()
    {
        GameManager.Database.RootReference.Child("boss").ValueChanged -= Refresh;
    }

    [ContextMenu("add")]
    void Add()
    {
        RankApplier.ApplyRank("boss", "d", "jkl", 12);
    }

    /// <summary>
    /// 랭킹표 변경시 표 새로고침하는 함수.
    /// </summary>
    void Refresh(object obj, ValueChangedEventArgs args)
    {
        GameManager.Database.RootReference.Child("boss").OrderByChild("score").LimitToLast(10).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {

                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("랭킹 정보 불러오기 실패");
                    return;
                }

                foreach (RankBlock block in rankBlocks)
                {
                    block.gameObject.SetActive(false);
                }

                int cnt = (int)task.Result.ChildrenCount - 1;

                foreach (DataSnapshot item in task.Result.Children)
                {
                    rankBlocks[cnt].gameObject.SetActive(true);
                    rankBlocks[cnt].SetBlockInfo(item.Child("nickname").Value.ToString(), (long)item.Child("score").Value);
                    cnt--;
                }

            });
    }

}
