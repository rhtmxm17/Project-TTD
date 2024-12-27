using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryUI : BaseUI
{
    [Header("prefabs")]
    [SerializeField] StoryDirector directorPrefab;
    [SerializeField] StageButton stageButtonPrefab;

    [Header("child")]
    [SerializeField] LayoutGroup stageButtonGroup;

    protected override void Awake()
    {
        base.Awake();

        // 프로토타입: 스토리 데이터 전부 가져오기
        var storys = GameManager.TableData.StoryDirectingDataList;

        for (int i = 0; i < storys.Count; i++)
        {
            StageButton instance = Instantiate(stageButtonPrefab, stageButtonGroup.transform);
            instance.Id = i;
            instance.Button.onClick.AddListener(() =>
            {
                StoryDirector dircetorInstance = Instantiate(directorPrefab);
                dircetorInstance.SetDirectionData(storys[instance.Id]);
            });

            instance.Text.text = storys[i].name;
        }
    }
}
