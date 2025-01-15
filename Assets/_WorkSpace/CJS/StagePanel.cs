using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{
    [SerializeField] OutskirtsUI outskirts; // 뒤로가기 버튼에 스택 등록
    [SerializeField] AdventureUI chapterPanel; // 챕터 선택시 열릴 UI
    [SerializeField] List<ChapterSet> chapters; // 챕터 버튼 및 스테이지 정보 목록

    [SerializeField] ScrollRect worldMapScroll;
    [SerializeField] RectTransform worldMapPanel;
    [SerializeField] LayoutGroup leadButtonGroup; // 바로가기 버튼이 놓일 곳
    [SerializeField] IndexedButton chapterLeadButtonPrefab; // 바로가기 버튼

    [System.Serializable]
    private struct ChapterSet
    {
        // 월드맵 상의 챕터 버튼
        public Button enterButton;

        // 해당 챕터의 구성 스테이지
        public List<StageData> stageDatas;
    }

    private void Awake()
    {
        int i = 0;
        foreach (var chapter in chapters)
        {
            chapter.enterButton.onClick.AddListener(() => OpenChapterPanel(chapter.stageDatas));

            // 바로가기 버튼 생성
            IndexedButton instance = Instantiate(chapterLeadButtonPrefab, leadButtonGroup.transform);
            instance.Id = i;
            instance.Button.onClick.AddListener(() =>
            {
                // 바로가기 버튼 클릭시 id번째 챕터 버튼의 위치로 월드맵 이동
                SnapTo(chapter.enterButton.GetComponent<RectTransform>());
            });

            instance.Text.text = (i + 1).ToString();
            i++;
        }
    }

    private void OpenChapterPanel(List<StageData> stageDatas)
    {
        // 챕터 창 열 때 스테이지 정보 등록
        //chapterPanel.SetChapterData(stageDatas);
        chapterPanel.SetChapterData(stageDatas[0]);// 첫 스테이지 기준으로 체이닝 하여 스테이지를 등록하는 함수.
        chapterPanel.gameObject.SetActive(true);
        outskirts.UIStack.Push(chapterPanel.gameObject);
    }


    public void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 moveTo = (Vector2)worldMapScroll.transform.InverseTransformPoint(worldMapPanel.position)
            - (Vector2)worldMapScroll.transform.InverseTransformPoint(target.position);

        worldMapPanel.DOAnchorPos(moveTo, 0.8f);
    }
}
