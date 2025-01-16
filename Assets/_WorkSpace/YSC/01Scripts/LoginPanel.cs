using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BaseUI
{
    // User Data
    [SerializeField] protected string UserID;
    [SerializeField] protected string Nickname;
    [SerializeField] protected string Email;
    [SerializeField] protected bool _isVerified; // Email인증 된 유저인가
    [SerializeField] private bool _isLoggedIn; // 이미 로그인된 유저인가
    
    
    // 로그인 입력란
    [SerializeField] private TMP_InputField loginIDInputField;      // 이메일  
    [SerializeField] private TMP_InputField loginPWInputField;    // 비밀번호     
    
    // 경고창
    [SerializeField] GameObject warningPopup;
    [SerializeField] TMP_Text warningText;
    [SerializeField] Image warningImage;

    //비밀번호 리셋
    [SerializeField] GameObject findPWPanel;
    [SerializeField] private TMP_InputField resetPWInputField;
    
    void Start()
    {
        Init();
    }

    private void Init()
    {
        // 로그인 Button
        GetUI<Button>("LoginButton").onClick.AddListener(EmailLogin);
        // Email가입 버튼
        GetUI<Button>("SignUpButton").onClick.AddListener(() => Open("SignUpPanel"));
        // Email 인증 버튼
        GetUI<Button>("EmailAuthButton").onClick.AddListener(() => Open("EmailAuthPopup"));
        
       // GetUI<TMP_Text>("EmailText").text = "이메일을 입력하세요.";
       // GetUI<TMP_Text>("PwText").text = "비밀번호를 입력하세요.";
        loginIDInputField = GetUI<TMP_InputField>("LoginIDInputField");
        loginPWInputField = GetUI<TMP_InputField>("LoginPWInputField");
        
        
        // 경고팝업
        warningPopup = GetUI("WarningPopup");
        GetUI<Button>("PopupCloseButton").onClick.AddListener(() => Close("WarningPopup"));
        warningText = GetUI<TMP_Text>("WarningText");
        warningImage = GetUI<Image>("warningImage");

        
        // 비밀번호 찾기
        GetUI<Button>("FindPWButton").onClick.AddListener(() => Open("FindPWPanel"));
        resetPWInputField = GetUI<TMP_InputField>("ResetPWInputField");
        GetUI<Button>("ResetPWButton").onClick.AddListener(ResetPW);
        GetUI<Button>("ResetPWCloseButton").onClick.AddListener(() => Close("FindPWPanel"));

        
        // 인증메일 팝업창
        GetUI("EmailAuthPopup");
        
    }
    
    public void EmailLogin()
    {
    
        string email = loginIDInputField.text;
        if (email == "")
        {
            Debug.Log("이메일을 입력 해 주세요.");
            warningPopup.SetActive(true);   
            warningText.text = "이메일을 입력 해 주세요.";
            return;

        }
        string password = loginPWInputField.text;
        if (password == "")
        {
            Debug.Log("비밀번호를 입력 하세요.");
            warningPopup.SetActive(true);   
            warningText.text = "비밀번호를 입력 하세요.";
            return;
        }
        // 이메일 인증
        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                   //  Debug.LogWarning("SignInWithEmailAndPasswordAsync was canceled.");
                    // GetErrorMessage(task.Exception);
                    Debug.LogError($"취소되었습니다, 사유: {task.IsCanceled}");
                    warningPopup.SetActive(true);   
                    warningText.text = $"취소되었습니다, 사유: {task.IsCanceled}";
                    
                    
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"실패하였습니다, 사유: {task.Exception}");
                    warningPopup.SetActive(true);   
                    warningText.text = $"{GetErrorMessage(task.Exception)}";
                    // GetUI<TMP_Text>("NotificationText").text = $"올바른 아이디/비밀번호를\n 입력해주세요.";
                    // OpenNotifiaction();
                    return;
                }

                AuthResult result = task.Result;
                Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
                CheckUserInfo();
                warningPopup.SetActive(true);   
                warningText.text = "로그인 성공~!";
            });
    }

    public void SaveUserData()
    {
        // TODO:  DB관리하는 거보고 참조해서 구성하기
        /*
        DatabaseReference userRef = BackendManager.Database.GetReference( UID경로 $"Users/UserID/{UserID}"); 
        Dictionary<string, object> userData = new Dictionary<string, object>()
        {
            // 저장되야될것들?
            // {"UID", UserID},
            // {"Nickname", Nickname },
            // {"Email", Email },
            // {"IsVerified", IsVerified },
            // {"Last Login", DateTime.UtcNow.ToString("O")}
        };
        userRef.SetValueAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("사용자 데이터 저장 완료!");
                return;
            }
            else
            {
                Debug.LogError($"사용자 데이터 저장 실패 {task.Exception}");
            }
        });
        */
    }
    
    public void CheckUserData() // FBDB에 저장된 UID를 비교하여서 기존사용자면 로그인
    {
        DatabaseReference userRef = BackendManager.Database.GetReference($"Users/UserID");
        userRef.Child(UserID);
        Debug.Log($"userRef 확인 {userRef}");

        userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log($"데이터읽기가 취소됐습니다 : {task.Exception}");
            }
            if (task.IsFaulted)
            {
                Debug.Log($"데이터를 불러오는데 뭔가 잘못됐습니다.");
            }
            if (task.IsCompleted && task.Result.Exists)
            {
                var UserData = task.Result.Value as Dictionary<string, object>;
                if (UserData != null)
                {
                    foreach (var entry in UserData)
                    {
                        if (entry.Value is Dictionary<string, object> userData && UserData.ContainsKey("UID"))
                        {
                            UserID = userData["UID"].ToString();
                            Nickname = userData["Nickname"].ToString();
                            Debug.Log($"기존 사용자 UID 확인: {UserID}");
                            Debug.Log($"기존 사용자 UID 확인: {Nickname}");
                            _isLoggedIn = true;
                            // 자동시작
                            // AutomaticStart(); 다른것들테스트해야되서 일단 비활성화 
                            return;
                        }
                    }
                }
            }
            Debug.Log($"태스크이즈컴플리티드: {task.IsCompleted}");
            Debug.Log($"태스크리저트익시스츠: {task.Result.Exists}");
            Debug.Log("기존 사용자 UID 못찾았습니다, UID생성");
            Debug.Log($"기존 사용자 UID 확인: {UserID}");
            
            // TODO: 로그인

        });
    }


    public void ResetPW()
    {
        string email = resetPWInputField.text;
        BackendManager.Auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogWarning("SendPasswordResetEmailAsync was canceled.");
                warningPopup.SetActive(true);
                warningText.text = "비밀번호 리셋이 취소되었습니다.";
                return;
            }
            if (task.IsFaulted)
            {
                // Debug.LogWarning("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                //에러등으로 실패했다는 알림창
                Debug.LogError($"실패하였습니다, 사유: {task.Exception}");
                warningPopup.SetActive(true);   
                warningText.text = $"{GetErrorMessage(task.Exception)}";
                return;
            }
            Debug.Log("Password reset email sent successfully.");
            warningPopup.SetActive(true);
            warningText.text = $"비밀번호 리셋 이메일이 발송됐습니다.";
            Close("FindPWPanel");
            // findPWPanel.SetActive(false);
        });

    }
    
    /// <summary>
    /// 로그아웃합니다.
    /// </summary>
    public void SignOut()
    {
        BackendManager.Auth.SignOut();
    }

    public void CheckUserInfo()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null)
            return;
        Debug.Log($"UID(UID) : {user.UserId}");
        Debug.Log($"닉네임(DisplayName) : {user.DisplayName}");
        Debug.Log($"익명로긴 여부(IsAnonymous) : {user.IsAnonymous}");
        Debug.Log($"이메일(Email) : {user.Email}");
        Debug.Log($"이메일 인증여부(IsEmailVerified) : {user.IsEmailVerified}");
    }


    public void Open(string name)
    {
        Debug.Log($"{name} 패널을 엽니다");
        GetUI(name).SetActive(true);
    }
    public void Close(string name)
    {
        Debug.Log($"{name} 패널을 닫습니다");
        GetUI(name).SetActive(false);
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
                message = "너무 취약한 비밀번호입니다.";
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
