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
                    GetErrorMessage(task.Exception);
                    Debug.LogError($"실패하였습니다, 사유: {task.Exception}");
                    warningPopup.SetActive(true);   
                    warningText.text = $"{GetErrorMessage(task.Exception)}";
                    
                    
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

            
        }
        return message;
    }
    #endregion

}
