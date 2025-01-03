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
        button.onClick.AddListener(() => {

            GameManager.Instance.SetLoadStageType(stageDataSO, stageType);
            SceneManager.LoadSceneAsync("HYJ_BattleFormation");

        });
    }


}
