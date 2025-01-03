using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SignUpPanel : UI_Manager
{
    [SerializeField] TMP_InputField _signUpIDInputField;    
    [SerializeField] TMP_InputField _signUpPWInputField;
    [SerializeField] TMP_InputField _pWConfirmInputField;


   //  AuthError error = AuthError.EmailAlreadyInUse;
    private void Start()
    {
         Init();
        // 여기다 두니까 갑자기 밑에  "GetUI<Button>("SignUpConfirmButton").onClick.AddListener(SignUp);"
        // NullReferenceException: Object reference not set to an instance of an object 뜸
    }
    private void OnEnable()
    {
        // Init();
    }
    private void Init()
    {
        _signUpIDInputField = GetUI<TMP_InputField>("SignUpIDInputField");
        _signUpPWInputField = GetUI<TMP_InputField>("SignUpPWInputField");
        GetUI<Button>("SignUpConfirmButton").onClick.AddListener(SignUp);
        _pWConfirmInputField = GetUI<TMP_InputField>("PwConfirmInputField");
        
    }

    public void SignUp()
    {
        Debug.Log("SignUpButton 테스트 로그 : 회원가입버튼 눌림");

        string _email = _signUpIDInputField.text;
        string _password = _signUpPWInputField.text;
        string _confirmPW = _pWConfirmInputField.text;

        if (_email == "" || _password == "" || _confirmPW == "")
        {
            // TODO : 시간이된다면 팝업창으로 띄우기..?
            Debug.LogWarning("입력하지 않은 곳이 있습니다. \n다시한번 확인해주세요");

           // _checkPopup.SetActive(true);
           // _checkPopupMsg.text = "입력하지 않은 곳이 있습니다. \n다시한번 확인해주세요";
            return;
        }
        if (_password != _confirmPW)
        {
            Debug.LogWarning("패스워드가 일치하지 않습니다.");
            // _checkPopup.SetActive(true);
            // _checkPopupMsg.text = "패스워드가 일치하지 않습니다.";
            return;
        }
        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("회원가입이 취소되었습니다.");
               // _checkPopup.SetActive(true);
              //  _checkPopupMsg.text = "회원가입이 취소되었습니다.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogWarning("이메일/비밀번호 계정생성중 에러: " + task.Exception);
              //  _checkPopup.SetActive(true);
              //  _checkPopupMsg.text = $"{task.Exception} 에러가 일어났습니다.";
                return;
            }
            // 체크팝업 _checkPopup.SetActive(true);

            // 계정생성.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.Log($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");
           // _checkPopupMsg.text = $"회원가입을 축하드립니다! \n{result.User.UserId}로 인증메일을 확인해주세요.";
           // gameObject.SetActive(false);
        });
        SaveUserData();

    }
    

}
