using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OverlayUIManager : SingletonBehaviour<OverlayUIManager>
{
    [SerializeField] ItemGainPopup itemGainPopupPrefab;

    [SerializeField] SimpleInfoUI simpleInfoUI;
    [SerializeField] DoubleInfoUI doubleInfoUI;

    [SerializeField] AdvencedGainPopup advencedGainPopupUI;

    private void Awake()
    {
        RegisterSingleton(this);
    }

    /// <summary>
    /// 아이템 획득 팝업 생성 함수<br/>
    /// 반환된 ItemGainPopup 참조를 통해 추가로 타이틀 등을 변경할 수 있음
    /// </summary>
    /// <param name="gain">획득한 아이템 종류와 개수 정보</param>
    /// <returns>생성된 팝업을 반환</returns>
    public ItemGainPopup PopupItemGain(List<ItemGain> gain = null)
    {
        ItemGainPopup popupInstance = Instantiate(itemGainPopupPrefab, GameManager.PopupCanvas.transform);
        if (gain != null)
        {
            popupInstance.AddItemGainCell(gain);
        }

        return popupInstance;
    }
    
    /// <summary>
    /// ItemGainPopup에 사용을 위한 함수
    /// </summary>
    /// <param name="item"></param>
    public ItemGainPopup PopupItemGainForMax(ItemData gain = null)
    {
        ItemGainPopup popupInstance = Instantiate(itemGainPopupPrefab, GameManager.PopupCanvas.transform);
        if (gain != null)
        {
            popupInstance.AddItemGainCell(gain);
        }
        return popupInstance;
    }

    /// <summary>
    /// 단순 정보 표기와 닫기버튼만 존재하는 팝업 생성
    /// </summary>
    /// <param name="content">출력할 내용 문자열</param>
    /// <param name="closeButtonText">닫기 버튼에 출력될 내용 문자열</param>
    /// <param name="onCloseAction">창이 닫힐 때 실행시킬 action, 없으면 null 전달</param>
    public void OpenSimpleInfoPopup(in string content, in string closeButtonText, Action onCloseAction)
    {
        simpleInfoUI.SetAndOpenSimpleWindow(content, closeButtonText, onCloseAction);
    }

    /// <summary>
    /// 정보 표기와 2개의 버튼이 존재하는 팝업 생성
    /// </summary>
    /// <param name="content">출력할 내용 문자열</param>
    /// <param name="leftButtonText">왼쪽 버튼에 출력될 내용 문자열</param>
    /// <param name="rightButtonText">오른쪽 버튼에 출력될 내용 문자열</param>
    /// <param name="onLeftAction">왼쪽 버튼이 눌릴때 실행시킬 행동</param>
    /// <param name="onRightAction">오른쪽 버튼이 눌릴때 실행시킬 행동</param>
    public void OpenDoubleInfoPopup(in string content,
                                        in string leftButtonText, in string rightButtonText,
                                        Action onLeftAction, Action onRightAction)
    {
        doubleInfoUI.SetAndOpenDoubleWindow(content, leftButtonText, rightButtonText, onLeftAction, onRightAction);
    }

    /// <summary>
    /// 버튼이 하나있는 스테이지 클리어 버튼
    /// </summary>
    /// <param name="headerText">상단에 표기될 텍스트</param>
    /// <param name="gainItems">획득한 아이템들의 리스트</param>
    /// <param name="buttonText">버튼에 표기될 텍스트</param>
    /// <param name="onClickAction">버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="isCloseOnClicked">버튼이 눌렸을 때 창을 닫을지의 여부</param>
    public void OpenAdvencedSingleGainItemPopup(in string headerText, List<ItemGain> gainItems, in string buttonText, UnityAction onClickAction, bool isCloseOnClicked)
    { 
        advencedGainPopupUI.OpenSingleButton(headerText, gainItems, buttonText, onClickAction, isCloseOnClicked);
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
    public void OpenAdvencedDoubleGainItemPopup(in string headerText, List<ItemGain> gainItems, in string leftButtonText, UnityAction onLeftClickAction, in string rightButtonText, UnityAction onRightClickAction, bool isCloseOnClicked)
    {
        advencedGainPopupUI.OpenDoubleButton(headerText, gainItems, leftButtonText, onLeftClickAction, rightButtonText, onRightClickAction, isCloseOnClicked);
    }

}
