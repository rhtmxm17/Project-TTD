using Live2D.Cubism.Framework.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopPanel : BaseUI
{
    [SerializeField] GameObject mainShopPanel;
    [SerializeField] GameObject dailyShopPanel;

    [SerializeField] ShopItem[] shopItem;
    [SerializeField] public List<ShopItem> shopItems; // 나중에 HideInInspector
    [SerializeField] int itemCounter;

    [SerializeField] public ShopItem CurItemInfo;

    [SerializeField] public GameObject ShopPopup;

    // 상점이름, 추후에 상점종류많아지면 열고 닫을때 교체
    [SerializeField] TMP_Text shopNameText;


    // 팝업창
    [SerializeField] public TMP_Text shopPopupText;
    [SerializeField] public Image shopPopupImage;


    [SerializeField] Button[] shopPopupButton;

  //  [SerializeField] private int _itemCount;
  //  private int ItemCount
  //  {
  //      get => shopItems.Count;
  //      set
  //      {
  //          _itemCount = value;
  //      }
  //  }
    private void Start()
    {
        Init();
        // popupSpot = GetComponentInChildren<Button>();
    }

    private void Init()
    {
        
        mainShopPanel = GetUI("MainShopCanvas");
        dailyShopPanel = GetUI("DailyShopCanvas");

        // 상점이름
        shopNameText = GetUI<TMP_Text>("ShopNameText");

        

        GetUI<Button>("ShopTestButton1").onClick.AddListener(() => Open("ItemListScrollView"));
        GetUI<Button>("ShopTestButton2").onClick.AddListener(() => Open("ItemListScrollView2"));


        ShopPopup = GetUI("ShopPopup");

        UpdateItemList();
        SetShopPopup();

        SetPopupButtons();


    }

    private void SetShopPopup()
    {
        // string description;
        // 클릭한 아이템의 정보를 받아와야함.

        foreach (ShopItem item in shopItems)
        {
           // int index = shopItems.Count;
           // for(int  i = 0; i < shopItems.Count; i++)
           // {
           //     index++;
           //     Debug.Log($"아이템샵 {item}, {index}");
           // }
            Debug.Log($"아이템샵 {item}");
            item.GetComponent<Button>();
        }
        // 아니 왜 이거 안되냐 리스트계쏙


        UpdateItemList();

        shopPopupText = GetUI<TMP_Text>("ShopPopupText");
        shopPopupImage = GetUI<Image>("ShopPopupImage");
        
        GetUI<Button>("ShopPopupClose").onClick.AddListener(() => GoBack("ShopPopup")); // 닫기버튼
        GetUI<TMP_Text>("ShopPopupText").text = "불러온설명";
        GetUI<Image>("ShopPopupImage"); // 누른 이미지에 따른 이미지 변경되도록...

        // ShopItem.SetItem(누른아이템); 누른아이템으로 나오게

    }


    // TODO:
    // 아이템별로 버튼 다 있으니까 Item01~끝번호까지 해서 
    // 거기서 팝업창 띄우고  팝업창이 그 아이템에 맞게 정보를 띄우기
    private void SetPopupButtons()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            Debug.Log($"숫자 {i}");
          //  shopPopupButton[i] = GetUI<Button>($"Itme{i}");
        }

        GetUI<Button>("Itme0").onClick.AddListener(() => OpenPopup());
        GetUI<Button>("Itme1").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme2").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme3").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme4").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme5").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme6").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme7").onClick.AddListener(() => Open("ShopPopup"));
    }

    private void UpdateItemList()
    {
        shopItems = GetComponentsInChildren<ShopItem>().ToList();
        itemCounter = shopItems.Count;
        
    }
    
    public void OpenPopup()
    {
        Open("ShopPopup");
        SetShopPopup();
    }



    public void Open(string name)
    {
        Debug.Log($"{name} 패널을 엽니다");
        GetUI(name).SetActive(true);
    }

    public void GoBack(string name)
    {
        Debug.Log($"{name} 패널을 닫습니다");
        GetUI(name).SetActive(false);
    }

    
}
