using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayUIManager : SingletonBehaviour<OverlayUIManager>
{
    [SerializeField] ItemGainPopup itemGainPopupPrefab;

    [SerializeField] SimpleInfoUI simpleInfoUI;
    [SerializeField] DoubleInfoUI doubleInfoUI;

    private void Awake()
    {
        RegisterSingleton(this);
    }

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

}
