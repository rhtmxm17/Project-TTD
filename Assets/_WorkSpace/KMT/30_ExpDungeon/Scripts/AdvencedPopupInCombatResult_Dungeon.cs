using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdvencedPopupInCombatResult_Dungeon : AdvencedPopupInCombatResult
{

    [SerializeField]
    Button thirdButton;
    [SerializeField]
    TextMeshProUGUI thirdButtonText;


    /// <summary>
    /// 버튼이 두개 있는 스테이지 클리어 버튼
    /// </summary>
    /// <param name="headerText">상단에 표기될 텍스트</param>
    /// <param name="gainItems">획득한 아이템들의 리스트</param>
    /// <param name="leftButtonText">좌측 버튼에 표기될 텍스트</param>
    /// <param name="onLeftClickAction">좌측 버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="middleButtonText">중앙 버튼에 표기될 텍스트</param>
    /// <param name="onMiddleClickAction">중앙 버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="rightButtonText">우측 버튼에 표기될 텍스트</param>
    /// <param name="onRightClickAction">우측 버튼이 눌렸을 때 반응할 행동</param>
    /// <param name="isCloseOnLeftClicked">좌측 버튼이 눌렸을 때 창을 닫을지의 여부</param>
    /// <param name="isCloseOnMiddleClicked">중앙 버튼이 눌렸을 때 창을 닫을지의 여부</param>
    /// <param name="isCloseOnRightClicked">우측 버튼이 눌렸을 때 창을 닫을지의 여부</param>
    /// <param name="resultText">결과로 출력할 텍스트</param>
    /// <param name="characterSprite">결과로 출력할 이미지 스프라이트</param>
    /// <param name="colorType">결과로 출력될 뒷배경의 타입</param>
    public void OpenTripleButtonWithResult(in string headerText, List<ItemGain> gainItems, in string leftButtonText, UnityAction onLeftClickAction, in string middleButtonText, UnityAction onMiddleClickAction, in string rightButtonText, UnityAction onRightClickAction, bool isCloseOnLeftClicked, bool isCloseOnMiddleClicked, bool isCloseOnRightClicked,
                                 in string resultText, Sprite characterSprite, ColorType colorType)
    {
        thirdButton.onClick.RemoveAllListeners();
        thirdButtonText.text = rightButtonText;

        if(onRightClickAction != null)
            thirdButton.onClick.AddListener(onRightClickAction);

        if(isCloseOnRightClicked)
            thirdButton.onClick.AddListener(CloseWindow);

        OpenDoubleButtonWithResult(headerText, gainItems, leftButtonText, onLeftClickAction, middleButtonText, onMiddleClickAction, isCloseOnLeftClicked, isCloseOnMiddleClicked,
            resultText, characterSprite, colorType);

    }

}
