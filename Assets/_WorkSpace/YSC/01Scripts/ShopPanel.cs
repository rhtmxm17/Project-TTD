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
    private void Start()
    {
        Init();

    }

    private void Init()
    {
        mainShopPanel = GetUI("MainShopCanvas");
        dailyShopPanel = GetUI("DailyShopCanvas");

        GetUI<Button>("ShopTestButton1").onClick.AddListener(() => Open("ItemListScrollView"));
        GetUI<Button>("ShopTestButton2").onClick.AddListener(() => Open("ItemListScrollView2"));


        GetUI<Button>("ItemInfoSpot").onClick.AddListener(() => Open("ShopPopup"));

    }

    private void SetShopPopup()
    {
       // string description;
        // 클릭한 아이템의 정보를 받아와야함.
        GetUI<Button>("ShopPopupClose").onClick.AddListener(() => GoBack("ShopPopup")); // 닫기버튼
        GetUI<TMP_Text>("ShopPopupText").text = "description";
        GetUI<Image>("ShopPopupImage"); // 누른 이미지에 따른 이미지 변경되도록...

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
