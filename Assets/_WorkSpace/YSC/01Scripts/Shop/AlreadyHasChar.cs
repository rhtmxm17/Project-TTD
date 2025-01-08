using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AlreadyHasChar : BaseUI
{
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


    #region  소유아이템
    [NonSerialized] List<int> haveItemIdxList = new List<int>();

    public bool HasItem(int itemIndex)
    {
        return haveItemIdxList.Contains(itemIndex);
    }

    public void ApplyItem(int itemIndex)
    {
        ItemData itemData = GameManager.TableData.GetItemData(itemIndex);
    }

    

    #endregion
    
    
    
    void OnEnable()
    {
        Debug.Log("더 이상 캐릭터를 강화할 수 없습니다. 그래도 구매하시겠습니까?");
        Init();
    }

    private void Init()
    {
        bgCancelButton = GetUI<Button>("CharWarningBG");
        bgCancelButton.onClick.AddListener(ClosePopup);
        cancelButton = GetUI<Button>("CancelButton");
        cancelButton.onClick.AddListener(ClosePopup);
        exitButton = GetUI<Button>("ExitButton");
        exitButton.onClick.AddListener(ClosePopup);
        purchaseButton = GetUI<Button>("PurchaseButton");
        warningText = GetUI<TMP_Text>("WarningText");
    }

    private void ClosePopup()
    {
        Destroy(this.gameObject);
    }
}
