using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTimer : MonoBehaviour
{
    // 토큰_강화, 일반_강화, 레벨업 => 5회 / 1일
    // 패키지 1,2,3                 => 1회 / 1달
    const int REFILL_TOKEN_COUNT = 5;
    const int REFILL_PACKAGE_COUNT = 1;

    public void OnClick()
    {
        // 갱신될 아이템 가져오기
        ShopItemData enTocken = DataTableManager.Instance.GetShopItemData(1);
        ShopItemData lvTocken = DataTableManager.Instance.GetShopItemData(2);
        ShopItemData genTocken = DataTableManager.Instance.GetShopItemData(3);
        
        UserDataManager.UpdateDbChain updateChain = GameManager.UserData.StartUpdateStream();
        
        // 구매횟수가 최대수치보다 작거나 같으면, 회숫 0으로 변경
        if (enTocken.Bought.Value <= REFILL_TOKEN_COUNT)
            updateChain.SetDBValue(enTocken.Bought, 0);
        if (lvTocken.Bought.Value <= REFILL_TOKEN_COUNT)
            updateChain.SetDBValue(enTocken.Bought, 0);
        if (genTocken.Bought.Value <= REFILL_TOKEN_COUNT)
            updateChain.SetDBValue(enTocken.Bought, 0);

    
        
    }
    

    
}
