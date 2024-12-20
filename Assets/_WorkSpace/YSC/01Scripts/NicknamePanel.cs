using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System;
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

    private void MakeNickName() // 일단킵해두고있는데 ConfirmNickName에다가 이식해둠.
    {
        DatabaseReference userRef = BackendManager.Database.GetReference($"Users/UserID/{UserID}"); //( $"Users/UserID/{UserID}")
        Debug.Log($"유저레프 체크 : {userRef}");
        Dictionary<string, object> userData = new Dictionary<string, object>()
        {
            // {"UID", UserID},
            {"Nickname", Nickname },
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


       // Debug.Log($"[DB]유저ID: {UserID}");
        Debug.Log($"[DB]닉네임: {Nickname}");
       // Debug.Log($"[DB]마지막로그인: {DateTime.UtcNow.ToString("O")}");
    
        GoBack("NickNamePopUp");

    }



    private void ConfrimNickName()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        Nickname = GetUI<TMP_InputField>("NickNameInputField").text;
        if (Nickname == "")
        {
            Debug.LogWarning("닉네임을 설정해주세요.");
            return;
        }

        // Firebase RTDatabase
        DatabaseReference userRef = BackendManager.Database.GetReference($"Users/UserID/{UserID}"); 
        Debug.Log($"유저레프 체크 : {userRef}");
        Dictionary<string, object> userData = new Dictionary<string, object>()
        {
            {"UID", UserID},
            {"Nickname", Nickname },
            {"Last Login", DateTime.UtcNow.ToString("O")}
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
        
        // FirebaseAuth
        UserProfile profile = new UserProfile();
        profile.DisplayName = Nickname; // Auth DisplayName/닉네임 저장
      
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
                Debug.Log($"Auth 디스플레이네임 :\t {user.DisplayName}");
                Debug.Log($"Auth 이메일 :\t\t {user.Email} 없으니까 안나오겠지. 추후 이메일인증후");
                Debug.Log($"Auth 이메일인증:\t  {user.IsEmailVerified} 이하동문 ");
                Debug.Log($"Auth UID: \t\t  {user.UserId}");
          
          
                gameObject.SetActive(false);
            });
        // SaveUserDatacheck();
   
   
        Debug.Log($"UserName 닉네임 : {user.DisplayName}");
        Debug.Log($"UserID 아이디 : {user.UserId}");
        Debug.Log($"[DB]닉네임: {userRef.Child("Nickname")}");

        GoBack("NickNamePopUp");
    }
   
}
