using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class AdventureUI : BaseUI
{
    [SerializeField] List<StageData> stages;

    [Header("prefabs")]
    [SerializeField] SimpleInfoPopup popupPrefab;
    [SerializeField] StageButton stageButtonPrefab;

    [Header("child")]
    [SerializeField] LayoutGroup stageButtonGroup;

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

        // TODO: 씬 전환 대신 편성창 열기
        instance.SubmitButton.onClick.AddListener(() => GameManager.Instance.LoadStageScene(data));
    }
}
