using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SignUpPanel : UI_Manager
{
    [SerializeField] TMP_InputField _signUpIDInputField;    
    [SerializeField] TMP_InputField _signUpPWInputField;


    AuthError error = AuthError.EmailAlreadyInUse;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _signUpIDInputField = GetUI<TMP_InputField>("SignUpIDInputField");
        _signUpPWInputField = GetUI<TMP_InputField>("SignUpPWInputField");
        GetUI<Button>("SignUpConfirmButton").onClick.AddListener(SignUp);
    }

    public void SignUp()
    {
        Debug.Log("SignUpButton 테스트 로그 : 회원가입버튼 눌림");

        string _email = _signUpIDInputField.text;
        string _password = _signUpPWInputField.text;
       // string _confirmPW = _PWConfirmInputField.text;

        if (_email == "" || _password == "")
        {
            // TODO : 시간이된다면 팝업창으로 띄우기..?
            Debug.LogWarning("입력하지 않은 곳이 있습니다. \n다시한번 확인해주세요");

           // _checkPopup.SetActive(true);
           // _checkPopupMsg.text = "입력하지 않은 곳이 있습니다. \n다시한번 확인해주세요";
            return;
        }
      //  if (_password != _confirmPW)
      //  {
      //      Debug.LogWarning("패스워드가 일치하지 않습니다.");
      //      _checkPopup.SetActive(true);
      //      _checkPopupMsg.text = "패스워드가 일치하지 않습니다.";
      //      return;
      //  }
        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
               // _checkPopup.SetActive(true);
              //  _checkPopupMsg.text = "회원가입이 취소되었습니다.";
                return;
            }
            if (task.IsFaulted)
            {
                // 일단 인증에러 팝업창나게하는거 중지
                Firebase.FirebaseException exception = task.Exception.InnerException as Firebase.FirebaseException;
                switch ((AuthError)exception.ErrorCode)
                {

                    case AuthError.EmailAlreadyInUse:
                        Debug.LogWarning($"이메일이 이미 사용중입니다.");
                       // _checkPopup.SetActive(true);
                       // _checkPopupMsg.text = "이메일이 이미 사용중입니다.";
                        break;
                    case AuthError.WeakPassword:
                        Debug.LogWarning($"비밀번호가 너무 쉽습니다.");
                       // _checkPopup.SetActive(true);
                       // _checkPopupMsg.text = "비밀번호가 너무 쉽습니다. \n다른비밀번호를 사용해주세요.";
                        break;
                    case AuthError.WrongPassword:
                        Debug.LogWarning($"잘못된 비밀번호입니다.: {exception.ErrorCode}");
                       // _checkPopupMsg.text = "비밀번호가 틀립니다.. \n바르게 입력하세요.";
                        break;

                }


                Debug.LogWarning("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
              //  _checkPopup.SetActive(true);
              //  _checkPopupMsg.text = $"{task.Exception} 에러가 일어났습니다.";
                return;
            }
            // 체크팝업 _checkPopup.SetActive(true);

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.Log($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");
           // _checkPopupMsg.text = $"회원가입을 축하드립니다! \n{result.User.UserId}로 인증메일을 확인해주세요.";
           // gameObject.SetActive(false);
        });
       

    }


}
