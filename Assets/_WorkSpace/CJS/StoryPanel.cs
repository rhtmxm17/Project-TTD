using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] IndexedButton chapterButtonPrefab;
    [SerializeField] IndexedButton episodeButtonPrefab;
    [SerializeField] SimpleInfoPopup episodeEnterPopupPrefab;
    [SerializeField] StoryDirector storyDirectorPrefab;

    [System.Serializable]
    private struct ChildUIField
    {
        public Image mainImage;
        public LayoutGroup chapterLayout;
        public LayoutGroup episodeLayout;
    }
    [SerializeField] ChildUIField childUIField;

    [System.Serializable]
    private class ChapterInfo
    {
        public Sprite mainTexture;
        public string chapterName;
        public List<StageData> episodeList;
    }

    [SerializeField] List<ChapterInfo> chapterInfos;

    private int selectedChapterIndex;

    private List<GameObject> episodeButtonList = new List<GameObject>();

    private void Awake()
    {
        // 챕터 선택 버튼 생성 및 챕터 선택 함수 등록
        for (int i = 0; i < chapterInfos.Count; i++)
        {
            IndexedButton buttonInstance = Instantiate(chapterButtonPrefab, childUIField.chapterLayout.transform);
            buttonInstance.Text.text = chapterInfos[i].chapterName;
            buttonInstance.Id = i;
            buttonInstance.Button.onClick.AddListener(() => SelectChapter(buttonInstance.Id));
        }
    }

    private void SelectChapter(int index)
    {
        Debug.Log($"챕터 선택됨:{index}");
        selectedChapterIndex = index;

        // 기존 챕터의 UI 정리
        foreach (GameObject button in episodeButtonList)
        {
            Destroy(button);
        }
        episodeButtonList.Clear();

        // 선택된 챕터에 따라 UI 셋팅
        ChapterInfo selectedChapterInfo = chapterInfos[index];

        childUIField.mainImage.sprite = selectedChapterInfo.mainTexture;

        for (int i = 0; i < selectedChapterInfo.episodeList.Count; i++)
        {
            IndexedButton buttonInstance = Instantiate(episodeButtonPrefab, childUIField.episodeLayout.transform);
            buttonInstance.Text.text = selectedChapterInfo.episodeList[i].ButtonName;
            buttonInstance.Id = i;
            buttonInstance.Button.onClick.AddListener(() => 
            {
                // TODO: 에피소드 선택시 동작
                Debug.Log($"에피소드 {buttonInstance.Text.text}선택됨");
                OnChapterEpisodeButtonClicked(buttonInstance.Id);
            });

            episodeButtonList.Add(buttonInstance.gameObject);
        }
    }

    /// <summary>
    /// 에피소드 목록에서 에피소드 버튼 클릭시 호출되는 함수
    /// </summary>
    /// <param name="episodeIndex">해당 에피소드 번호</param>
    private void OnChapterEpisodeButtonClicked(int episodeIndex)
    {
        StageData episodeData = chapterInfos[selectedChapterIndex].episodeList[episodeIndex];

        // 에피소드 정보 팝업 생성 및 정보 등록
        SimpleInfoPopup popupInstance = Instantiate(episodeEnterPopupPrefab, GameManager.PopupCanvas);
        popupInstance.Title.text = episodeData.StageName;
        popupInstance.Description.text = episodeData.Description;
        if (null != episodeData.SpriteImage) // null이라면 프리펩에 있는 스프라이트(기본값)를 그대로 사용
            popupInstance.Image.sprite = episodeData.SpriteImage;
        popupInstance.SubmitButton.onClick.AddListener(() => EnterEpisode(episodeData)); // 확인 버튼 클릭시 에피소드 진입 함수 호출
    }

    /// <summary>
    /// 실제로 에피소드 진입이 이뤄지는 함수
    /// </summary>
    /// <param name="episodeData">에피소드 데이터</param>
    private void EnterEpisode(StageData episodeData)
    {
        if (null == episodeData.PreStory)
        {
            // 선행 스토리가 없다면 바로 스테이지 단계로 이동
            EnterStage(episodeData);
        }
        else
        {
            // 선행 스토리가 있다면 재생 및 종료시 편성창 진입 등록
            StoryDirector dircetorInstance = Instantiate(storyDirectorPrefab);
            dircetorInstance.SetDirectionData(episodeData.PreStory);
            dircetorInstance.onStoryCompleted += () => EnterStage(episodeData);
        }
    }

    /// <summary>
    /// 몬스터가 있다면 편성 창으로 진입, 없다면 바로 보상 획득
    /// </summary>
    /// <param name="episodeData">에피소드 데이터</param>
    private void EnterStage(StageData episodeData)
    {
        if (episodeData.Waves.Count > 0)
        {
            // 스테이지에 몬스터가 있다면 편성창 진입

            // 스테이지 씬 변경 정보 설정
            StageSceneChangeArgs sceneChangeArgs = new StageSceneChangeArgs()
            {
                stageType = StageType.STORY,
                prevScene = MenuType.STORY,
                stageData = episodeData,
            };

            GameManager.Instance.LoadBattleFormationScene(sceneChangeArgs);
        }
        else
        {
            // 스테이지에 몬스터가 없다면 보상 획득 후 종료
            bool isFirst = (episodeData.ClearCount.Value == 0);

            episodeData.UserGetRewardOnceAsync(result =>
            {
                if (false == result)
                {
                    Debug.Log("요청 전송에 실패했습니다");
                    return;
                }

                if (isFirst)
                {
                    // 아이템 획득 팝업
                    ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(episodeData.Reward);
                    popupInstance.Title.text = "에피소드 클리어!";
                }
            });
        }
    }
}
