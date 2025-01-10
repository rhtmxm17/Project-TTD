using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdvencedGainPopup : OpenableWindow
{
    [Header("HeaderText")]
    [SerializeField]
    TextMeshProUGUI headerText;

    [Header("GainItemPopup")]
    [SerializeField]
    ItemGainPopup itemGainPopup;

    [Header("EmptyGainText")]
    [SerializeField]
    GameObject emptyTextObj;

    [Header("Single windows component")]
    [SerializeField]
    GameObject singleButtonGroup;
    [SerializeField]
    TextMeshProUGUI singleButtonText;
    [SerializeField]
    Button singleButton;

    [Header("Doble windows component")]
    [SerializeField]
    GameObject doubleButtonGroup;
    [SerializeField]
    TextMeshProUGUI leftButtonText;
    [SerializeField]
    Button leftButton;
    [SerializeField]
    TextMeshProUGUI rightButtonText;
    [SerializeField]
    Button rightButton;

    void InitPrvSetting()
    {
        itemGainPopup.ClearAllItems();

        singleButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        emptyTextObj.SetActive(false);

        singleButtonGroup.SetActive(false);
        doubleButtonGroup.SetActive(false);
    }

    /// <summary>
    /// 버튼이 하나있는 스테이지 클리어 버튼
    /// </summary>
    /// <param name="headerText">상단에 표기될 텍스트</param>
    /// <param name="gainItems">획득한 아이템들의 리스트</param>
    /// <param name="buttonText">버튼에 표기될 텍스트</param>
    /// <param name="onClickAction">버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="isCloseOnClicked">버튼이 눌렸을 때 창을 닫을지의 여부</param>
    public void OpenSingleButton(in string headerText, List<ItemGain> gainItems, in string buttonText, UnityAction onClickAction, bool isCloseOnClicked)
    {

        InitPrvSetting();
        singleButtonGroup.SetActive(true);

        this.headerText.text = headerText;

        if (gainItems == null || gainItems.Count == 0)
        {
            emptyTextObj.SetActive(true);
        }
        else
        {
            itemGainPopup.AddItemGainCell(gainItems);
        }

        singleButtonText.text = buttonText;
        if(onClickAction != null)
            singleButton.onClick.AddListener(onClickAction);

        if (isCloseOnClicked)
        {
            singleButton.onClick.AddListener(CloseWindow);
        }

        OpenWindow();

    }

    /// <summary>
    /// 버튼이 두개 있는 스테이지 클리어 버튼
    /// </summary>
    /// <param name="headerText">상단에 표기될 텍스트</param>
    /// <param name="gainItems">획득한 아이템들의 리스트</param>
    /// <param name="leftButtonText">좌측 버튼에 표기될 텍스트</param>
    /// <param name="onLeftClickAction">좌측 버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="rightButtonText">우측 버튼에 표기될 텍스트</param>
    /// <param name="onRightClickAction">우측 버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="isCloseOnClicked">버튼이 눌렸을 때 창을 닫을지의 여부</param>
    public void OpenDoubleButton(in string headerText, List<ItemGain> gainItems, in string leftButtonText, UnityAction onLeftClickAction, in string rightButtonText, UnityAction onRightClickAction, bool isCloseOnClicked)
    {
        InitPrvSetting();
        doubleButtonGroup.SetActive(true);

        this.headerText.text = headerText;

        if (gainItems == null || gainItems.Count == 0)
        {
            emptyTextObj.SetActive(true);
        }
        else
        {
            itemGainPopup.AddItemGainCell(gainItems);
        }

        this.leftButtonText.text = leftButtonText;
        if(onLeftClickAction != null)
            leftButton.onClick.AddListener(onLeftClickAction);

        this.rightButtonText.text = rightButtonText;
        if(onRightClickAction != null)
            rightButton.onClick.AddListener(onRightClickAction);

        if (isCloseOnClicked)
        {
            leftButton.onClick.AddListener(CloseWindow);
            rightButton.onClick.AddListener(CloseWindow);
        }

        OpenWindow();

    }

    /// <summary>
    /// 버튼이 두개 있는 스테이지 클리어 버튼
    /// </summary>
    /// <param name="headerText">상단에 표기될 텍스트</param>
    /// <param name="gainItems">획득한 아이템들의 리스트</param>
    /// <param name="leftButtonText">좌측 버튼에 표기될 텍스트</param>
    /// <param name="onLeftClickAction">좌측 버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="rightButtonText">우측 버튼에 표기될 텍스트</param>
    /// <param name="onRightClickAction">우측 버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="isCloseOnLeftClicked">좌측 버튼이 눌렸을 때 창을 닫을지의 여부</param>
    /// <param name="isCloseOnRightClicked">우측 버튼이 눌렸을 때 창을 닫을지의 여부</param>
    public void OpenDoubleButton(in string headerText, List<ItemGain> gainItems, in string leftButtonText, UnityAction onLeftClickAction, in string rightButtonText, UnityAction onRightClickAction, bool isCloseOnLeftClicked, bool isCloseOnRightClicked)
    {
        InitPrvSetting();
        doubleButtonGroup.SetActive(true);

        this.headerText.text = headerText;

        if (gainItems == null || gainItems.Count == 0)
        {
            emptyTextObj.SetActive(true);
        }
        else
        {
            itemGainPopup.AddItemGainCell(gainItems);
        }

        this.leftButtonText.text = leftButtonText;
        if (onLeftClickAction != null)
            leftButton.onClick.AddListener(onLeftClickAction);

        this.rightButtonText.text = rightButtonText;
        if (onRightClickAction != null)
            rightButton.onClick.AddListener(onRightClickAction);

        if (isCloseOnLeftClicked)
        {
            leftButton.onClick.AddListener(CloseWindow);
        }

        if (isCloseOnRightClicked)
        {
            rightButton.onClick.AddListener(CloseWindow);
        }

        OpenWindow();

    }


}
