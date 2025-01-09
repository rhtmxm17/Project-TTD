using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryStageManager : StageManager
{
    [SerializeField] StoryDirector storyDirectorPrefab;

    protected override void OnClear()
    {
        if (IsCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        IsCombatEnd = true;

        Debug.Log("클리어!");

        // 보상 획득 후 종료
        bool isFirst = (stageDataOnLoad.ClearCount.Value == 0);

        stageDataOnLoad.UserGetRewardOnceAsync(result =>
        {
            if (false == result)
            {
                Debug.Log("요청 전송에 실패했습니다");
                return;
            }

            if (isFirst)
            {
                // 아이템 획득 팝업
                ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(stageDataOnLoad.Reward);
                popupInstance.Title.text = "에피소드 클리어!";
                popupInstance.onPopupClosed += PlayPostStory; // 닫히면 후 스토리 단계로
            }
            else
            {
                // 획득할 아이템이 없다면 바로 후 스토리
                PlayPostStory();
            }
        });

    }

    private void PlayPostStory()
    {
        if (null == stageDataOnLoad.PostStory)
        {
            // 클리어 후 스토리가 없다면 이전 씬으로
            LoadPreviousScene();
        }
        else
        {
            // 클리어 후 스토리가 있다면 재생 및 종료시 이전 씬으로
            StoryDirector dircetorInstance = Instantiate(storyDirectorPrefab);
            dircetorInstance.SetDirectionData(stageDataOnLoad.PostStory);
            dircetorInstance.onStoryCompleted += LoadPreviousScene;
        }
    }
}
