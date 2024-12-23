using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_Manager : BaseUI
{
    [SerializeField] Button stageButton;
    // User Data
    [SerializeField] protected string UserID;
    [SerializeField] protected string Nickname;
    [SerializeField] protected string Email;
    [SerializeField] protected bool IsVerified; // Email인증 된 유저인가/아닌가
    private bool _isLoggedIn;


    private TMP_InputField _emailLoginInputField;
    private TMP_InputField _emailLoginPWInputField;





    private void Start()
    {
        Init();

        // 아무래도 데이터들 다 불러와야되니까 시작을 하게된다면 로딩씬을 플레이하고 난뒤에??




    }

    private void Init()
    {
        // 로긴패널
        SetLoginPanel();

        // 로비패널
        SetLobbyPanel();

        // 스테이지패널
        SetStagePanel();

        // 이메일 로긴 팝업
        SetEmailLoginPanel();

        // 프로필 팝업
        SetProfilePopUp();



        //이메일계정연동:
        SetLinkEmailPanel();




    }


    
    private void SetLoginPanel()
    {
        // 로긴패널
        GetUI("LoginPanel");
        GetUI<Button>("LobbyButton").onClick.AddListener(() => Open("LobbyPanel"));
        
        // 게스트로그인 버튼
        // GetUI<Button>("GuestLoginButton").onClick.AddListener(() => Open("ProfilePopUp"));
        GetUI<Button>("GuestLoginButton").onClick.AddListener(CheckUserData);


        // 프로필이름텍스트
           //    GetUI<TMP_Text>("ProfileNameText").text = Nickname;
           //    if (Nickname != null)
           //    {
           //        GetUI<TMP_Text>("ProfileNameText").text = "닉네임을 생성하세요";
           //    }
        GetUI<TMP_Text>("UserIDText").text = "프로필아이디";
        // 닉네임 정도는 설정할 수 있는게 좋지 않으려나
        GetUI<TMP_Text>("StatusText").text = "게스트 로그인 입니다.";
        // 정보저장을위해 이메일 로그인 유도

        GetUI("NickNamePopUp");
        // _nickName =  GetUI<TMP_InputField>("NickNameInputField").text;
        // GetUI<Button>("NickBackButton").onClick.AddListener(() => GoBack("NickNamePopUp"));
        // GetUI<Button>("NickConfirmButton").onClick.AddListener(ConfrimNickName);


        // 이메일로그인 Button
        GetUI<Button>("EmailLoginButton").onClick.AddListener(() => Open("EmailLoginPopup"));

        // Email가입 버튼
        GetUI<Button>("SignUpButton").onClick.AddListener(() => Open("SignUpPanel"));

        // Email 인증 버튼
        GetUI<Button>("EmailAuthButton").onClick.AddListener(() => Open("EmailAuthPopup"));
        
        
        // 인증메일 팝업창
        GetUI("EmailAuthPopup");
        // TODO:
        // 프로필 창을 만들어서
        // 닉네임(++변경버튼?), UID, 
        // 이메일인증 & 이메일가입 버튼 추가해서 계정연동(인증)해서 할 수 있도록하기
        // 프리펩화?



    }
    private void SetProfilePopUp()
    {
        // 프로필팝업
        GetUI<Button>("ProfileButton").onClick.AddListener(() => Open("ProfilePopUp"));
        GetUI<Button>("ProfileBackButton").onClick.AddListener(() => GoBack("ProfilePopUp"));
        GetUI<Button>("ProfileButton").onClick.AddListener(CheckUserInfo);
    }
    private void SetLinkEmailPanel()
    {
        GetUI<Button>("AddEmailButton").onClick.AddListener(() => Open("LinkEmailPopup"));
        GetUI<Button>("LinkEmailConfirmButton").onClick.AddListener(AddEmailAccount);
    }

    private void SetEmailLoginPanel()
    {
        GetUI("EmailLoginPopup");
        _emailLoginInputField = GetUI<TMP_InputField>("EmailLoginInputField");
        _emailLoginPWInputField = GetUI<TMP_InputField>("EmailLoginPWInputField");
        GetUI<Button>("EmailLoginConfirmButton").onClick.AddListener(EmailLogin); 
    }
    private void SetLobbyPanel()
    {
        GetUI("LobbyPanel");
        GetUI<TMP_Text>("TestText").text = "Tears To Dragon 티어즈 투 드래곤";
        // 스테이지버튼
        stageButton = GetUI<Button>("StageButton");
        stageButton.onClick.AddListener(() => Open("StagePanel"));
        // 팝업버튼 // 변수선언안해도 잘되는구만
        GetUI<Button>("PopUpButton").onClick.AddListener(() => Open("PopUpPanel"));
        GetUI<TMP_Text>("PopUpButtonText").text = "팝업";
        // 팝업패널
        GetUI("PopUpPanel").SetActive(false);
        GetUI<TMP_Text>("PopUpText").text = "팝업창 짜잔";
        // Stage패널
        GetUI("StagePanel").SetActive(false);
        // 뒤로가기 버튼
        GetUI<Button>("BackButton").onClick.AddListener(() => GoBack("PopUpPanel"));
    }
    private void SetStagePanel()
    {
        GetUI<Button>("StageBackButton").onClick.AddListener(() => GoBack("StagePanel"));
    }


    /// <summary>
    /// 패널이름 넣기
    /// gameObject로 되있어야함.
    /// ex)  GetUI("PopUpPanel");
    /// </summary>
    /// <param name="name"></param>
    public void Open(string name)
    {
        Debug.Log($"{name} 패널을 엽니다");
        GetUI(name).SetActive(true);
    }

    public void GoBack(string name)
    {
        Debug.Log($"{name} 패널을 닫습니다");
        GetUI(name).SetActive(false);
        /*뭔가하려다 방식변경
        // 부모오브젝트 가져와서 .SetActive(false)
        // 바로 상위 오브젝트만 꺼버리고 싶은데 최상위가 꺼짐.
        // 아 왜 이게 계속 더 밑에 자식이 안되나 했더니 이거 스크립트가 팝업패널에 있는게 아니라 그러네 
        // 그러면 닫고싶은 패널(from. 패널?? ______패널에서 뒤로가기)같은 형식으로??
        // 근데 어차피 Stack으로 해서 POP으로 하게될거같은디 무튼...
        Transform transform = this.gameObject.transform.parent;
        Debug.Log($"부모오브젝트 이름: {transform.name}");
        Transform transform2 = transform.GetChild(transform.childCount);
        Debug.Log($"{transform2.name}");

      //  Transform parent = transform;
      //  transform.SetParent(parent);
      //  Debug.Log($"{gameObject.name}");
      //  parent.GetChild(parent.childCount-1).gameObject.SetActive(false); // gameObject.SetActive(false);
      //  Debug.Log($"{gameObject.name}");
      //  Debug.Log($"뒤로가기");
        */
    }
    public void LinkLoginAuth()
    {

    }

    public void EmailSignUp()
    {

    }

    public void EmailLogin()
    {
        Debug.Log("LoginButton 테스트 로그");
        
        
        string email = _emailLoginInputField.text;
        if (email == "")
        {
            Debug.Log("이메일을 입력 해 주세요.");
            // GetUI<TMP_Text>("NotificationText").text = "이메일을 입력 해 주세요.";
            return;

        }
        string password = _emailLoginPWInputField.text;
        if (password == "")
        {
            Debug.Log("비밀번호를 입력 하세요.");
            // GetUI<TMP_Text>("NotificationText").text = "비밀번호를 입력 하세요.";
            return;
        }
        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
           .ContinueWithOnMainThread(task =>
           {
               if (task.IsCanceled)
               {
                   Debug.LogWarning("SignInWithEmailAndPasswordAsync was canceled.");
                   // GetUI<TMP_Text>("NotificationText").text = "로그인 인증이 취소되었습니다. ";
                   // OpenNotifiaction();
                   return;
               }
               if (task.IsFaulted)
               {
                   Debug.LogWarning("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                   // GetUI<TMP_Text>("NotificationText").text = $"올바른 아이디/비밀번호를\n 입력해주세요.";
                   // OpenNotifiaction();
                   return;
               }

               AuthResult result = task.Result;
               Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
               // CheckUserInfo();
           });
        GoBack("EmailLoginPopup");

    }

    /// <summary>
    /// 게스트로그인 :
    /// 가입없이 바로 로그인하게 해줌
    /// 
    /// </summary>
    public void GuestLogin()
    {
        var _userdataRef = BackendManager.UserDataRef;
        
        BackendManager.Auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task => 
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result; // 계정생성

            Debug.LogFormat("성공적으로 로그인했습니다 : {0} ({1})",result.User.DisplayName, result.User.UserId);
               Nickname = result.User.DisplayName;
               UserID = result.User.UserId;
            // UID Firebase저장하기
               SaveUserData();
            // Debug.Log($"UserName = {result.User.DisplayName} \n유저ID: {result.User.UserId}");
        });
        //Debug.Log($"UserName 닉네임 : {Nickname}");
        //Debug.Log($"UserID 아이디 : {UserID}");
        

        if (Nickname == "") 
        {
            Debug.Log("닉네임 없음");
            Open("NickNamePopUp");
        }
        // SaveUserData();
    }

    /// <summary>
    /// UID존재하나 확인
    /// </summary>
    // TODO :
    // 작동원리 / 구조 / 돌아가는거 파악제대로 해야됨
    // 지금 안되고있었는데 닉네임설정창에서 설정(저장)하면서 하니까 됨
    // 저장위치 똑같이 한거 같은데 상위 위치에서 저장되고 뭔가 뭔가임...
    public void CheckUserData()
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
            GuestLogin();

        });
    }


    public void SaveUserData()
    {
        DatabaseReference userRef = BackendManager.Database.GetReference( $"Users/UserID/{UserID}"); //( $"Users/UserID/{UserID}")
        Debug.Log($"유저레프 체크 : {userRef}");
        Dictionary<string, object> userData = new Dictionary<string, object>()
        {
            {"UID", UserID},
            {"Nickname", Nickname },
            // {"Email", Email },
            // {"IsVerified", IsVerified },
            {"Last Login", DateTime.UtcNow.ToString("O")}
        };
        userRef.SetValueAsync( userData ).ContinueWithOnMainThread(task =>
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
        Debug.Log($"[DB]유저ID: {UserID}");
        Debug.Log($"[DB]닉네임: {Nickname}");
        Debug.Log($"[DB]마지막로그인: {DateTime.UtcNow.ToString("O")}");
    }

    /// <summary>
    /// 이메일 계정 추가 / 이메일(+비번)연동?
    /// </summary>
    private void AddEmailAccount()
    {
        // 이메일 계정추가 (일단 지금은 있는 계정을 연동 & 로그인시키니까)
        // 게스트상태에서 작동한다면
        // 이메일 생성을 먼저하고 이걸 그 뒤에 추가해서 작동하도록
        // 그렇게하려면 밑에 인풋필드부분 다 삭제하고 이메일 가입하는거에다가 이거 더해놓으면 될듯.
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        string email = GetUI<TMP_InputField>("LinkEmailInputField").text;
        if (email == "")
        {
            Debug.Log("이메일을 입력 해 주세요.");
            // GetUI<TMP_Text>("NotificationText").text = "이메일을 입력 해 주세요.";
            return;

        }
        string password = GetUI<TMP_InputField>("LinkEmailPWInputField").text;
        if (password == "")
        {
            Debug.Log("비밀번호를 입력 하세요.");
            // GetUI<TMP_Text>("NotificationText").text = "비밀번호를 입력 하세요.";
            return;
        }
        Debug.Log($"[연동하기전] 유저ID: {UserID}");
        Debug.Log($"[연동하기전] 닉네임: {Nickname}");
        Firebase.Auth.Credential credential = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
      //  user.LinkWithCredentialAsync(credential).ContinueWith(task => {
      //      if (task.IsCanceled)
      //      {
      //          Debug.LogError("LinkWithCredentialAsync was canceled.");
      //          return;
      //      }
      //      if (task.IsFaulted)
      //      {
      //          Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
      //          return;
      //      }
      //
      //      Firebase.Auth.AuthResult result = task.Result;
      //      Debug.LogFormat("사용자 인증정보 파이어베이스에연동됨: {0} ({1})",
      //          result.User.DisplayName, result.User.UserId);
      //  });
        // LinkWithCredentialAsync()
        // 위에 이거는 보니까 '인증정보가 이미 다른사용자 계정에 연결'되있으면 실패한다고함
        // 위에꺼만  되나 테스트해봐야함
        BackendManager.Auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }
     
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat($" 연동후 유저정보 :{result.User.DisplayName}, {result.User.UserId}");
        });
        
    }



    public void AutomaticStart()
    {
        if (_isLoggedIn)
            Open("LobbyPanel");
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

    public void SignOut()
    {
        BackendManager.Auth.SignOut();
    }

}
