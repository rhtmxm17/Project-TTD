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
    public static event UnityAction OnAmountChanged;
                                          
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
   // [SerializeField] private TMP_Text itemDescriptionText;  // (없음) 아이템설명 txt : 패키지 설명할라고 그냥 넣음.
    
    // 아이템 model 
    [SerializeField] private string itemName;        // (9) 상품명    
    [SerializeField] private int itemAmount;         // (8) 아이템 수량 
    [SerializeField] private int itemOwn;            // (11) 보유량     
    [SerializeField] private int currentNumber;      // (16) 현재량

    ShopPanel shopPanel;
    
    [SerializeField] ShopItem shopItem;

    [Header("아이템리스트 보여주기용")]
    [SerializeField] ItemGainCell itemGainCellPrefab;
    [SerializeField] LayoutGroup layoutGroup;
    
    public ShopItemData shopItemData {get; private set;}
    
    bool isBulk => shopItemData.IsMany;
    // private int remainCount => shopItemData.LimitedCount - shopItemData.Bought.Value; // 남은 구매횟수
    // 위에람다식은 재활용못해서 변경          구매가능횟수 - 구매한 횟수
    private int remainCount;
    void OnEnable()
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
       // itemDescriptionText = GetUI<TMP_Text>("ItemDescriptionText");
        
        // 구매버튼
        purchaseButton.onClick.AddListener(OpenDoubleWarning);
        
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

    /// <summary>
    /// 아이템 구매확인창에 나오는 아이템 세팅
    /// </summary>
    /// <param name="data"></param>
    public void SetItem(ShopItemData data)      // 아이템 세팅 
    {
        shopItemData = data;
        itemNameText.text = data.ShopItemName;
        itemImage.sprite = data.Sprite;
        
        // 남은구매횟수
        remainCount = shopItemData.LimitedCount - shopItemData.Bought.Value; // 구매가능횟수 - 구매한 횟수
        
        // 복수구매 숫자 조정
        if(isBulk)
        {   // 복수구매면 구매가능횟수 바꾸기 (소지중인 지불하는 아이템 총갯수 / 지불해야되는 아이템 가격 => 반내림)
            remainCount = Mathf.FloorToInt((shopItemData.Price.item.Number.Value) / (shopItemData.Price.gain));
        }  //구매 한도 => 갖고있는 지불할 아이템 총량 / 아이템 가격 => 반내림
        
        
        // TODO: boolType 만들어서 패키지상품일떄만 tiemOwnText만 나오게하고 다른상품들은 기존대로 나오게 하기.
        for (int i = 0; i < data.Products.Count; i++)
        {
            switch (i)
            {
                case 0:
                    itemAmountText.gameObject.SetActive(true);
                    itemOwnText.text = $"현재 보유량: {shopItemData.Products[i].item.Number.Value}";
                    break;
                case 1:
                    itemAmountText.gameObject.SetActive(false);
                    itemOwnText.text = $"현재 보유량:\n {shopItemData.Products[0].item.ItemName}: {shopItemData.Products[0].item.Number.Value}" +
                                       $"\t{shopItemData.Products[1].item.ItemName}:{shopItemData.Products[1].item.Number.Value}";
                    itemOwnText.fontSize = 30;
                    break;
                case 2:
                    itemAmountText.gameObject.SetActive(false);
                    itemOwnText.text = $"현재 보유량:\n {shopItemData.Products[0].item.ItemName}: {shopItemData.Products[0].item.Number.Value}" +
                                       $"\n{shopItemData.Products[1].item.ItemName}:{shopItemData.Products[1].item.Number.Value}" +
                                       $"\n{shopItemData.Products[2].item.ItemName}:{shopItemData.Products[2].item.Number.Value}";
                    itemOwnText.fontSize = 24;
                    break;
                case 3:
                    itemAmountText.gameObject.SetActive(false);
                    itemOwnText.text = $"현재 보유량:\n {shopItemData.Products[0].item.ItemName}: {shopItemData.Products[0].item.Number.Value}" +
                                       $"\n{shopItemData.Products[1].item.ItemName}:{shopItemData.Products[1].item.Number.Value}" +
                                       $"\n{shopItemData.Products[2].item.ItemName}:{shopItemData.Products[2].item.Number.Value}"+
                                       $"\n{shopItemData.Products[3].item.ItemName}:{shopItemData.Products[3].item.Number.Value}";;
                    itemOwnText.fontSize = 18;
                    break;
            }
        }
        //itemOwnText.text = $"현재 보유량: {shopItemData.Products[0].item.Number.Value}"; // (10) 보유량 //지금 친건 가격
        
        itemAmountText.text = $"아이템 수량: {remainCount}";     //  (8) 아이템수량
        

       // itemDescriptionText.text = data.Description.Replace("\\n", "\n"); // 아이템설명

    }

    public void UpdateInfo()
    {
        if (remainCount >= 0)
        {
            itemAmountText.text = $"아이템 수량: {remainCount}";
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

        ItemData itemPay = shopItemData.Price.item;
        if (null != itemPay && shopItemData.Price.item.Number.Value < shopItemData.Price.gain * currentNumber)
        {   // 지불해야하는 아이템이고 비용이 부족하면
            Debug.LogWarning("비용 부족");
            // 팝업UI
            OpenWarning();      // 비용부족 경고 팝업.
            return;
        }

        var dbUpdateStream = GameManager.UserData.StartUpdateStream() // DB에 갱신 요청 시작
            .AddDBValue(shopItemData.Bought, currentNumber);  // 요청에 '구매 횟수만큼 증가' 등록 // 이게 들어가버림
        
        if (null != itemPay) // 무료가 아니라면
        {
            Debug.Log($"골드 소지 개수:{itemPay.Number.Value}/비용:{shopItemData.Price.gain * currentNumber}");
            dbUpdateStream.AddDBValue(itemPay.Number, -shopItemData.Price.gain * currentNumber); // 요청에 요구 수량만큼'비용 지불' 등록
            Debug.Log($"지불 후 골드 :{(itemPay.Number.Value)}");   
            // 지불할템 소지량 구매 가능/불가 판별 => 불가능 팝업
            // (소지금 < 가격 => 구매불가(팝업띄우기))
            if (shopItemData.Price.item.Number.Value < shopItemData.Price.gain * currentNumber)
            {
                Debug.LogWarning("비용 부족");
                OpenWarning();      // 비용부족 경고 팝업.
                return;
            }
        }
        
        foreach (ItemGain product in shopItemData.Products)
        {
            UserDataInt itemGet = product.item.Number;
            dbUpdateStream.AddDBValue(itemGet, product.gain * currentNumber); // 요청에 갯수만큼 '상품 획득' 등록
        }
        
        ClosePopup();

        dbUpdateStream.Submit(OnComplete);

    }

    private void OnComplete(bool result)  // 구매완료
    {

        if (false == result)
        {
            Debug.Log($"네트워크 오류");
            return;
        }
        Debug.Log($"구매 하였습니다.");

        AmmountChanged();
        UpdateInfo(); // 갱신된 상품 정보(구매 횟수) 반영

        // 팝업 UI용 구입 완료한 아이템 목록 생성
        List<ItemGain> bought = new List<ItemGain>(shopItemData.Products.Count);
        for (int i = 0; i < shopItemData.Products.Count; i++)
        {
            bought.Add(new ItemGain()
            {
                item = shopItemData.Products[i].item,
                gain = shopItemData.Products[i].gain * currentNumber, // 구매 완료된 개수
            });
        }

        ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(bought);
        popupInstance.Title.text = "구매 성공!";
        // this.shopItem.UpdateInfo(); // 이거 하면 창이 안닫힘
        // ClosePopup();
    }
    
    public void AddItemPackage(List<ItemGain> gain)
    {
        List<ItemGainCell> items = new List<ItemGainCell>();
        foreach (ItemGain info in gain)
        {
            ItemGainCell cell = Instantiate(itemGainCellPrefab, layoutGroup.transform);

            // 정보 출력칸 초기화
            cell.SetItem(info.item, info.gain);
            items.Add(cell);

        }
    }
    
    

    private void ClosePopup()
    {
       // onPopupClosed?.Invoke();
        Destroy(this.gameObject);
    }
    
    private void OpenDoubleWarning()
    {
        OverlayUIManager popupInstance = GameManager.OverlayUIManager;
        popupInstance.OpenDoubleInfoPopup("해당 아이템을 정말 구매하시겠습니까?", "취소",
            "확인",null, Purchase);
    }

    

    private void Add()
    {
        currentNumber++;
        if (currentNumber >= remainCount)
        {
            currentNumber = remainCount;
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
        currentNumber = remainCount;
        UpdateInfo();
        Debug.Log($"갯수를 최대로 올립니다. 현재갯수: {currentNumber}");
    }
    private void Minimize()
    {
        currentNumber = 1;
        UpdateInfo();
        Debug.Log($"갯수를 최저로 내립니다. 현재갯수: {currentNumber}");
    }

    public void AmmountChanged()
    {
        Debug.Log("갯수변경 이벤트");
        OnAmountChanged?.Invoke();
    }
    private void OpenWarning()
    {
        OverlayUIManager popupInstance = GameManager.OverlayUIManager;
        popupInstance.OpenSimpleInfoPopup("소지하신 재료가 부족합니다.", "닫기", null);
    }
    
}
