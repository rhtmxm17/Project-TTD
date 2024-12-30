using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopPanel : BaseUI
{
    // TODO: 확인하려고 [SerializeField] 구별해서 바꾸기

    // 상점 판매 품목 데이터
    [SerializeField] List<ShopItemData> shopItems; // 나중에 HideInInspector

    // 상점 창 레이아웃
    [SerializeField] LayoutGroup shopLayoutGroup;
    [SerializeField] ShopItem shopCellPrefab;

    // 정보팝업창
    [SerializeField] public GameObject ShopPopup;

    // 상점이름, 추후에 상점종류많아지면 열고 닫을때 교체
    [SerializeField] TMP_Text shopNameText;

    // 팝업창
    [SerializeField] TMP_Text shopPopupText;
    [SerializeField] Image shopPopupImage;
    [SerializeField] TMP_Text shopPopupNameText;

    private List<ShopItem> shopItemsList;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        // 상점이름
        shopNameText = GetUI<TMP_Text>("ShopNameText");

        // 정보팝업
        ShopPopup = GetUI("ShopPopup");

        // 상점템 리스트 갱신
        ListUpItems();
        // 정보팝업 UI세팅
        SetShopPopup();
    }
   
    
    private void ListUpItems() // 상점아이템리스트 갱신
    {
        shopItemsList = new List<ShopItem>(shopItems.Count);

        foreach (ShopItemData shopitem in shopItems)
        {
            ShopItem cellInstance = Instantiate(shopCellPrefab, shopLayoutGroup.transform);
            cellInstance.SetItem(shopitem);
            cellInstance.GetComponent<Button>().onClick.AddListener(() => OpenPopup(cellInstance.shopItemData)); // 정보팝업 열기 버튼 추가
        }
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
    
    /// <summary>
    /// 인덱스번호에 할당된 아이템 팝업창을 열고
    /// 그 할당된 정보를 업뎃하고 보여주기
    /// </summary>
    /// <param name="data"></param>
    public void OpenPopup(ShopItemData data)
    {
        Open("ShopPopup");
        SetShopPopup();
        UpdateItemList(data);
    }

    // ShopItem 에서 ItemData파싱한 데이터를 불러와서
    // 정보팝업창 UI를 업데이트
    private void UpdateItemList(ShopItemData data)
    {
        shopPopupText.text = data.Description;
        shopPopupImage.sprite = data.Sprite;
        shopPopupNameText.text = data.ShopItemName;
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
