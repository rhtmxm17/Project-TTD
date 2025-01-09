using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AlreadyHasChar : BaseUI
{
    public event UnityAction onPopupClosed;
    
    // 배경, 닫히는 버튼
    [SerializeField] Button bgCancelButton;
    // 취소버튼
    [SerializeField] Button cancelButton;
    // 닫기버튼
    [SerializeField] Button exitButton;
    // 확인버튼
    [SerializeField] Button purchaseButton;
    // 경고메세지
    [SerializeField] TMP_Text warningText;
    
    public ShopItemData shopItemData { get; private set; }
    
    
  //  private void Awake()
  //  {
  //      bgCancelButton.onClick.AddListener(OnPopupOKButtonClicked);
  //      cancelButton.onClick.AddListener(OnPopupOKButtonClicked);
  //      exitButton.onClick.AddListener(OnPopupOKButtonClicked);
  //  }

    void OnEnable()
    {
        Debug.Log("더 이상 캐릭터를 강화할 수 없습니다. 그래도 구매하시겠습니까?");
        Init();
    }

    private void Init()
    {
        bgCancelButton = GetUI<Button>("CharWarningBG");
        bgCancelButton.onClick.AddListener(OnPopupOKButtonClicked);
        cancelButton = GetUI<Button>("CancelButton");
        cancelButton.onClick.AddListener(OnPopupOKButtonClicked);
        exitButton = GetUI<Button>("ExitButton");
        exitButton.onClick.AddListener(OnPopupOKButtonClicked);
        purchaseButton = GetUI<Button>("PurchaseButton");
        purchaseButton.onClick.AddListener(Purchase);
        warningText = GetUI<TMP_Text>("WarningText");
        
        // TODO: 아이템 정보는 만들때 받아와야함
    }

    public void SetItem(ShopItemData data)
    {
        shopItemData = data;
    }

    private void Purchase()
    {
        ItemData itemGive = shopItemData.Price.item;
        if (null != itemGive && shopItemData.Price.item.Number.Value < shopItemData.Price.gain)
        {
            Debug.LogWarning("비용 부족");
            // 돈부족하면 돈없어 팝업UI
            GameManager.OverlayUIManager.OpenSimpleInfoPopup("소지하신 재료가 부족합니다.", "닫기", null);
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
    }
    
    private void OnComplete(bool result)
    {
        // TODO: 네트워크 로딩 닫기

        if (false == result)
        {
            Debug.Log($"네트워크 오류");
            return;
        }
        Debug.Log($"구매 하였습니다.");

        // UpdateInfo(); // 갱신된 상품 정보(구매 횟수) 반영
        
        ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(shopItemData.Products);
        popupInstance.Title.text = "구매 성공!";
        //TODO: 구매되면 창닫기 
        OnPopupOKButtonClicked();
    }
    
    protected virtual void OnPopupOKButtonClicked()
    {
        onPopupClosed?.Invoke();
        Destroy(this.gameObject);
    }
    
}
