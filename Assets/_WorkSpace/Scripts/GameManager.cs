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

    public static RectTransform PopupCanvas => Instance.popupCanvas;

    [SerializeField] RectTransform popupCanvas;

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

    #region 짧은 로딩
    [SerializeField] RectTransform shortLoadingPanel;
    private int loadingCounter = 0;

    public void StartShortLoadingUI()
    {
        if (loadingCounter == 0)
            shortLoadingPanel.gameObject.SetActive(true);

        loadingCounter++;
    }

    public void StopShortLoadingUI()
    {
        loadingCounter--;

        if (loadingCounter == 0)
            shortLoadingPanel.gameObject.SetActive(false);

#if DEBUG
        if (loadingCounter < 0)
            Debug.LogError("시작되지 않은 로딩이 완료됨");
#endif
    }
    #endregion

    #region LoadScene

    /// <summary>
    /// 스테이지 로드 시점에서만 사용, 불러올 스테이지 데이터
    /// </summary>
    private StageData stageData;
    
    public enum StageType { NORMAL, GOLD, BOSS, NONE}
    public StageType stageType { get; private set; } = StageType.NONE;

    public void LoadMainScene()
    {
        Debug.Log($"씬 전환: {SceneManager.GetActiveScene().name} -> MainMenu");
        SceneManager.LoadSceneAsync("MainMenu");
    }

    /// <summary>
    /// 전투 스테이지 데이터와 전투씬 타입을 지정
    /// </summary>
    /// <param name="StageData">스테이지 데이터</param>
    /// <param name="stageType">전투씬 타입</param>
    public void SetLoadStageType(StageData StageData, StageType stageType)
    {
        this.stageType = stageType;
        Debug.Log($"씬 전환: {SceneManager.GetActiveScene().name} -> StageDefault(스테이지명:{StageData.StageName})");
        this.stageData = StageData;
    }

    /// <summary>
    /// 지정한 스테이지 타입과 스테이지 데이터를 기반으로
    /// 전투씬 로딩 [ SetLoadStageType으로 설정. ]
    /// </summary>
    public void LoadStageScene()
    {
        SwitchStageType();
    }

    void SwitchStageType()
    {
        switch (stageType)
        { 
            case StageType.NORMAL:
                SceneManager.sceneLoaded += OnStageSceneLoaded;
                SceneManager.LoadSceneAsync("StageDefault");
                break;
            case StageType.GOLD:
                SceneManager.sceneLoaded += OnStageSceneLoaded;
                SceneManager.LoadSceneAsync("Stage_Coin");
                break;
            case StageType.BOSS:
                SceneManager.sceneLoaded += OnStageSceneLoaded;
                SceneManager.LoadSceneAsync("Stage_Boss");
                break;
            case StageType.NONE:
                Debug.Log("지정된 스테이지 타입이 없음");
                break;
            default:
                Debug.Log("예외상황");
                break;
        }
    }

    /// <summary>
    /// 스테이지 씬 로드 완료시 스테이지 관리자 클래스 초기화(선택된 스테이지 데이터 전달)
    /// </summary>
    private void OnStageSceneLoaded(Scene unused1, LoadSceneMode unused2)
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out StageManager stageManager);

        stageManager.Initialize(stageData);
        stageManager.StartGameOnSceneLoaded();
        SceneManager.sceneLoaded -= OnStageSceneLoaded;
    }

    #endregion LoadScene
}
