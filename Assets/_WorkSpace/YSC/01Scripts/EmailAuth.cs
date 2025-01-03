using Firebase.Auth;
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
        // 익명로그인한거에서 이메일을 추가하려고 사용하려는데 
        // 중요한정보 변경이라고 최근접속기록없다고 재인증하라는데 
        // 필요한 매개변수가 이메일이랑 비밀번호임 이거뭐 이건안될듯.
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
           // SendAuthEmail();
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
    // 익명에서 이메일set할때 재인증을 하라고함
    // 
    private void Reauthenticate()
    {
        Firebase.Auth.FirebaseUser user = BackendManager.Auth.CurrentUser;

        // Get auth credentials from the user for re-authentication. The example below shows
        // email and password credentials but there are multiple possible providers,
        // such as GoogleAuthProvider or FacebookAuthProvider.
        // 아니 근데 익명로그인은 어쩔
        Firebase.Auth.Credential credential =
            Firebase.Auth.EmailAuthProvider.GetCredential("user@example.com", "password1234");

        if (user != null)
        {
            user.ReauthenticateAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("ReauthenticateAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("ReauthenticateAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User reauthenticated successfully.");
            });
        }
    }

    



    private void CancelVerification()
    {

    }
    
    
}
