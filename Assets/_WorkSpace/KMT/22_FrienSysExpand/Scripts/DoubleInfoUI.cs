using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoubleInfoUI : OpenableWindow
{
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    TextMeshProUGUI leftButtonText;
    [SerializeField]
    TextMeshProUGUI rightButtonText;

    [Header("Button")]
    [SerializeField]
    Button leftButton;
    [SerializeField]
    Button rightButton;

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

        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        leftButton.onClick.AddListener(() => { onLeftAction?.Invoke(); });
        leftButton.onClick.AddListener(CloseWindow);

        rightButton.onClick.AddListener(() => { onRightAction?.Invoke(); });
        rightButton.onClick.AddListener(CloseWindow);

        text.text = content;

        this.leftButtonText.text = leftButtonText;
        this.rightButtonText.text = rightButtonText;
        OpenWindow();
    }

    /// <summary>
    /// 정보 표기와 2개의 버튼이 존재하는 팝업 생성
    /// </summary>
    /// <param name="content">출력할 내용 문자열</param>
    /// <param name="leftButtonText">왼쪽 버튼에 출력될 내용 문자열</param>
    /// <param name="rightButtonText">오른쪽 버튼에 출력될 내용 문자열</param>
    /// <param name="onLeftAction">왼쪽 버튼이 눌릴때 실행시킬 행동</param>
    /// <param name="onRightAction">오른쪽 버튼이 눌릴때 실행시킬 행동</param>
    /// <param name="isCloseOnLeftClicked">왼쪽쪽 버튼이 눌릴때 창을 닫을지 여부</param>
    /// <param name="isColseOnRightClicked">오른쪽 버튼이 눌릴때 창을 닫을지 여부</param>
    public void SetAndOpenDoubleWindow(in string content,
                                        in string leftButtonText, in string rightButtonText,
                                        Action onLeftAction, Action onRightAction,
                                        bool isCloseOnLeftClicked, bool isColseOnRightClicked)
    {
        if (gameObject.activeSelf)
            return;

        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        leftButton.onClick.AddListener(() => { onLeftAction?.Invoke(); });
        if(isCloseOnLeftClicked)
            leftButton.onClick.AddListener(CloseWindow);

        rightButton.onClick.AddListener(() => { onRightAction?.Invoke(); });
        if(isColseOnRightClicked)
            rightButton.onClick.AddListener(CloseWindow);

        text.text = content;
        this.leftButtonText.text = leftButtonText;
        this.rightButtonText.text = rightButtonText;
        OpenWindow();
    }

}
