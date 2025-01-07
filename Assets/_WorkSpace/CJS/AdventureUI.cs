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
    GameManager.StageType stageType;

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

    private void Popup(StageData data)
    {
        SimpleInfoPopup instance = Instantiate(popupPrefab, GameManager.PopupCanvas);
        instance.Title.text = data.StageName;

        instance.SubmitButton.onClick.AddListener(() => {
            GameManager.Instance.SetLoadStageType(data, stageType);
            SceneManager.LoadSceneAsync("HYJ_BattleFormation");
        });
    }
}
