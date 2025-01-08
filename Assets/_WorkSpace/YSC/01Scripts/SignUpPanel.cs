using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class SignUpPanel : LoginPanel
{
    // 인풋필드
    [Header("입력란")]
    [SerializeField] TMP_InputField _signUpIDInputField;    // [InputField] 아이디 입력란
    [SerializeField] TMP_InputField _signUpPWInputField;    // [InputField] 비밀번호 입력란
    [SerializeField] TMP_InputField _pWConfirmInputField;   // [InputField] 비밀번호 확인 입력란
    [Header("팝업프리펩")]
    [SerializeField] PopupContoller messagePopup;


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
        // UI_Binding
        _signUpIDInputField = GetUI<TMP_InputField>("SignUpIDInputField");      // 아이디   입력란
        _signUpPWInputField = GetUI<TMP_InputField>("SignUpPWInputField");      // 비밀번호 입력란
        _pWConfirmInputField = GetUI<TMP_InputField>("PwConfirmInputField");    // 비번확인 입력란
        GetUI<Button>("SignUpConfirmButton").onClick.AddListener(SignUp);       // 회원가입 버튼.(가입기능)
        GetUI<Button>("CloseButton").onClick.AddListener(()=>gameObject.SetActive(false));       // 나가기
        
    }

    public void SignUp()
    {
        Debug.Log("SignUpButton 테스트 로그 : 회원가입버튼 눌림");
        // 변수할당
        string _email = _signUpIDInputField.text;
        string _password = _signUpPWInputField.text;
        string _confirmPW = _pWConfirmInputField.text;

        if (_email == "" || _password == "" || _confirmPW == "")
        {
            // TODO : 시간이된다면 팝업창으로 띄우기..?
            Debug.LogWarning("입력하지 않은 곳이 있습니다. \n다시한번 확인해주세요");
            messagePopup.gameObject.SetActive(true);   
            messagePopup.Mesage.text = $"입력하지 않은 곳이 있습니다. \n다시한번 확인해주세요";


            return;
        }
        if (_password != _confirmPW)
        {
            Debug.LogWarning("패스워드가 일치하지 않습니다.");
            messagePopup.gameObject.SetActive(true);   
            messagePopup.Mesage.text = $"패스워드가 일치하지 않습니다.";

            return;
        }
        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("회원가입이 취소되었습니다.");
                Debug.LogError($"Failed because {task.Exception}");
                GetErrorMessage(task.Exception);
                messagePopup.gameObject.SetActive(true);   
                messagePopup.Mesage.text = $"{GetErrorMessage(task.Exception)}";
                return;
            }
            if (task.IsFaulted)
            {
                GetErrorMessage(task.Exception);
                Debug.LogError($"Failed because {task.Exception}");
                messagePopup.gameObject.SetActive(true);   
                messagePopup.Mesage.text = $"{GetErrorMessage(task.Exception)}";
                
                /* 에러종류뽑기 밑에옮김
            //    var exception = task.Exception.InnerExceptions[0] as FirebaseException;
            //    var ErrorCode = (AuthError)exception.ErrorCode;
            //    print(ErrorCode);
            //    switch (ErrorCode)
            //    {
            //        case AuthError.MissingEmail:
            //            Debug.LogWarning($" 계정생성중 에러: {ErrorCode}" );
            //            messagePopup.Mesage.text = "이메일을 입력하세요";
            //            break;
            //        case AuthError.InvalidEmail:
            //            Debug.LogWarning($" 계정생성중 에러: {ErrorCode}");
            //            messagePopup.Mesage.text = "올바른 이메일을 입력하세요";
            //        break;
            //        case AuthError.EmailAlreadyInUse:
            //            Debug.LogWarning($" 계정생성중 에러: {ErrorCode}" );
            //            messagePopup.Mesage.text =  "이미 사용중인 이메일입니다.";
            //        break;
            //    }
            */

                return;
            }
            // 체크팝업 _checkPopup.SetActive(true);

            // 계정생성.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.Log($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");
            // TODO: 이메일인증 추가 해서 이메일 인증 되야지 되도록? 기획안 보고 그대로 적용하기.
           // _checkPopupMsg.text = $"회원가입을 축하드립니다! \n{result.User.UserId}로 인증메일을 확인해주세요.";
           // gameObject.SetActive(false);
        });
        // SaveUserData();

    }

    #region 에러관련
    public string GetErrorMessage(Exception exception)
    {
        Debug.Log(exception.ToString());
        FirebaseException firebaseException = exception.InnerException as FirebaseException;
        if (firebaseException != null)
        {
            var errorCode = (AuthError)firebaseException.ErrorCode;
            return GetErrorMessage(errorCode);
        }
        return exception.ToString();
    }

    private string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                message = "이메일을 입력하세요";
                break;
            case AuthError.InvalidEmail:
                message = "올바른 이메일을 입력하세요";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "이미 사용중인 이메일입니다.";
                break;
            case AuthError.MissingPassword:
                message = "비밀번호를 입력하세요";
                break;
            case AuthError.SessionExpired:
                message = "제공된 Firebase 세션 쿠키가 만료되었습니다.";
                break;
            case AuthError.UserDisabled:
                message = "정지된 계정입니다.";
                break;
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "다른인증으로 이미 가입된 아이디입니다.";
                break;
            case AuthError.OperationNotAllowed:
                message = "제공된 로그인 제공업체가 Firebase 프로젝트에서 사용 중지되었습니다.";
                break;
            case AuthError.RequiresRecentLogin:
                message = "최근 로그인 기록이 필요합니다.";
                break;
            case AuthError.CredentialAlreadyInUse:
                message = "이미 사용중인 인증방법입니다.";
                break;
            case AuthError.WrongPassword:
                message = "잘못된 비밀번호 입니다.";
                break;
            case AuthError.TooManyRequests:
                message = "요청 수가 최대 허용치를 초과합니다.";
                break;
            case AuthError.UserNotFound:
                message = "제공된 식별자에 해당하는 기존 사용자 레코드가 없습니다.";
                break;
            case AuthError.InvalidUserToken:
                message = "제공된 ID 토큰이 올바른 Firebase ID 토큰이 아닙니다.";
                break;
            case AuthError.ApiNotAvailable:
                message = "불가능한 api입니다.";
                break;
            case AuthError.WebInternalError:
                message = "요청을 처리하려고 시도하는 중에 Authentication 서버에 예기치 않은 오류가 발생했습니다. ";
                break;
            
        }
        return message;
    }
    #endregion
    

}
