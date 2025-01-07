using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DailyBonus : MonoBehaviour
{

    const int REFILL_GOLDTICKET_COUNT = 3;
    const int REFILL_FriendTicket_COUNT = 10;

    public void OnClick()
    {

        ItemData goldTicket = DataTableManager.Instance.GetItemData(9/*골드티켓*/);
        ItemData friendTicket = DataTableManager.Instance.GetItemData(10/*친구방문 보상 수령 카운트*/);

        UserDataManager.UpdateDbChain updateChain = GameManager.UserData.StartUpdateStream();

        //TODO : 일일보상 여기에 추가
        if (goldTicket.Number.Value < 3)
            updateChain.SetDBValue(goldTicket.Number, REFILL_GOLDTICKET_COUNT);

        if(friendTicket.Number.Value < 10)
            updateChain.SetDBValue(friendTicket.Number, REFILL_FriendTicket_COUNT);

        updateChain.SetDBValue<object>("friends/visitedList", null);

        updateChain
            .Submit((result) =>
            {
                if (result)
                {
                    Debug.Log("일일보상처리 완료");
                    SceneManager.LoadSceneAsync("MainMenu");
                }
                else 
                {
                    Debug.Log("일일보상 획득 실패");
                }
            });

        

    }

}
