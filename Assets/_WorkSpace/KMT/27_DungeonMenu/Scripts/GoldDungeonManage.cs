using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldDungeonManage : MonoBehaviour
{
    [SerializeField]
    Button enterButton;

    [SerializeField]
    List<StageData> stageDatas;

    ItemData goldTicketSO;
    StageSceneChangeArgs sceneChangeArgs = null;
    int stageLevelIdx = 0;

    private void Awake()
    {
        goldTicketSO = DataTableManager.Instance.GetItemData(9/*골드티켓*/);

        enterButton.onClick.AddListener(() => {

            if (goldTicketSO != null && goldTicketSO.Number.Value > 0)
            {
                if (stageLevelIdx < 0 || stageLevelIdx >= stageDatas.Count)
                {
                    Debug.Log("스테이지가 지정되지 않음.");
                    return;
                }

                GameManager.Instance.LoadBattleFormationScene(new StageSceneChangeArgs() 
                {
                    stageData = stageDatas[stageLevelIdx],
                    stageType = StageType.GOLD,
                    prevScene = MenuType.ADVANTURE,
                });
            }
            else
            {
                Debug.Log("골드 티켓이 부족합니다.");
            }

        });
    }

    public void SlideChanged(int idx)
    {
        stageLevelIdx = idx;
    }
}   
