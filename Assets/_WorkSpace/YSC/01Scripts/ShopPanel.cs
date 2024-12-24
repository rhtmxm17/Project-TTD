using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : BaseUI
{
    [SerializeField] GameObject mainShopPanel;
    [SerializeField] GameObject dailyShopPanel;

    [SerializeField] ShopItem[] shopItems;

    [SerializeField] TMP_Text shopNameText;

    // [SerializeField] Button popupSpot;
    private void Start()
    {
        Init();
        // popupSpot = GetComponentInChildren<Button>();
    }

    private void Init()
    {
        mainShopPanel = GetUI("MainShopCanvas");
        dailyShopPanel = GetUI("DailyShopCanvas");

        GetUI<Button>("ShopTestButton1").onClick.AddListener(() => Open("ItemListScrollView"));
        GetUI<Button>("ShopTestButton2").onClick.AddListener(() => Open("ItemListScrollView2"));

        SetPopupButtons();



    }

    private void SetShopPopup()
    {
        // string description;
        // 클릭한 아이템의 정보를 받아와야함.

        shopNameText = GetUI<TMP_Text>("ShopNameText");                                 // 상점이름
        GetUI<Button>("ShopPopupClose").onClick.AddListener(() => GoBack("ShopPopup")); // 닫기버튼
        GetUI<TMP_Text>("ShopPopupText").text = "description";
        GetUI<Image>("ShopPopupImage"); // 누른 이미지에 따른 이미지 변경되도록...
    }


    // TODO:
    // 아이템별로 버튼 다 있으니까 Item01~끝번호까지 해서 
    // 거기서 팝업창 띄우고  팝업창이 그 아이템에 맞게 정보를 띄우기
    private void SetPopupButtons()
    {
        GetUI<Button>("Itme01").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme02").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme03").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme04").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme05").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme06").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme07").onClick.AddListener(() => Open("ShopPopup"));
        GetUI<Button>("Itme08").onClick.AddListener(() => Open("ShopPopup"));
    }


    private void SetStore()
    {

    }



    private void Open(string name)
    {
        Debug.Log($"{name} 패널을 엽니다");
        GetUI(name).SetActive(true);
    }

    private void GoBack(string name)
    {
        Debug.Log($"{name} 패널을 닫습니다");
        GetUI(name).SetActive(false);
    }

    
}
