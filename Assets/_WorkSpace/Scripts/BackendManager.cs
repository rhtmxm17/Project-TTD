using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;

public class BackendManager : SingletonBehaviour<BackendManager>
{
    private static FirebaseApp app;
    private static FirebaseAuth auth;
    private static FirebaseDatabase database;

    private static DatabaseReference usersDataRef;
    private static DatabaseReference currentUserDataRef;

    public static FirebaseAuth Auth => auth;
    public static FirebaseDatabase Database => database;

    public static DatabaseReference UserDataRef => usersDataRef;
    public static DatabaseReference CurrentUserDataRef => currentUserDataRef;

    private void Awake()
    {
        RegisterSingleton(this);
        CheckFirebaseDependencies();
    }

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                usersDataRef = database.RootReference.Child($"Users");
                auth.IdTokenChanged += Auth_IdTokenChanged;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
                app = null;
                auth = null;
                database = null;

                usersDataRef = null;
                currentUserDataRef = null;
            }
        });
    }

    private void Auth_IdTokenChanged(object sender, System.EventArgs args)
    {
        if (auth.CurrentUser == null)
        {
            currentUserDataRef = null;
        }
        else
        {
            currentUserDataRef = database.RootReference.Child($"Users/{auth.CurrentUser.UserId}");
        }
    }

    /// <summary>
    /// 개발용 메서드, 인증이 적용되지 않은 가상의 유저 데이터를 사용합니다
    /// DB에 규칙이 추가되면 사용이 불가능해집니다
    /// </summary>
    /// <param name="number">더미 UID 뒤에 붙일 번호</param>
    public void UseDummyUserDataRef(int number)
    {
        currentUserDataRef = database.RootReference.Child($"Users/Dummy{number}");
    }
}
