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
    // TODO: 확인하려고 [SerializeField] 구별해서 바꾸기

    // // 상점 아이템 
    [SerializeField] public List<ShopItem> shopItems; // 나중에 HideInInspector
    
    // 팝업창열리게하는 버튼
    [SerializeField] Button[] shopPopupButton;

    // 정보팝업창
    [SerializeField] public GameObject ShopPopup;

    // 상점이름, 추후에 상점종류많아지면 열고 닫을때 교체
    [SerializeField] TMP_Text shopNameText;

    // 팝업창
    [SerializeField] TMP_Text shopPopupText;
    [SerializeField] Image shopPopupImage;
    [SerializeField] TMP_Text shopPopupNameText;

    private int _itemCount;
    // 아이탬 갯수 새고 리스트에 패스
    [SerializeField] int itemCounter
    {
        get => shopItems.Count;
        set => _itemCount = value;
    }
    private void Start()
    {
        Init();

    }

    private void Init()
    {
        // 상점이름
        shopNameText = GetUI<TMP_Text>("ShopNameText");

        // 다른 종류 상점들 추가 => CanvasSwitch 스크립트에서 관리
        // GetUI<Button>("ShopTestButton1").onClick.AddListener(() => Open("ItemListScrollView"));
        // GetUI<Button>("ShopTestButton2").onClick.AddListener(() => Open("ItemListScrollView2"));

        // 정보팝업
        ShopPopup = GetUI("ShopPopup");

        // 상점템 리스트 갱신
        ListUpItems();
        // 정보팝업 UI세팅
        SetShopPopup();
        // 정보팝업 얼리는 버튼 추가
        SetPopupButtons();
    }
   
    
    private void ListUpItems() // 상점아이템리스트 갱신
    {
        shopItems = GetComponentsInChildren<ShopItem>().ToList();
        itemCounter = shopItems.Count;
        // 버튼 개수 양만큼 추가
        shopPopupButton = new Button[itemCounter];
    }
    private void SetShopPopup() // 정보팝업창 UI 변수
    {
        shopPopupText = GetUI<TMP_Text>("ShopPopupText");
        shopPopupImage = GetUI<Image>("ShopPopupImage");
        shopPopupNameText = GetUI<TMP_Text>("ShopPopupNameText");
        // 닫기버튼
        GetUI<Button>("ShopPopupClose").onClick.AddListener(() => GoBack("ShopPopup")); 
        // 기본설명
        GetUI<TMP_Text>("ShopPopupText").text = "기본설명";

    }
    private void SetPopupButtons()  // 정보팝업 얼리는 버튼 추가
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            Debug.Log($"숫자 {i}");
            shopPopupButton[i] = shopItems[i].GetComponent<Button>();
            int index = i;
            shopPopupButton[i].onClick.AddListener(() => OpenPopup(index));
            // AddListner에 그냥 i로 넣으면 배열읽어오는데 OutOfBoundary에러가 남... 그래서 지역변수하나해서 삽입
        }
    }

    


    /// <summary>
    /// 인덱스번호에 할당된 아이템 팝업창을 열고
    /// 그 할당된 정보를 업뎃하고 보여주기
    /// </summary>
    /// <param name="index"></param>
    public void OpenPopup(int index)
    {
        Open("ShopPopup");
        SetShopPopup();
        UpdateItemList(index);
    }
    // ShopItem 에서 ItemData파싱한 데이터를 불러와서
    // 정보팝업창 UI를 업데이트
    private void UpdateItemList(int index)
    {
        shopPopupText.text = shopItems[index].Description;
        shopPopupImage.sprite = shopItems[index].ShopItemImage.sprite;
        shopPopupNameText.text = shopItems[index].ShopItemName;
    }



    #region 기본여닫이
    public void Open(string name)
    {
        // Debug.Log($"{name} 패널을 엽니다");
        GetUI(name).SetActive(true);
    }

    public void GoBack(string name)
    {
        // Debug.Log($"{name} 패널을 닫습니다");
        GetUI(name).SetActive(false);
    }
    #endregion

}
