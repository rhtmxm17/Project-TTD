using System;
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

    // 정보팝업창 - 이제 안씀
    // [SerializeField] SimpleInfoPopup shopPopupPrefab;

    // 상점이름, 추후에 상점종류많아지면 열고 닫을때 교체
  //  [SerializeField] TMP_Text shopNameText;

    [Header("현제 상점 표시관련")]
    [SerializeField] Button tabButton;
    [SerializeField] Material selectedMaterial;
    [SerializeField] TMP_Text shopChatText;
    
    private List<ShopItem> shopItemsList;
    

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        tabButton.GetComponent<Image>().material = selectedMaterial;
    }
    private void OnDisable()
    {
        tabButton.GetComponent<Image>().material = null;
    }


    private void Init()
    {
        // 상점이름
       // shopNameText = GetUI<TMP_Text>("ShopNameText");

        // 상점템 리스트 갱신
        ListUpItems();
    }
   
    
    private void ListUpItems() // 상점아이템리스트 갱신
    {
        shopItemsList = new List<ShopItem>(shopItems.Count);

        foreach (ShopItemData shopitem in shopItems)
        {
            ShopItem cellInstance = Instantiate(shopCellPrefab, shopLayoutGroup.transform);
            cellInstance.SetItem(shopitem);
           // 이제안써서 합치고 추후에 삭제 cellInstance.GetComponent<Button>().onClick.AddListener(() => OpenPopup(cellInstance.shopItemData)); // 정보팝업 열기 버튼 추가
        }
    }
    
    /// <summary>
    /// 인덱스번호에 할당된 아이템 팝업창을 열고
    /// 그 할당된 정보를 업뎃하고 보여주기
    /// </summary>
    /// <param name="data"></param>
    
    /* 정보만나오는거 안씀
    public void OpenPopup(ShopItemData data)
    {
        SimpleInfoPopup popupInstance = Instantiate(shopPopupPrefab, GameManager.PopupCanvas);
        popupInstance.Title.text = data.ShopItemName;
        popupInstance.Description.text = data.Description;
        popupInstance.Image.sprite = data.Sprite;
    }
    */


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
