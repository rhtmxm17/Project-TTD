using Live2D.Cubism.Framework.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;


public class ShopPanel : BaseUI
{
    [SerializeField] GameObject mainShopPanel;
    [SerializeField] GameObject dailyShopPanel;

    [SerializeField] ShopItem shopItem;
    [SerializeField] public List<ShopItem> shopItems; // 나중에 HideInInspector
    [SerializeField] int itemCounter;


    [SerializeField] public GameObject ShopPopup;

    // 상점이름, 추후에 상점종류많아지면 열고 닫을때 교체
    [SerializeField] TMP_Text shopNameText;


    // 팝업창
    [SerializeField] TMP_Text shopPopupText;
    [SerializeField] Image shopPopupImage;
    [SerializeField] TMP_Text shopPopupNameText;
    // 팝업창열리게하는 버튼
    [SerializeField] Button[] shopPopupButton;


    private void Start()
    {
        Init();

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

        ListUpItems();
        SetShopPopup();

        SetPopupButtons();


    }

    private void SetShopPopup()
    {
        // string description;
        // 클릭한 아이템의 정보를 받아와야함.

        shopPopupText = GetUI<TMP_Text>("ShopPopupText");
        shopPopupImage = GetUI<Image>("ShopPopupImage");
        shopPopupNameText = GetUI<TMP_Text>("ShopPopupNameText");

        GetUI<Button>("ShopPopupClose").onClick.AddListener(() => GoBack("ShopPopup")); // 닫기버튼
        GetUI<TMP_Text>("ShopPopupText").text = "불러온설명";

    }
    public void OpenPopup(int index)
    {
        Open("ShopPopup");
        SetShopPopup();
        UpdateItemList(index);
    }
    private void UpdateItemList(int index)
    {
        shopPopupText.text = shopItems[index].Description;
        shopPopupImage.sprite = shopItems[index].ShopItemImage.sprite;
        shopPopupNameText.text = shopItems[index].ShopItemName;
    }


    private void SetPopupButtons()
    {
        // 버튼 추가
        for (int i = 0; i < shopItems.Count; i++)
        {
            // Debug.Log($"숫자 {i}");
            shopPopupButton[i] = shopItems[i].GetComponent<Button>();
            int index = i;
            shopPopupButton[i].onClick.AddListener(() => OpenPopup(index));
        }

    }

    private void ListUpItems()
    {
        shopItems = GetComponentsInChildren<ShopItem>().ToList();
        itemCounter = shopItems.Count;
    }



    #region 기본여닫이
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
    #endregion

}
