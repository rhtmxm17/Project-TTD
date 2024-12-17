using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_Manager : BaseUI
{
    [SerializeField] Button stageButton;
    [SerializeField] string _nickName;
    // TODO :
    // 닉네임 없으면 설정하게 해주기
    // 준비물 : 
    // 닉네임 있나 없나 확인, 없으면 만드는 팝업창 띄우기
    // TMP_Text(닉넹미 입력 메세지) & TMP_InputField(닉네임입력)
    // _nickName = TMP_InputField
    // ID && Nick 있으면, 그냥 메인패널
    [SerializeField] string _userID;
    private bool _isLoggedIn;

    
    Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        // 로긴패널
        GetUI("LoginPanel");
        GetUI<Button>("LobbyButton").onClick.AddListener(() => Open("LobbyPanel"));
        GetUI<Button>("ProfileButton").onClick.AddListener(() => Open("ProfilePopUp"));
        // 게스트로그인 버튼
        // GetUI<Button>("GuestLoginButton").onClick.AddListener(() => Open("ProfilePopUp"));
        GetUI<Button>("GuestLoginButton").onClick.AddListener(GuestLogin);

        // 프로필팝업
        GetUI<Button>("ProfileBackButton").onClick.AddListener(() => GoBack("ProfilePopUp"));
        // 프로필이름텍스트
        GetUI<TMP_Text>("ProfileNameText").text = _nickName;
        if (_nickName != null)
        {
            GetUI<TMP_Text>("ProfileNameText").text = "닉네임을 생성하세요";
        }
        GetUI<TMP_Text>("UserIDText").text = "프로필아이디";
        // 닉네임 정도는 설정할 수 있는게 좋지 않으려나
        GetUI<TMP_Text>("StatusText").text = "게스트 로그인 입니다.";
        // 정보저장을위해 이메일 로그인 유도

        GetUI("NickNamePopUp");
        // _nickName =  GetUI<TMP_InputField>("NickNameInputField").text;
        GetUI<Button>("GuestLoginButton").onClick.AddListener(ConfrimNickName);
        GetUI<Button>("NickBackButton").onClick.AddListener(() => GoBack("NickNamePopUp"));



        // 로비패널
        GetUI("LobbyPanel");
        GetUI<TMP_Text>("TestText").text = "Tear To Dragon 티어 투 드래곤";
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


        // 스테이지패널
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

    public void GuestLogin()
    {
        
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
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

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            _nickName = result.User.DisplayName;
            _userID = result.User.UserId;

            Debug.Log($"UserName = {result.User.DisplayName} \n유저ID: {result.User.UserId}");
        });


        Debug.Log($"UserName 닉네임 : {_nickName}");
        Debug.Log($"UserID 아이디 : {_userID}");

        if (_nickName == "")
        {
            Open("NickNamePopUp");
        }
    }
    
    private void ConfrimNickName()
    {
        _nickName = GetUI<TMP_InputField>("NickNameInputField").text;
    }
}
