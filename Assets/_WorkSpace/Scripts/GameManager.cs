using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum StageType { NORMAL, GOLD, BOSS, STORY, NONE }

/// <summary>
/// 메뉴 목록
/// </summary>
public enum MenuType 
{
    // 주요 메뉴 (빠른 이동 메뉴에서 이동 가능)
    CHARACTERS, ACHIEVEMENT, STORY, SHOP, MYROOM, ADVANTURE, 
    // 기타 메뉴
    PROFILE,
    // 미정의
    NONE,
}

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
    public static OverlayUIManager OverlayUIManager => OverlayUIManager.Instance;

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
    //private int loadingCounter = 0;

    public void StartShortLoadingUI()
    {
        Debug.Log("로딩시작했다");
        shortLoadingPanel.gameObject.SetActive(true);

        //if (loadingCounter == 0)
        //    shortLoadingPanel.gameObject.SetActive(true);

        //loadingCounter++;
    }

    public void StopShortLoadingUI()
    {
        Debug.Log("로딩끝났다");
        shortLoadingPanel.gameObject.SetActive(false);

//        loadingCounter--;

//        if (loadingCounter == 0)
//            shortLoadingPanel.gameObject.SetActive(false);

//#if DEBUG
//        if (loadingCounter < 0)
//            Debug.LogError("시작되지 않은 로딩이 완료됨");
//#endif
    }
    #endregion

    #region LoadScene

    /// <summary>
    /// 스테이지 로드 시점에서만 사용, 불러올 스테이지 데이터
    /// </summary>
    public StageSceneChangeArgs sceneChangeArgs {get; private set;}

    public void LoadMainScene()
    {
        Debug.Log($"씬 전환: {SceneManager.GetActiveScene().name} -> LobbyScene");
        SceneManager.LoadSceneAsync("LobbyScene");
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadSceneAsync("LobbyScene");
    }

    /// <summary>
    /// 지정된 메뉴로 이동
    /// </summary>
    /// <param name="menu"></param>
    public void LoadMenuScene(MenuType menu)
    {
        Debug.Log($"{menu} 메뉴 씬으로 이동 호출됨");

        string sceneName;
        switch (menu)
        {
            case MenuType.ACHIEVEMENT:
                Debug.LogWarning("아직 씬이 준비되지 않음");
                return;
            case MenuType.CHARACTERS:
                sceneName = "CharacterMenuScene";
                break;
            case MenuType.STORY:
                sceneName = "StoryMenuScene";
                break;
            case MenuType.SHOP:
                sceneName = "ShopMenuScene";
                break;
            case MenuType.MYROOM:
                sceneName = "MyRoomScene";
                break;
            case MenuType.ADVANTURE:
                sceneName = "AdvantureMenuScene";
                break;
            case MenuType.PROFILE:
                sceneName = "ProfileScene";
                break;
            case MenuType.NONE:
                Debug.LogWarning("메뉴 타입이 지정되지 않아 로비로 이동함");
                sceneName = "LobbyScene";
                break;
            default:
                Debug.LogWarning($"잘못된 MenuType: {menu}");
                return;
        }
        SceneManager.LoadSceneAsync(sceneName);

    }

    /// <summary>
    /// 스테이지 씬 로딩 데이터를 지정해 편성 씬 로딩
    /// </summary>
    /// <param name="args">스테이지 씬 전환 데이터 목록</param>
    public void LoadBattleFormationScene(StageSceneChangeArgs args)
    {
        this.sceneChangeArgs = args;
        SceneManager.LoadSceneAsync("HYJ_BattleFormation");
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
        string destScene; // 이동할 씬 이름
        switch (sceneChangeArgs.stageType)
        { 
            case StageType.NORMAL:
                destScene = "StageDefault";
                break;
            case StageType.GOLD:
                destScene = "Stage_Coin";
                break;
            case StageType.BOSS:
                destScene = "Stage_Boss";
                break;
            case StageType.STORY:
                destScene = "Stage_Story";
                break;
            case StageType.NONE:
                Debug.Log("지정된 스테이지 타입이 없어 기본 전투 씬으로 이동함");
                destScene = "StageDefault";
                break;
            default:
                Debug.Log($"잘못된 StageType: {sceneChangeArgs.stageType}");
                return;
        }
        SceneManager.sceneLoaded += OnStageSceneLoaded;
        SceneManager.LoadSceneAsync(destScene);
    }

    /// <summary>
    /// 스테이지 씬 로드 완료시 스테이지 관리자 클래스 초기화(선택된 스테이지 데이터 전달)
    /// </summary>
    private void OnStageSceneLoaded(Scene unused1, LoadSceneMode unused2)
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out StageManager stageManager);

        stageManager.Initialize(sceneChangeArgs);
        stageManager.StartGameOnSceneLoaded();
        SceneManager.sceneLoaded -= OnStageSceneLoaded;
    }

    #endregion LoadScene
}

/// <summary>
/// 스테이지 변경 및 초기화에 필요한 데이터 모음<br/>
/// 스테이지에 따라서는 사용되지 않는 데이터도 포함되어있습니다
/// </summary>
public class StageSceneChangeArgs
{
    /// <summary>
    /// 시트 기반 스테이지 데이터
    /// </summary>
    public StageData stageData = null;

    /// <summary>
    /// 스테이지 타입(기본값: NORMAL)
    /// </summary>
    public StageType stageType = StageType.NORMAL;

    /// <summary>
    /// 편성 씬으로 진입 이전 씬(기본값: ADVANTURE)
    /// </summary>
    public MenuType prevScene = MenuType.ADVANTURE;

    /// <summary>
    /// 던전 클리어율 기록을 위한 던전 레벨 인덱스.
    /// </summary>
    public int dungeonLevel = 0;

}