using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class GoldDungeonManage : MonoBehaviour
{
    [SerializeField]
    Button enterButton;

    [SerializeField]
    Button skipButton;

    [SerializeField]
    List<StageData> stageDatas;

    [SerializeField]
    StageData firstStageDataOfDungeon;

    ItemData goldTicketSO;
    int stageLevelIdx = 0;

    private void Awake()
    {
        goldTicketSO = DataTableManager.Instance.GetItemData(9/*골드티켓*/);

        stageDatas = new List<StageData>();
        if (firstStageDataOfDungeon == null)
        {
            Debug.Log("1단계 스테이지가 없음.");
            return;
        }

        stageDatas.Add(firstStageDataOfDungeon);

        //다음 스테이지가 존재한다면 순서대로 던전 스테이지를 추가.
        while (firstStageDataOfDungeon.NextStageId != -1)
        {
            firstStageDataOfDungeon = DataTableManager.Instance.GetStageData(firstStageDataOfDungeon.NextStageId);
            if (firstStageDataOfDungeon == null)
                break;
            stageDatas.Add(firstStageDataOfDungeon);
        }


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
                    dungeonLevel = stageLevelIdx,
                });
            }
            else
            {
                Debug.Log("골드 티켓이 부족합니다.");
            }

        });

        skipButton.onClick.AddListener(() => {

            if (goldTicketSO != null && goldTicketSO.Number.Value > 0)
            {
                if (stageLevelIdx < 0 || stageLevelIdx >= stageDatas.Count)
                {
                    Debug.Log("스테이지가 지정되지 않음.");
                    return;
                }

                //보상이 골드 하나임을 상정, 여러개일 경우 확장 필요
                ItemGain goldGain = stageDatas[stageLevelIdx].Reward[0];

                GameManager.UserData.StartUpdateStream()
                    .AddDBValue(goldGain.item.Number, goldGain.gain)
                    .AddDBValue(DataTableManager.Instance.GetItemData(9/*골드티켓*/).Number, -1)
                    .Submit(result => {

                        if (!result)
                        {
                            Debug.Log("데이터 갱신 실패!");
                            return;
                        }

                        Debug.Log("와! 골드!");

                        // 아이템 획득 팝업 + 확인 클릭시 메인 화면으로
                        ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(stageDatas[stageLevelIdx].Reward);
                        popupInstance.Title.text = "와! 골드!";

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
