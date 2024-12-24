using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] TMP_Text itemName;
    [SerializeField] int itemPrice;
    [SerializeField] Image itemImage;
    [SerializeField] Button buyButton;

    private void Init()
    {
        buyButton.GetComponentInChildren<Button>().onClick.AddListener(Buy);
    }

    private void Start()
    {
        Init();

        // 테스트용 가짜 유저 셋팅
        UserDataManager.InitDummyUser(3);
    }
    private void Buy()
    {
        ItemData gold = GameManager.TableData.GetItemData(1);
        ItemData tocken = GameManager.TableData.GetItemData(2);

        Debug.Log(gold.Number.Value);
        gold.Number.onValueChanged += Number_onValueChanged;

        GameManager.UserData.StartUpdateStream()
            .SetDBValue(gold.Number, gold.Number.Value - 10)
            .SetDBValue(tocken.Number, tocken.Number.Value + 2)
            .Submit(OnComplete);
    }

    private void OnComplete(bool result)
    {
        if (false == result)
        {
            Debug.Log($"네트워크 오류");
            return;
        }

        Debug.Log($"구입 완료");
    }

    private void Number_onValueChanged(long arg0)
    {
        Debug.Log($"골드가 {arg0}개로 바뀜!");
    }
}
