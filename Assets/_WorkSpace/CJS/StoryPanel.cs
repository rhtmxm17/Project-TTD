using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : MonoBehaviour
{
    [SerializeField] IndexedButton chapterButtonPrefab;
    [SerializeField] IndexedButton episodeButtonPrefab;

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
        public List<StoryDirectingData> episodeList;
    }

    [SerializeField] List<ChapterInfo> chapterInfos;

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

        // 기존 챕터의 UI 정리
        foreach (GameObject button in episodeButtonList)
        {
            Destroy(button);
        }
        episodeButtonList.Clear();

        // 선택된 챕터에 따라 UI 셋팅
        ChapterInfo selectedChapter = chapterInfos[index];

        childUIField.mainImage.sprite = selectedChapter.mainTexture;

        for (int i = 0; i < selectedChapter.episodeList.Count; i++)
        {
            IndexedButton buttonInstance = Instantiate(episodeButtonPrefab, childUIField.episodeLayout.transform);
            buttonInstance.Text.text = selectedChapter.episodeList[i].Title;
            buttonInstance.Id = i;
            buttonInstance.Button.onClick.AddListener(() => 
            {
                // TODO: 에피소드 선택시 동작
                Debug.Log($"에피소드 {buttonInstance.Text.text}선택됨");
            });

            episodeButtonList.Add(buttonInstance.gameObject);
        }
    }
}
