using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class AdventureUI : BaseUI
{
    [SerializeField] List<StageData> stages;

    [Header("prefabs")]
    [SerializeField] SimpleInfoPopup popupPrefab;
    [SerializeField] StageButton stageButtonPrefab;

    [Header("child")]
    [SerializeField] LayoutGroup stageButtonGroup;

    [Header("combat Scene Type")]
    [SerializeField]
    StageType stageType;

    protected override void Awake()
    {
        base.Awake();

        // 프로토타입: 직렬화 필드로 등록된 스테이지 데이터로부터 UI 구성하기
        for (int i = 0; i < stages.Count; i++)
        {
            StageButton instance = Instantiate(stageButtonPrefab, stageButtonGroup.transform);
            instance.Id = i;
            instance.Button.onClick.AddListener(() =>
            {
                Debug.Log(instance.Id);
                Popup(stages[instance.Id]);
            });
            instance.Text.text = stages[i].StageName;
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
