using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoubleInfoUI : OpenableWindow
{
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    TextMeshProUGUI leftButtonText;
    [SerializeField]
    TextMeshProUGUI rightButtonText;


    Action leftAction = null;
    Action rightAction = null;

    /// <summary>
    /// 정보 표기와 2개의 버튼이 존재하는 팝업 생성
    /// </summary>
    /// <param name="content">출력할 내용 문자열</param>
    /// <param name="leftButtonText">왼쪽 버튼에 출력될 내용 문자열</param>
    /// <param name="rightButtonText">오른쪽 버튼에 출력될 내용 문자열</param>
    /// <param name="onLeftAction">왼쪽 버튼이 눌릴때 실행시킬 행동</param>
    /// <param name="onRightAction">오른쪽 버튼이 눌릴때 실행시킬 행동</param>
    public void SetAndOpenDoubleWindow(in string content, 
                                        in string leftButtonText, in string rightButtonText,
                                        Action onLeftAction, Action onRightAction)
    {
        if (gameObject.activeSelf)
            return;

        leftAction = onLeftAction;
        rightAction = onRightAction;
        text.text = content;
        this.leftButtonText.text = leftButtonText;
        this.rightButtonText.text = rightButtonText;
        OpenWindow();
    }

    public void OnClickLeftButton()
    {
        CloseWindow();
        leftAction?.Invoke();
    }

    public void OnClickRightButton()
    {
        CloseWindow();
        rightAction?.Invoke();
    }

}
