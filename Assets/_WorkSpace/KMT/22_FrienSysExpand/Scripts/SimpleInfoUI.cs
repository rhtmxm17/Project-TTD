using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoUI : OpenableWindow
{

    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    TextMeshProUGUI closeButtonText;


    Action closeAction = null;

    /// <summary>
    /// 단순 정보 표기와 닫기버튼만 존재하는 팝업 생성
    /// </summary>
    /// <param name="content">출력할 내용 문자열</param>
    /// <param name="closeButtonText">닫기 버튼에 출력될 내용 문자열</param>
    /// <param name="onCloseAction">창이 닫힐 때 실행시킬 action, 없으면 null 전달</param>
    public void SetAndOpenSimpleWindow(in string content, in string closeButtonText, Action onCloseAction)
    {
        if (gameObject.activeSelf)
            return;

        closeAction = null;
        closeAction += onCloseAction;
        text.text = content;
        this.closeButtonText.text = closeButtonText;
        OpenWindow();
    }

    public override void CloseWindow()
    {
        base.CloseWindow();
        closeAction?.Invoke();
    }
}
