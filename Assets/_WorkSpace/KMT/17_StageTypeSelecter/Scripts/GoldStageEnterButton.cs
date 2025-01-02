using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoldStageEnterButton : EnterStageButton
{
    protected override void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => {

            ItemData goldTicketSO = DataTableManager.Instance.GetItemData(9/*골드티켓*/);

            if (goldTicketSO != null && goldTicketSO.Number.Value > 0)
            {
                GameManager.Instance.SetLoadStageType(stageDataSO, stageType);
                SceneManager.LoadSceneAsync("HYJ_BattleFormation");
            }
            else
            {
                Debug.Log("골드 티켓이 부족합니다.");
            }

        });
    }
}
