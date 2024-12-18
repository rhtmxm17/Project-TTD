using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknamePanel : UI_Manager
{
    

    void Start()
    {
        Init();
    }
    private void Init()
    {
        GetUI("NickNamePopUp");
        // _nickName =  GetUI<TMP_InputField>("NickNameInputField").text;
        GetUI<Button>("NickBackButton").onClick.AddListener(() => GoBack("NickNamePopUp"));
        GetUI<Button>("NickConfirmButton").onClick.AddListener(ConfrimNickName);
    }

    private void ConfrimNickName() // 게스트로그인 현상태에선 안됨
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        Nickname = GetUI<TMP_InputField>("NickNameInputField").text;
        if (Nickname == "")
        {
            Debug.LogWarning("닉네임을 설정해주세요.");
            return;
        }
   
        UserProfile profile = new UserProfile();
        profile.DisplayName = Nickname;
   
        BackendManager.Auth.CurrentUser.UpdateUserProfileAsync(profile)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                  //여기서 에러남 그냥 주석 처리해뒀는데 에러남 
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }
          
                Debug.Log("User profile updated successfully.");
          
          
                // 일단 그냥 체크용 로그 
                Debug.Log($"Display Name :\t {user.DisplayName}");
                Debug.Log($"Email :\t\t {user.Email} 없으니까 안나오겠지. 추후 이메일인증후");
                Debug.Log($"Email Verified:\t  {user.IsEmailVerified} 이하동문 ");
                Debug.Log($"User ID: \t\t  {user.UserId}");
          
          
                gameObject.SetActive(false);
            });
        SaveUserData();
   
        GoBack("NickNamePopUp");
   
        Debug.Log($"UserName 닉네임 : {Nickname}");
        Debug.Log($"UserID 아이디 : {user.UserId}");
   
    }
   
}
