using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DailyBonus : MonoBehaviour
{

    const int REFILL_GOLDTICKET_COUNT = 3;

    public void OnClick()
    {

        ItemData goldTicket = DataTableManager.Instance.GetItemData(9/*골드티켓*/);

        //TODO : 일일보상 갱신조건등등을 여기에 추가
        if (goldTicket.Number.Value < 3)
        {
            //부족하니까 티켓을 채움

            //TODO : 일일보상 여기에 추가
            GameManager.UserData.StartUpdateStream()
                .SetDBValue(goldTicket.Number, REFILL_GOLDTICKET_COUNT)
                .Submit((result) =>
                {
                    if (result)
                    {
                        Debug.Log("골드티켓 획득 성공");
                        SceneManager.LoadSceneAsync("MainMenu");
                    }
                    else 
                    {
                        Debug.Log("골드티켓 획득 실패");
                    }
                });

        }
        else
        {
            Debug.Log("이미 3개 이상 있음");
            SceneManager.LoadSceneAsync("MainMenu");
        }

    }

}
