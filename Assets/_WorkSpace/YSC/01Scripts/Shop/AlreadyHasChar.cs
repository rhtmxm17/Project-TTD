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
    // 아이템이름 텍스트
    [SerializeField] TMP_Text itemNameText;
    
    public ShopItemData shopItemData { get; private set; }

    [Header("강화수치 관련")]
    [SerializeField] int currentEnhance;
    [SerializeField] int maxEnhance = 10;
    private bool isMaxed = false;
    [SerializeField] ItemData maxTocken;
    // [SerializeField] int currentChar;

    void OnEnable()
    {
        Debug.Log("더 이상 캐릭터를 강화할 수 없습니다. 그래도 구매하시겠습니까?");
        Init();
    }

    private void Init()
    {
        bgCancelButton = GetUI<Button>("CharWarningBG");        // 외각 나가기 버튼
        bgCancelButton.onClick.AddListener(OnPopupOKButtonClicked);
        cancelButton = GetUI<Button>("CancelButton");           // 취소 버튼(나가기)
        cancelButton.onClick.AddListener(OnPopupOKButtonClicked);
        exitButton = GetUI<Button>("ExitButton");               // 나가기버튼
        exitButton.onClick.AddListener(OnPopupOKButtonClicked);
        purchaseButton = GetUI<Button>("PurchaseButton");       // 구매버튼
        purchaseButton.onClick.AddListener(Purchase);               
        warningText = GetUI<TMP_Text>("WarningText");           // 경고 텍스트
        itemNameText = GetUI<TMP_Text>("ItemNameText");         // 아이템 이름 텍스트
        
        // TODO: 아이템 정보는 만들때 받아와야함
      //  CheckMaxEnhance();
    }

    public void SetItem(ShopItemData data)
    {
        shopItemData = data;
    }

    public void CheckMaxEnhance()
    {
        int charItemData = this.shopItemData.Id;
        charItemData -= 200;

        var currentChar = GameManager.TableData.GetCharacterData(charItemData);
        Debug.Log($"currentChar: {currentChar}");
        Debug.Log($"불러온캐릭터 인덱스: {currentEnhance}");
        Debug.Log($"불러온캐릭터 강화 수치: {currentChar.Enhancement.Value}");
        currentEnhance = currentChar.Enhancement.Value;
        if (currentEnhance == maxEnhance)
        {
            isMaxed = true;
            warningText.text = "더이상 캐릭터를 강화 할 수 없습니다\n그래도구매하시겠습니까?\n용젤리가 지급됩니다.";
        }
        else isMaxed = false;
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
            // TODO: 강화상태맥스면 (isMaxed==true) => maxTocken(일단냥 "용과"로) 주기
            UserDataInt itemGet = product.item.Number;
            if (isMaxed)
            {
                Debug.Log("캐릭터 강화단계가 최대이므로 대체아이템이 지급됩니다.");
                // TODO: itemGet(받는템)을 바꿔야함.
                itemGet = maxTocken.Number;
                dbUpdateStream.AddDBValue(itemGet, 10); 
                Debug.Log($"{itemGet}이 {10}개 지급되었습니다.");
                // DB에서 용과가 오르긴 하는데 ItemGain 팝업이 안뜸.
            }
            else
            dbUpdateStream.AddDBValue(itemGet, product.gain); // 요청에 '상품 획득' 등록
            // itemGet을 product.gain(게인에 set된 숫자만큼 받음)
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
        if (isMaxed)
        {
            ItemGainPopup popupInstance2 = GameManager.OverlayUIManager.PopupItemGainForMax(maxTocken);
            popupInstance2.Title.text = "구매 성공!";
            // 아 이거 너무 하드코딩 지저분인데...
        }
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
