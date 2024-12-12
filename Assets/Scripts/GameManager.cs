using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    /*
     모든 장면에서 사용되는 클래스만 등록할것
     */

    public static SoundManager Sound => SoundManager.Instance;
    public static FirebaseAuth Auth => BackendManager.Auth;
    public static FirebaseDatabase Database => BackendManager.Database;

    // GameManager 싱글톤 프리팹 생성
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        Instantiate(Resources.Load<GameManager>("GameManager"));
    }

    private void Awake()
    {
        RegisterSingleton(this);
        DontDestroyOnLoad(this);
    }
}
