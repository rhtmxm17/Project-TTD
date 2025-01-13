using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdventureUI : BaseUI
{
    [SerializeField] float verticalSpacing;

    [Header("prefabs")]
    [SerializeField] SimpleInfoPopup popupPrefab;
    [SerializeField] IndexedButton stageButtonPrefab;

    [Header("child")]
    [SerializeField] LayoutGroup stageButtonGroup;

    [Header("combat Scene Type")]
    [SerializeField]
    StageType stageType;

    private List<StageData> stagesData;
    private List<GameObject> stageButtons = new List<GameObject>();

    public void SetChapterData(List<StageData> stagesData)
    {
        this.stagesData = stagesData;
        foreach (GameObject oldButton in stageButtons)
        {
            Destroy(oldButton);
        }
        stageButtons.Clear();

        bool isOdd = true;

        for (int i = 0; i < stagesData.Count; i++)
        {
            GameObject buttonHolder = new GameObject($"_{i}", typeof(RectTransform));
            buttonHolder.GetComponent<RectTransform>().SetParent(stageButtonGroup.transform);

            IndexedButton instance = Instantiate(stageButtonPrefab, buttonHolder.transform);
            RectTransform rt = instance.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = Vector2.one * 0.5f;
            rt.anchoredPosition = new Vector2(0f, isOdd ? -verticalSpacing : verticalSpacing);

            instance.Id = i;

            if (this.stagesData[instance.Id].IsOpened)
            {
                instance.Button.interactable = true;
            }
            else
            {
                instance.Button.interactable = false;
            }

            instance.Button.onClick.AddListener(() =>
            {
                Debug.Log(instance.Id);
                Popup(this.stagesData[instance.Id]);
            });
            instance.Text.text = stagesData[i].StageName;

            stageButtons.Add(buttonHolder);
            isOdd = !isOdd;
        }
    }

    public void SetChapterData(StageData firstStagesDataOfChapter)
    {
        foreach (GameObject oldButton in stageButtons)
        {
            Destroy(oldButton);
        }
        stageButtons.Clear();

        if (firstStagesDataOfChapter == null)
        {
            Debug.Log("첫 스테이지가 잘못 지정됨.");
            return;
        }

        bool isOdd = true;
        int idx = 0;

        while (firstStagesDataOfChapter != null)
        {
            GameObject buttonHolder = new GameObject($"_{idx}", typeof(RectTransform));
            buttonHolder.GetComponent<RectTransform>().SetParent(stageButtonGroup.transform);

            IndexedButton instance = Instantiate(stageButtonPrefab, buttonHolder.transform);
            RectTransform rt = instance.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = Vector2.one * 0.5f;
            rt.anchoredPosition = new Vector2(0f, isOdd ? -verticalSpacing : verticalSpacing);

            instance.Id = idx;
            instance.SetStageData(firstStagesDataOfChapter);

            if (firstStagesDataOfChapter.IsOpened)
            {
                instance.Button.interactable = true;
            }
            else
            {
                instance.Button.interactable = false;
            }

            instance.Button.onClick.AddListener(() =>
            {
                Debug.Log(instance.Id);
                Popup(instance.StageData);
            });
            instance.Text.text = firstStagesDataOfChapter.StageName;

            stageButtons.Add(buttonHolder);
            isOdd = !isOdd;

            idx++;

            firstStagesDataOfChapter = DataTableManager.Instance.GetStageData(firstStagesDataOfChapter.NextStageId);
            if (firstStagesDataOfChapter == null)
                break;

        }

    }

    public void ForceOpen()
    {
        Debug.LogWarning("테스트 모드 전용 메서드 호출됨");
        foreach (GameObject stageButton in stageButtons)
        {
            stageButton.GetComponentInChildren<IndexedButton>().Button.interactable = true;
        }
    }

    private void Popup(StageData data)
    {
        SimpleInfoPopup instance = Instantiate(popupPrefab, GameManager.PopupCanvas);
        instance.Title.text = data.StageName;

        StageSceneChangeArgs sceneChangeArgs = new StageSceneChangeArgs()
        {
            stageData = data,
            stageType = stageType,
            prevScene = MenuType.ADVANTURE,
        };

        instance.SubmitButton.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadBattleFormationScene(sceneChangeArgs);
        });
    }
}
