using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.HID;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PurchasingPanel : BaseUI
{
   // public event UnityAction onPopupClosed;
    
    [Header("버튼들")]
    [SerializeField] private Button backgroundButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button minButton;
    [SerializeField] private Button maxButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    
    [Header("아이템관련 정보")]
    // 아이템 view
    [SerializeField] private Image itemImage;               // (7) 아이템 img
    [SerializeField] private TMP_Text itemNameText;         // (9) 상품명 txt
    [SerializeField] private TMP_Text itemAmountText;       // (8) 아이템 수량 txt
    [SerializeField] private TMP_Text itemOwnText;          // (11) 보유량 txt
    [SerializeField] private TMP_Text currentNumberText;    // (16) 현재량 txt
    // 아이템 model 
    [SerializeField] private string itemName;        // (9) 상품명    
    [SerializeField] private int itemAmount;         // (8) 아이템 수량 
    [SerializeField] private int itemOwn;            // (11) 보유량     
    [SerializeField] private int currentNumber;      // (16) 현재량

    ShopPanel shopPanel;
    
    ShopItem shopItem;
    
    public ShopItemData shopItemData {get; private set;}


    void Start()
    {
        Init();
    }

    private void Init()
    {
        // Buttons
        backgroundButton = GetUI<Button>("PurchasingBG"); 
        closeButton = GetUI<Button>("CloseButton");
        purchaseButton = GetUI<Button>("PurchaseButton");
        cancelButton = GetUI<Button>("CancelButton");
        minButton = GetUI<Button>("MinButton");
        maxButton = GetUI<Button>("MaxButton");
        plusButton = GetUI<Button>("PlusButton");
        minusButton = GetUI<Button>("MinusButton");
        
        // 아이템View
        itemImage = GetUI<Image>("ItemImage");
        itemNameText = GetUI<TMP_Text>("ItemNameText");
        itemAmountText = GetUI<TMP_Text>("ItemAmountText");
        itemOwnText = GetUI<TMP_Text>("ItemOwnText");
        currentNumberText = GetUI<TMP_Text>("CurrentNumberText");
        
        // 구매버튼
        purchaseButton.onClick.AddListener(Purchase);
        
        // 닫기버튼
        backgroundButton.onClick.AddListener(ClosePopup);
        closeButton.onClick.AddListener(ClosePopup);
        cancelButton.onClick.AddListener(ClosePopup);
        
        // 갯수변동 버튼
        // 최소 | 최대
        maxButton.onClick.AddListener(Maximize);
        minButton.onClick.AddListener(Minimize);
        // 추가 | 감소
        plusButton.onClick.AddListener(Add);
        minusButton.onClick.AddListener(Subtract);
        
        // 현재 아이템 갯수 세팅
        currentNumber = 1;
        currentNumberText.text = currentNumber.ToString();

    }

    public void SetItem(ShopItemData data)
    {
        shopItemData = data;
        itemNameText.text = data.ShopItemName;
        itemImage.sprite = data.Sprite;
        int amount = shopItemData.LimitedCount - shopItemData.Bought.Value;
        itemAmountText.text = $"아이템 수량: {amount}";     //  (8) 아이템수량
        itemOwnText.text = $""; // (10) 보유량
        // TODO: 유저데이터 가져와서 해당아이템의 보유 갯수 보여줘야함, 어떻게 할지 아직 구상이안됨
    }

    public void UpdateInfo()
    {
        int remain = shopItemData.LimitedCount - shopItemData.Bought.Value;
        if (remain > 0)
        {
            itemAmountText.text = $"아이템 수량: {remain}";
           // itemAmountText.text = $"아이템 수량: {remain}/{shopItemData.LimitedCount}";
        }
        else
        {
            itemAmountText.text = "매!\t진";
            itemImage.color = new Color(.3f, .3f, .3f, 1f); 
        }
        currentNumberText.text = currentNumber.ToString();
    }

    private void Purchase()
    {
        Debug.Log($"해당 아이템을 {currentNumber}개 구매 합니다");
        
        var dbUpdateStream = GameManager.UserData.StartUpdateStream() // DB에 갱신 요청 시작
            .AddDBValue(shopItemData.Bought, currentNumber);  // 요청에 '구매 횟수만큼 증가' 등록
        
        ItemData itemPay = shopItemData.Price.item;
        if (null != itemPay) // 무료가 아니라면
        {
            Debug.Log($"골드 소지 개수:{itemPay.Number.Value}/비용:{shopItemData.Price.gain * currentNumber}");
            dbUpdateStream.AddDBValue(itemPay.Number, -shopItemData.Price.gain * currentNumber); // 요청에 요구 수량만큼'비용 지불' 등록
            Debug.Log($"지불 후 골드 :{itemPay.Number.Value}");   
            // TODO: 구매 가능/불가 판별 => 불가능 팝업
            // (소지금 < 가격 => 구매불가(팝업띄우기))
        }
        
        foreach (ItemGain product in shopItemData.Products)
        {
            UserDataInt itemGet = product.item.Number;
            dbUpdateStream.AddDBValue(itemGet, product.gain * currentNumber); // 요청에 갯수만큼 '상품 획득' 등록
        }
        
        
        dbUpdateStream.Submit(OnComplete);
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

        UpdateInfo(); // 갱신된 상품 정보(구매 횟수) 반영
        
        
        
        ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(shopItemData.Products);
        popupInstance.Title.text = "구매 성공!";
        // this.shopItem.UpdateInfo(); // 이거 하면 창이 안닫힘
        ClosePopup();
    }

    private void ClosePopup()
    {
       // onPopupClosed?.Invoke();
        Destroy(this.gameObject);
    }

    private void Add()
    {
        currentNumber++;
        if (currentNumber >= shopItemData.LimitedCount)
        {
            currentNumber = shopItemData.LimitedCount;
        }
        UpdateInfo();
        Debug.Log($"갯수를 추가 합니다. 현재갯수: {currentNumber}");
    }
    private void Subtract()
    {
        currentNumber--;
        if (currentNumber <= 0)
        {
            currentNumber = 1;
        }
        
        UpdateInfo();
        Debug.Log($"갯수를 감소 합니다. 현재갯수: {currentNumber}");
    }

    private void Maximize()
    {
        currentNumber = shopItemData.LimitedCount;
        UpdateInfo();
        Debug.Log($"갯수를 최대로 올립니다. 현재갯수: {currentNumber}");
    }
    private void Minimize()
    {
        currentNumber = 1;
        UpdateInfo();
        Debug.Log($"갯수를 최저로 내립니다. 현재갯수: {currentNumber}");
    }
    
    
}
