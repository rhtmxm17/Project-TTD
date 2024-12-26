using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    /*
     모든 장면에서 사용되는 클래스만 등록할것
     */
    
    public static SoundManager Sound => SoundManager.Instance;
    public static Firebase.Auth.FirebaseAuth Auth => BackendManager.Auth;
    public static Firebase.Database.FirebaseDatabase Database => BackendManager.Database;
    public static PlayerInput Input { get; private set; } = null;
    public static DataTableManager TableData => DataTableManager.Instance;
    public static UserDataManager UserData => UserDataManager.Instance;

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
        Input = GetComponent<PlayerInput>();
    }

    #region LoadScene

    /// <summary>
    /// 스테이지 로드 시점에서만 사용, 불러올 스테이지 데이터
    /// </summary>
    private StageData stageData;

    public void LoadMainScene()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void LoadStageScene(StageData StageData)
    {
        this.stageData = StageData;
        SceneManager.sceneLoaded += OnStageSceneLoaded;
        SceneManager.LoadSceneAsync("SecondSkillSystem_kmt");
    }

    /// <summary>
    /// 스테이지 씬 로드 완료시 스테이지 관리자 클래스 초기화(선택된 스테이지 데이터 전달)
    /// </summary>
    private void OnStageSceneLoaded(Scene unused1, LoadSceneMode unused2)
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out StageManager stageManager);

        // TODO: 스테이지 데이터로 초기화
        // stageManager.Initialize(stageData);

        SceneManager.sceneLoaded -= OnStageSceneLoaded;
    }

    #endregion LoadScene
}
