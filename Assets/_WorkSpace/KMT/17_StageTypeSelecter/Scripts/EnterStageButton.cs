using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

[RequireComponent(typeof(Button))]
public class EnterStageButton : MonoBehaviour
{
    [SerializeField]
    protected StageData stageDataSO;
    [SerializeField]
    protected StageType stageType;

    protected Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();

        StageSceneChangeArgs sceneChangeArgs = new StageSceneChangeArgs()
        {
            stageData = stageDataSO,
            stageType = stageType,
            prevScene = MenuType.ADVANTURE,
        };

        button.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadBattleFormationScene(sceneChangeArgs);
        });
    }


}
