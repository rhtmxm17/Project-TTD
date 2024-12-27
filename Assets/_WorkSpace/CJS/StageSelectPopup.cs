using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectPopup : MonoBehaviour
{
    [SerializeField] Button backgroundButton;
    [SerializeField] TMP_Text stageName;
    [SerializeField] Button enterButton;
    private StageData stageData;

    private void Awake()
    {
        backgroundButton.onClick.AddListener(() => Destroy(this.gameObject));
    }

    public void Initialize(StageData stageData)
    {
        this.stageData = stageData;
        stageName.text = stageData.name;
    }
}
