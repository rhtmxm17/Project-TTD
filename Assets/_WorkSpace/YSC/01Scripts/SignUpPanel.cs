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
   // private void OnEnable()
   // {
   //     Init();
   // }

    private void Init()
    {
        // UI_Binding
        _signUpIDInputField = GetUI<TMP_InputField>("SignUpIDInputField");      // 아이디   입력란
        _signUpPWInputField = GetUI<TMP_InputField>("SignUpPWInputField");      // 비밀번호 입력란
        _pWConfirmInputField = GetUI<TMP_InputField>("PwConfirmInputField");    // 비번확인 입력란
        GetUI<Button>("SignUpConfirmButton").onClick.AddListener(SignUp);       // 회원가입 버튼.(가입기능)
    //    GetUI<Button>("CloseButton").onClick.AddListener(()=>gameObject.SetActive(false));       // 나가기
        
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
                  
            case AuthError.Unimplemented:
                message = "-1";
                break;
            case AuthError.None:
                message = "없음? 뭐임";
                break;
            case AuthError.Failure:
                message = "이메일/비밀번호가 틀렸습니다."; // 1 실패: 이게 그냥 아이디 / 비밀번호? 잘못 입력되면 나온느거 같음 유니티상에 인풋필드에
                break;
            case AuthError.InvalidCustomToken:
                message = "유효치않은 토큰";
                break;
            case AuthError.CustomTokenMismatch:
                message = "불일치 커스텀 토큰.";
                break;
            case AuthError.InvalidCredential:
                message = "유효치않은 인증정보";
                break;
            case AuthError.ProviderAlreadyLinked:
                message = "이미 연동된 계정입니다.";
                break;
            case AuthError.NoSuchProvider:
                message = "제공되지 않는 연동방식입니다.";
                break;
            case AuthError.UserTokenExpired:
                message = "만료된 유저 토큰입니다";
                break;
            case AuthError.NetworkRequestFailed:
                message = "네트워크 요청실패.";
                break;
            case AuthError.InvalidApiKey:
                message = "잘못된 API키";
                break;
            case AuthError.AppNotAuthorized:
                message = "권한이 없습니다.";
                break;
            case AuthError.UserMismatch:
                message = "유저 매칭이 잘못되었습니다.";
                break;
            case AuthError.WeakPassword:
                message = "너무 취약한 비밀번호입니다.\n(6자이상)";
                break;
            case AuthError.NoSignedInUser:
                message = "가입된 유저가 아닙니다.";
                break;
            case AuthError.ExpiredActionCode:
                message = "만료된 세션 입니다.";
                break;
            case AuthError.InvalidActionCode:
                message = "유요치 않은 액션코드/세션 입니다.";
                break;
            case AuthError.InvalidMessagePayload:
                message = "잘못된 메세지 페이로드 입니다.";
                break;
            case AuthError.InvalidPhoneNumber:
                message = "유요하지 않은 번호입니다.";
                break;
            case AuthError.MissingPhoneNumber:
                message = "전호번호가 누락되었습니다.";
                break;
            case AuthError.InvalidRecipientEmail:
                message = "유효치않은 수신자 이메일입니다.";
                break;
            case AuthError.InvalidSender:
                message = "유효하지 않은 송신자 입니다.";
                break;
            case AuthError.InvalidVerificationCode:
                message = "유효하지 않은 인증코드 입니다.";
                break;
            case AuthError.InvalidVerificationId:
                message = "유효치않은 인증아이디입니다.";
                break;
            case AuthError.MissingVerificationCode:
                message = "인증코드가 누락되었습니다.";
                break;
            case AuthError.MissingVerificationId:
                message = "인증아이디가 누락되었습니다.";
                break;
            case AuthError.QuotaExceeded:
                message = "할당량이 초과되었습니다.";
                break;
            case AuthError.RetryPhoneAuth:
                message = "번호 인증을 다시 시도하십시오.";
                break;
            case AuthError.AppNotVerified:
                message = "인증되지 않은 앱입니다. ";
                break;
            case AuthError.AppVerificationFailed:
                message = "앱인증에 실패하였습니다.";
                break;
            case AuthError.CaptchaCheckFailed:
                message = "당신은 로봇입니까? 로봇이아님을 인증하십쇼.";
                break;
            case AuthError.InvalidAppCredential:
                message = "올바른 접근방식이 아닙니다.";
                break;
            case AuthError.MissingAppCredential:
                message = "인증되지 않은 접근입니다.";
                break;
            case AuthError.InvalidClientId:
                message = "잘못된 클라이언트입니다.";
                break;
            case AuthError.InvalidContinueUri:
                message = "유효치 않은 ContinueURL입니다.";
                break;
            case AuthError.MissingContinueUri:
                message = "ContinueURL이 존재하지 않습니다.";
                break;
            case AuthError.KeychainError:
                message = "키체인 오류!";
                break;
            case AuthError.MissingAppToken:
                message = "앱에대한 토큰이 존재하지 않습니다.";
                break;
            case AuthError.MissingIosBundleId:
                message = "IOS번들ID가 존재하지 않습니다.";
                break;
            case AuthError.NotificationNotForwarded:
                message = "알림이 전달되지 않았습니다.";
                break;
            case AuthError.UnauthorizedDomain:
                message = "허용되지않은 도메인접근 입니다.";
                break;
            case AuthError.WebContextAlreadyPresented:
                message = "이미 존재하는 웹컨텍스트입니다.";
                break;
            case AuthError.WebContextCancelled:
                message = "웹컨텍스트가 취소 됐습니다.";
                break;
            case AuthError.DynamicLinkNotActivated:
                message = "다이나믹 링크가 비활성화 됐습니다.";
                break;
            case AuthError.Cancelled:
                message = "취소되었습니다.";
                break;
            case AuthError.InvalidProviderId:
                message = "잘못된 아이디입니다.";
                break;
            case AuthError.WebStorateUnsupported:
                message = "지원되지않는 웹 저장방식입니다.";
                break;
            case AuthError.TenantIdMismatch:
                message = "GUID(Tenanat) 아이디가 일치 하지 않습니다.";
                break;
            case AuthError.UnsupportedTenantOperation:
                message = "지원하지않는 클라이언트 실행입니다.";
                break;
            case AuthError.InvalidLinkDomain:
                message = "유효하지않은 도메인링크입니다.";
                break;
            case AuthError.RejectedCredential:
                message = "인증이 거부되었습니다.";
                break;
            case AuthError.PhoneNumberNotFound:
                message = "전화번호를 찾지 못하였습니다.";
                break;

            case AuthError.InvalidTenantId:
                message = "유요하지 않은 GUID(TenantID)입니다.";
                break;
            case AuthError.MissingClientIdentifier:
                message = "클라이언트 식별이 되지않습니다.";
                break;
            case AuthError.MissingMultiFactorSession:
                message = "MFA(Multi Factor Session)이 존재하지않습니다.";
                break;
            case AuthError.MissingMultiFactorInfo:
                message = "MFA(Multi Factor Session)정보가 존재하지않습니다.";
                break;
            case AuthError.InvalidMultiFactorSession:
                message = "유요하지않은 MFA(Multi Factor Session) 입니다.";
                break;
            case AuthError.MultiFactorInfoNotFound:
                message = "MFA(Multi Factor Session)정보를 찾을 수 없습니다.";
                break;
            case AuthError.AdminRestrictedOperation:
                message = "관리자에 의해 제한되었습니다.";
                break;
            case AuthError.UnverifiedEmail:
                message = "인증되지않은 이메일입니다.";
                break;
            case AuthError.SecondFactorAlreadyEnrolled:
                message = "2단계 인증절차가 이미 등록되어있습니다.";
                break;
            case AuthError.MaximumSecondFactorCountExceeded:
                message = "2단계 인증절차 제한수를 초과합니다";
                break;
            case AuthError.UnsupportedFirstFactor:
                message = "지원하지않은 인증절차입니다.";
                break;
            case AuthError.EmailChangeNeedsVerification:
                message = "이메일은 변경에 인증이 필요합니다.";
                break;
        }
        return message;
    }
    #endregion
    

}
