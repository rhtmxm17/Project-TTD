using System;
using System.Collections;
using System.Collections.Generic;
// using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopItem : BaseUI
{
    [Header("아이템 이름")]
    [SerializeField] TMP_Text itemNameText;     // UI_아이템이름
    [SerializeField] public string ShopItemName;

    [Header("아이템 가격")]
    [SerializeField] TMP_Text itemPriceText;    // UI에 표기되는 가격표   
    [SerializeField] int itemPrice;         
    
    [Header("아이템 갯수")]
    [SerializeField] TMP_Text itemCountText;    // UI에 표기되는 갯수
    [SerializeField] int itemCount;         
    
    [Header("아이템 이미지")]
    [SerializeField] public Image ShopItemImage;

    [Header("구매버튼")]
    [SerializeField] Button buyButton;

    [Header("얻는 아이템 (Ex. 토큰)")]
    [SerializeField] int getCount;               // UI에 표기되는 아이템갯수?

    [Header("매진")]
    [SerializeField] bool isSoldOut;

    [Header("설명")]
    [SerializeField] public string Description;

    [SerializeField] private TMP_Text buyButtonText;
    [SerializeField] private ItemData itemGive;         // 살때 주는 재화 데이터 Ex. gold
    [SerializeField] private ItemData itemGet;          // 살때 받는 아이템      Ex. Token


    // 아이템
    [SerializeField] private ItemData _item;

    [SerializeField] public int ShopItemNumber;

    private void Start()
    {
        Init();

        GameManager.UserData.TryInitDummyUserAsync(3, () =>
        {
            Debug.Log("완료");

        });
    }
    private void Init()
    {
        itemNameText = GetUI<TMP_Text>("ItemInfoText"); 
        buyButton.GetComponentInChildren<Button>().onClick.AddListener(Buy);
        buyButtonText = GetUI<TMP_Text>("BuyButtonText");
        itemPriceText = GetUI<TMP_Text>("ItemPriceText");
        itemCountText = GetUI<TMP_Text>("ItemCountText");

        // 테스트용
        itemNameText.text = ShopItemName;
        itemPriceText.text = itemPrice.ToString();
        itemCountText.text = itemCount.ToString();
        // 갯수 바뀌면 업데이트 되도록 해야함

        SetItem(_item);
    }

    
    // ItemData 가져오기
    public void SetItem(ItemData item)
    {
        _item = item;
        ShopItemNumber = item.Id;
        ShopItemName = item.ItemName;
        itemNameText.text = ShopItemName;
        ShopItemImage.sprite = item.SspriteImage;
        Description = item.Description;
        
        // itemPrice
        // itemCount

    }

    /// <summary>
    /// 아이템 관련 업데이트
    /// 현재는 아이템갯수, 매진하면 변경되는거
    /// </summary>
    public void UpdateInfo()
    {
        itemCountText.text = itemCount.ToString();
        if (isSoldOut)
        {
            buyButtonText.text = "매!\t진!";
            buyButton.onClick.RemoveListener(Buy); //구매버튼 비활성화
            ShopItemImage.color = new Color(.3f, .3f, .3f, 1f); // 어둡게 
        }
    }

    private void Buy()
    {
        itemGive = GameManager.TableData.GetItemData(1);
        itemGet = GameManager.TableData.GetItemData(2);
        // ItemData gold = GameManager.TableData.GetItemData(1);
        // ItemData tocken = GameManager.TableData.GetItemData(2);


        Debug.Log(itemGive.Number.Value);
        itemGive.Number.onValueChanged += Gold_onValueChanged;
        itemGet.Number.onValueChanged += Tocken_onValueChanged;

        GameManager.UserData.StartUpdateStream()                    // DB에 갱신 요청 시작
            .SetDBValue(itemGive.Number, itemGive.Number.Value - 10)        // 골드 --
            .SetDBValue(itemGet.Number, itemGet.Number.Value + 2)     // 토큰 ++, 일괄로 갱신할 내용들 등록
            .Submit(OnComplete);                                    // 위에 갱신할것들 갱신요청 전송
        
        // TODO:
        // 구매하는 아이템에 따라 요구하는 재화, 받는 아이템 바꿀 수 있도록.


        itemCount--;
        SoldOut();
        UpdateInfo();
    }

    // 갱신 효과 결과반환
    private void OnComplete(bool result)
    {
        if (false == result)
        {
            Debug.Log($"네트워크 오류");
            return;
        }
        Debug.Log($"구매 하였습니다.");
    }


    // UserDataXX 타입의 값이 갱신되면 통지받음
    private void Gold_onValueChanged(long num)
    {
        Debug.Log($"골드가 {num}개로 바뀜.");
    }
    private void Tocken_onValueChanged(long num)
    {
        Debug.Log($"토큰이 {num}개로 바뀜.");
    }
    public void SoldOut()
    {
        if (itemCount <= 0)
        {
            isSoldOut = true;
        }
        else
            return;
    }
}
