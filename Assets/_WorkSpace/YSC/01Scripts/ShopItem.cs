using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : BaseUI
{
    public ShopItemData shopItemData { get; private set; }

    [SerializeField] ItemGainPopup itemGainPopupPrefab;

    [Header("아이템 이름")]
    [SerializeField] TMP_Text itemNameText;     // UI_아이템이름

    [Header("아이템 가격")]
    [SerializeField] TMP_Text itemPriceText;    // UI에 표기되는 가격표
    
    [Header("아이템 갯수")]
    [SerializeField] TMP_Text itemCountText;    // UI에 표기되는 갯수
    
    [Header("아이템 이미지")]
    [SerializeField] public Image ShopItemImage;

    [Header("구매버튼")]
    [SerializeField] Button buyButton;

    [SerializeField] private TMP_Text buyButtonText;

    private void Start()
    {
        Init();

    }
    private void Init()
    {
        itemNameText = GetUI<TMP_Text>("ItemInfoText"); 
        buyButton.GetComponentInChildren<Button>().onClick.AddListener(Buy);
        buyButtonText = GetUI<TMP_Text>("BuyButtonText");
        itemPriceText = GetUI<TMP_Text>("ItemPriceText");
        itemCountText = GetUI<TMP_Text>("ItemCountText");

        SetItem(shopItemData);
    }

    // ItemData 가져오기
    public void SetItem(ShopItemData data)
    {
        shopItemData = data;
        itemNameText.text = data.ShopItemName;

        ShopItemImage.sprite = data.Sprite;

        // 가격 표시
        if (null == data.Price.item)
        {
            itemPriceText.text = "무료";
        }
        else
        {
            itemPriceText.text = $"{data.Price.item.ItemName} {data.Price.gain}개";
        }

        UpdateInfo();
    }

    /// <summary>
    /// 아이템 관련 업데이트
    /// 현재는 아이템갯수, 매진하면 변경되는거
    /// </summary>
    public void UpdateInfo()
    {
        if (false == shopItemData.IsLimited)
        {
            itemCountText.text = string.Empty;
            return;
        }

        int remain = shopItemData.LimitedCount - shopItemData.Bought.Value;
        if (remain > 0)
        {
            itemCountText.text = $"구매 가능 횟수 {remain}/{shopItemData.LimitedCount}";
        }
        else
        {
            buyButtonText.text = "매!\t진!";
            buyButton.onClick.RemoveListener(Buy); //구매버튼 비활성화
            ShopItemImage.color = new Color(.3f, .3f, .3f, 1f); // 어둡게 
        }
    }

    private void Buy()
    {
        var dbUpdateStream = GameManager.UserData.StartUpdateStream() // DB에 갱신 요청 시작
            .AddDBValue(shopItemData.Bought, 1);  // 요청에 '구매 횟수 증가' 등록

        ItemData itemGive = shopItemData.Price.item;
        if (null != itemGive) // 무료가 아니라면
        {
            Debug.Log($"소지 개수:{itemGive.Number.Value}/비용:{shopItemData.Price.gain}");
            dbUpdateStream.AddDBValue(itemGive.Number, -shopItemData.Price.gain); // 요청에 '비용 지불' 등록
        }
        
        foreach (ItemGain product in shopItemData.Products)
        {
            UserDataInt itemGet = product.item.Number;
            dbUpdateStream.AddDBValue(itemGet, product.gain); // 요청에 '상품 획득' 등록
        }

        dbUpdateStream.Submit(OnComplete); // 등록된 갱신 요청 전송
        
        // TODO: 네트워크 로딩 띄우기
    }

    // 갱신 효과 결과반환
    private void OnComplete(bool result)
    {
        // TODO: 네트워크 로딩 닫기

        if (false == result)
        {
            Debug.Log($"네트워크 오류");
            return;
        }
        Debug.Log($"구매 하였습니다.");

        UpdateInfo(); // 갱신된 상품 정보(구매 횟수) 반영

        ItemGainPopup popupInstance = Instantiate(itemGainPopupPrefab, GameManager.PopupCanvas);
        popupInstance.Initialize(shopItemData.Products);
    }

}
