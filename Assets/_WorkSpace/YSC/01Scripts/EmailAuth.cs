using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailAuth : UI_Manager
{
    [SerializeField] string _email;
    [SerializeField] TMP_InputField _emailAuthInputField;

    GameObject waitingPopup;
    // TODO:
    // 프로필 창을 만들어서
    // 닉네임(++변경버튼?), UID, 
    // 이메일인증 & 이메일가입 버튼 추가해서 계정연동(인증)해서 할 수 있도록하기
    // 프리펩화?

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        // 인증메일 팝업창
        GetUI("EmailAuthPopup");
        _emailAuthInputField = GetUI<TMP_InputField>("EmailAuthInputField");
        GetUI<Button>("SendButton").onClick.AddListener(SetEmail);

        waitingPopup = GetUI("WaitingPopup");
        waitingPopup.SetActive(false);
        GetUI<Button>("AuthCancelButton").onClick.AddListener(() => GoBack("WaitingPopup")); 
    }
    private void CheckVerification()
    {
        if (string.IsNullOrEmpty(_email))
        {
               
        }
    }
    private void SetEmail()
    {
        // 이메일이 없는 경우에만 할 수 있도록하기

        Firebase.Auth.FirebaseUser user = BackendManager.Auth.CurrentUser;
        _email = _emailAuthInputField.text;
        if (user != null)
        {
            user.SendEmailVerificationBeforeUpdatingEmailAsync(_email).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateEmailAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User email updated successfully.");
                Email = _email;
            });
            // 인증메일 보내기
            SendAuthEmail();
        }
    }
    private void SendAuthEmail()
    {
        Firebase.Auth.FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogWarning("SendEmailVerificationAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogWarning("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Email sent successfully.");
            });
            // 인증기다리는창 띄우기
            waitingPopup.SetActive(true);
        }
    }

    private void CancelVerification()
    {

    }
    
    
}
