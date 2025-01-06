using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : BaseUI
{
    public ShopItemData shopItemData { get; private set; }

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

    [SerializeField] private TMP_Text buyButtonText;    // 매진되면 매진나오는 텍스트

    // 구매확인창 (구매버튼 누르면 팝업)
    [SerializeField] PurchasingPanel purchasingPanel;
    
    private void Start()
    {
        Init();
        SubscribeEvents();
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
        int remain = shopItemData.LimitedCount - shopItemData.Bought.Value;
        itemCountText.text = $"구매 가능 횟수 {remain}/{shopItemData.LimitedCount}";

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
        Debug.Log("아이템정보를 갱신합니다!!!!!!!!!!");
       
    }

    private void Buy()
    {

        // TODO: 구매확인창 UI && 기능
        // 구매갯수 > 1 || 무제한 => 구매확인창열기 
        int remain = shopItemData.LimitedCount - shopItemData.Bought.Value;
        if (remain > 0)
        {
            Debug.Log("구매확인창 열기");
            OpenPurchasingPanel(); // 확인 팝업띄우는
            return;
        }

        ItemData itemGive = shopItemData.Price.item;
        if (null != itemGive && shopItemData.Price.item.Number.Value < shopItemData.Price.gain)
        {
            Debug.LogWarning("비용 부족");
            // TODO: 팝업UI
            return;
        }

        var dbUpdateStream = GameManager.UserData.StartUpdateStream() // DB에 갱신 요청 시작
            .AddDBValue(shopItemData.Bought, 1);  // 요청에 '구매 횟수 증가' 등록


        if (null != itemGive) // 무료가 아니라면
        {
            Debug.Log($"소지 개수:{itemGive.Number.Value}/비용:{shopItemData.Price.gain}");
            dbUpdateStream.AddDBValue(itemGive.Number, -shopItemData.Price.gain); // 요청에 '비용 지불' 등록
            // TODO: 구매 가능/불가 판별 => 불가능 팝업
            // (소지금 < 가격 => 구매불가(팝업띄우기))
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
        
        ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(shopItemData.Products);
        popupInstance.Title.text = "구매 성공!";
    }

    public void SoldOut()
    {
        
        buyButtonText.text = "매!\t진!";
        buyButton.onClick.RemoveListener(Buy); //구매버튼 비활성화
        ShopItemImage.color = new Color(.3f, .3f, .3f, 1f); // 어둡게 
        
    }

    public void SubscribeEvents()
    {
        // shopItemData.Bought.onValueChanged += UpdateInfo;
        PurchasingPanel.OnAmountChanged += UpdateInfo;        
    }
    // 구매확인창 열기
    private void OpenPurchasingPanel()
    {
        PurchasingPanel popupInstance = Instantiate(purchasingPanel, GameManager.PopupCanvas);
        popupInstance.SetItem(shopItemData);
    }
}
