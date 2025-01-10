using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageTester : MonoBehaviour
{
    [SerializeField] OutskirtsUI outskirts; // 뒤로가기 버튼에 스택 등록
    [SerializeField] AdventureUI chapterPanel; // 챕터 선택시 열릴 UI

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OpenTesterChapterPanel);
    }

    private void OpenTesterChapterPanel()
    {
        // 존재하는 모든 스테이지 데이터 등록
        chapterPanel.SetChapterData(new List<StageData>(GameManager.TableData.StageDataList));
        chapterPanel.gameObject.SetActive(true);
        outskirts.UIStack.Push(chapterPanel.gameObject);
        chapterPanel.ForceOpen();
    }
}
