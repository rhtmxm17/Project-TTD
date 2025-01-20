using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DailyBonus : MonoBehaviour
{

    const int REFILL_GOLDTICKET_COUNT = 3;
    const int REFILL_FriendTicket_COUNT = 10;
    // 토큰_강화, 일반_강화, 레벨업 => 5회 / 1일
    const int REFILL_TOKEN_COUNT = 5;

    public void OnClick()
    {
        ItemData goldTicket = DataTableManager.Instance.GetItemData(9/*골드티켓*/);
        ItemData friendTicket = DataTableManager.Instance.GetItemData(10/*친구방문 보상 수령 카운트*/);
        
        
        // 머리가 일단 잘 안돌아가고 있고 다른것도 해야되니 대충
        #region 상점아이템 일일리셋
        ShopItemData lvTocken = DataTableManager.Instance.GetShopItemData(2);
        ShopItemData genTocken = DataTableManager.Instance.GetShopItemData(3);
        ShopItemData dragonCandy1 = DataTableManager.Instance.GetShopItemData(4);
        ShopItemData dragonCandy2 = DataTableManager.Instance.GetShopItemData(5);
        ShopItemData dragonCandy3 = DataTableManager.Instance.GetShopItemData(6);
        ShopItemData dragonCandy4 = DataTableManager.Instance.GetShopItemData(7);
        ShopItemData dragonCandy5 = DataTableManager.Instance.GetShopItemData(8);
        ShopItemData dragonCandy6 = DataTableManager.Instance.GetShopItemData(9);
        ShopItemData dragonCandy7 = DataTableManager.Instance.GetShopItemData(10);
        ShopItemData dragonCandy8 = DataTableManager.Instance.GetShopItemData(11);
        ShopItemData dragonCandy9 = DataTableManager.Instance.GetShopItemData(12);
        #endregion
       

        UserDataManager.UpdateDbChain updateChain = GameManager.UserData.StartUpdateStream();

        // 팝업에 표시될 출석 보상
        // TODO : 일일보상 직렬화?
        List<ItemGain> gainList = new List<ItemGain>()
        {
            new ItemGain() { item = DataTableManager.Instance.GetItemData(1), gain = 5000 }, // 5000 골드
            new ItemGain() { item = DataTableManager.Instance.GetItemData(4), gain = 100 }, // 100 용철석  (이전:10 용젤리)
        };

        foreach (ItemGain gain in gainList)
        {
            updateChain.AddDBValue(gain.item.Number, gain.gain);
        }

        // 입장 횟수 아이템 충전
        if (goldTicket.Number.Value < 3)
            updateChain.SetDBValue(goldTicket.Number, REFILL_GOLDTICKET_COUNT);

        if(friendTicket.Number.Value < 10)
            updateChain.SetDBValue(friendTicket.Number, REFILL_FriendTicket_COUNT);
        
        if (lvTocken.Bought.Value != 0)
            updateChain.SetDBValue(lvTocken.Bought, 0);
        if (genTocken.Bought.Value != 0)
            updateChain.SetDBValue(genTocken.Bought, 0);
        if (dragonCandy1.Bought.Value != 0) updateChain.SetDBValue(dragonCandy1.Bought, 0);
        if (dragonCandy2.Bought.Value != 0) updateChain.SetDBValue(dragonCandy2.Bought, 0);
        if (dragonCandy3.Bought.Value != 0) updateChain.SetDBValue(dragonCandy3.Bought, 0);
        if (dragonCandy4.Bought.Value != 0) updateChain.SetDBValue(dragonCandy4.Bought, 0);
        if (dragonCandy5.Bought.Value != 0) updateChain.SetDBValue(dragonCandy5.Bought, 0);
        if (dragonCandy6.Bought.Value != 0) updateChain.SetDBValue(dragonCandy6.Bought, 0);
        if (dragonCandy7.Bought.Value != 0) updateChain.SetDBValue(dragonCandy7.Bought, 0);
        if (dragonCandy8.Bought.Value != 0) updateChain.SetDBValue(dragonCandy8.Bought, 0);
        if (dragonCandy9.Bought.Value != 0) updateChain.SetDBValue(dragonCandy9.Bought, 0);


        
        
        /// 상점아이템 구매횟수 초기화 추가
        // 구매횟수가 최대수치보다 작거나 같으면, 회숫 0으로 변경
    //    for (int i = 0; i < chargeToekn.Length; i++)
    //    {
    //        updateChain.SetDBValue(chargeToekn[i].Bought, 0);
    //    }


        updateChain.SetDBValue<object>("friends/visitedList", null);
        updateChain.SetDBValue("PlayData/DayCount", UserData.connectedDayCount);

        updateChain
            .Submit((result) =>
            {
                if (result)
                {
                    Debug.Log("일일보상처리 완료");
                    var popUp = GameManager.OverlayUIManager.PopupItemGain(gainList);
                    popUp.onPopupClosed += GameManager.Instance.LoadLobbyScene;
                    popUp.Title.text = "로그인 보너스 획득!";
                }
                else 
                {
                    Debug.Log("일일보상 획득 실패");
                }
            });
    }
}
