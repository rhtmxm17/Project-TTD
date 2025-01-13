using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance = null;

    public float PartyCost { get; private set; } = 0;
    public bool IsInChangeWave { get; private set; } = true;
    public DamageDisplayer DamageDisplayer { get; private set; }

    [SerializeField] protected StageData stageData;
    public StageData stageDataOnLoad { get; protected set; }
    public MenuType PrevScene { get; protected set; } = MenuType.NONE;

    protected Dictionary<int, CharacterData> batchDictionary;

    [Header("Camera Effecter")]
    [SerializeField]
    CombatCamera combatCamera;

    [Header("Characters")]
    [SerializeField]
    CombManager characterManager;
    [SerializeField]
    StageCharacterSetter characterSetter;

    [Header("Scroller")]
    [SerializeField]
    BGScroller scroller;

    [Header("Monsters")]
    [SerializeField] Transform monsterWaveParent;
    [SerializeField] MonsterWaveSetter monsterWavePrefab;
    [SerializeField]
    List<CombManager> monsterWaveQueue = new List<CombManager>();
    [SerializeField]
    float monsterWaveOffset;

    [Header("Party Gauge")]
    [SerializeField]
    float MaxPartyCost;

    //TODO : 이것도 스테이지별로 파라미터 지정하기.
    [Header("Timer")]
    [SerializeField]
    protected int timeLimit;
    [SerializeField]
    protected TextMeshProUGUI leftTimeText;

    protected bool isTimeOver;

    [Header("Result Popup Window")]
    [SerializeField]
    protected AdvencedPopupInCombatResult resultPopupWindow;

    [Header("Debug")]
    [Space(20)]
    [SerializeField]
    TextMeshProUGUI costText;
    [SerializeField]
    Slider costSlider;
    [SerializeField]
    TextMeshProUGUI wavesText;

    protected int curWave;
    protected int maxWave;
    string MAX_WAVE;

    private bool isCombatEnd;
    protected StageSceneChangeArgs prevSceneData;

    public bool IsCombatEnd {
        get { return isCombatEnd; }
        protected set {
            isCombatEnd = value; 
            Time.timeScale = 1;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DamageDisplayer = GetComponent<DamageDisplayer>();
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {

        if (stageDataOnLoad == null)
            InitializeStageData(stageData);
    }

    public virtual void Initialize(StageSceneChangeArgs sceneChangeArgs)
    {
        prevSceneData = sceneChangeArgs;
        PrevScene = sceneChangeArgs.prevScene;
        InitializeStageData(sceneChangeArgs.stageData);
    }

    public void InitializeStageData(StageData _stageData)
    {
        if (null == _stageData)
            return;

        if (null != stageDataOnLoad)
        {
            Debug.Log("스테이지 초기화가 두 번 진행됨");
        }

        stageDataOnLoad = _stageData;
        stageData = _stageData;

        monsterWaveQueue.Clear();
        for (int i = monsterWaveParent.childCount - 1; i >= 0; i--)
        {
            Destroy(monsterWaveParent.GetChild(i).gameObject);
        }

        scroller.SetScroller(_stageData.BackgroundTypePrefab);


        // ============= 플레이어 캐릭터 초기화 =============

        //키 : 배치정보, 값 : 캐릭터 고유 번호(ID)
        Dictionary<string, long> batchData = GameManager.UserData.PlayData.BatchInfo.Value;
        batchDictionary = new Dictionary<int, CharacterData>(batchData.Count);

        foreach (var pair in batchData)
        {
            batchDictionary[int.Parse(pair.Key)] =
                GameManager.TableData.GetCharacterData((int)pair.Value);
        }

        characterSetter.InitCharacters(batchDictionary, _stageData.TileBuff);
        characterManager.ListClearedEvent.AddListener(OnDefeat);

        // ============= 몬스터 캐릭터 초기화 =============

        foreach (StageData.WaveInfo wave in _stageData.Waves)
        {
            MonsterWaveSetter monsterWave = Instantiate(monsterWavePrefab, monsterWaveParent);
            monsterWave.InitCharacters(wave);
            monsterWave.gameObject.SetActive(false);
            monsterWave.GetComponent<CombManager>().ListClearedEvent.AddListener(() =>
            {
                StartCoroutine(GoNextWaveCO());
            });
            monsterWaveQueue.Add(monsterWave.GetComponent<CombManager>());
            monsterWave.transform.position += new Vector3(monsterWaveOffset, 0, 0);
        }

        maxWave = monsterWaveQueue.Count;
        MAX_WAVE = maxWave.ToString();
        curWave = 0;

        AddWaveText();

    }

    public void StartGameOnSceneLoaded() => StartGame();

    IEnumerator StartPartyCostCO()
    {
        while (true)
        {
            PartyCost = Math.Clamp(PartyCost + Time.deltaTime / 3, 0, MaxPartyCost);
            costText.text = PartyCost.ToString();
            costSlider.value = PartyCost / MaxPartyCost;
            yield return null;
        }
    }

    public void AddPartyCost(float amount)
    {
        PartyCost = Math.Clamp(PartyCost + amount, 0, MaxPartyCost);
        costSlider.value = PartyCost / MaxPartyCost;
        costText.text = PartyCost.ToString();
    }
    public void UsePartyCost(float cost)
    {
        if (PartyCost < cost)
        {
            Debug.Log("코스트 부족");
            return;
        }

        PartyCost -= cost;
        costSlider.value = PartyCost / MaxPartyCost;
        costText.text = PartyCost.ToString();
    }

    void AddWaveText()
    {
        if (curWave >= maxWave)
            return;

        curWave++;
        wavesText.text = $"WAVE\n{curWave.ToString()} / {MAX_WAVE}";
    }

    IEnumerator GoNextWaveCO()
    {
        monsterWaveQueue.RemoveAt(0);

        if (monsterWaveQueue.Count <= 0)
        {
            OnClear();
            yield break;
        }

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (CheckCharactersWait())
                break;

            Debug.Log("캐릭터 복귀중...");

        }

        IsInChangeWave = true;

        AddWaveText();
        Debug.Log("다음 웨이브로 이동중...");

        scroller.StartScroll();
        yield return StartCoroutine(combatCamera.StartFocusCO());//줌인

        StartCoroutine(combatCamera.ReleaseFocusCO());//줌아웃과 동시에 다음 웨이브 활성화
        monsterWaveQueue[0].gameObject.SetActive(true);
        yield return StartCoroutine(WaitMonsterWaveCO(monsterWaveQueue[0].transform));//몬스터 웨이브 이동시간

        scroller.StopScroll();


        characterManager.StartCombat(monsterWaveQueue[0]);
        monsterWaveQueue[0].StartCombat(characterManager);

        IsInChangeWave = false;
    }

    IEnumerator WaitMonsterWaveCO(Transform monsterWave)
    {
        yield return null;

        float curMonved = 0;

        while (curMonved < monsterWaveOffset)
        {
            float delta = 8/*웨이브 접근 속도*/ * Time.deltaTime;
            curMonved += delta;
            monsterWave.transform.position += new Vector3(-delta, 0, 0);
            yield return null;
        }

    }

    protected virtual void OnClear()
    {
        if (IsCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        IsCombatEnd = true;

        Debug.Log("클리어!");

        // 보상 획득 후 종료
        bool isFirst = (stageDataOnLoad.ClearCount.Value == 0);

        // 클리어 횟수 증가 및 첫클리어시 보상 획득
        stageDataOnLoad.UserGetRewardOnceAsync(result =>
        {
            if (false == result)
            {
                Debug.Log("요청 전송에 실패했습니다");
                return;
            }

            // 첫 클리어라면 보상 목록에 반영, 아니라면 비어있음
            List<ItemGain> gainList = null;
            if (isFirst)
            {
                gainList = stageDataOnLoad.Reward;
            }

            // 아이템 획득 팝업 + 확인 클릭시 메인 화면으로
            List<CharacterData> chDataL = new List<CharacterData>(batchDictionary.Values);
            int randIdx = UnityEngine.Random.Range(0, chDataL.Count);

                resultPopupWindow.OpenDoubleButtonWithResult(
                    stageDataOnLoad.StageName,
                    gainList,
                    "확인", LoadPreviousScene,
                    "다음 스테이지로", () => {

                        if (stageDataOnLoad.NextStageId == -1)
                        {
                            GameManager.OverlayUIManager.OpenSimpleInfoPopup("해당 챕터의 마지막 스테이지입니다!", "닫기", null);
                        }
                        else
                        {
                            StageData nextStageData = DataTableManager.Instance.GetStageData(stageDataOnLoad.NextStageId);

                            if (nextStageData == null)
                            {
                                Debug.Log("전달된 다음 인덱스 스테이지 정보가 존재하지 않음.");
                                return;
                            }

                            GameManager.OverlayUIManager.OpenDoubleInfoPopup($"다음 스테이지로 가시겠습니까? \n {nextStageData.StageName}", "그만두기", "도전하기", null,
                                () => {
                                    prevSceneData.stageData = nextStageData;
                                    //TODO : 용도에 따라서 지우거나 이용
                                    //prevSceneData.prevScene = prevSceneData.prevScene;
                                    GameManager.Instance.LoadBattleFormationScene(prevSceneData);
                                });
                        }

                    },
                    true, false,
                    "승리!", chDataL[randIdx].FaceIconSprite,
                    AdvencedPopupInCombatResult.ColorType.VICTORY
                );

        });
    }

    protected virtual void OnDefeat()
    {
        if (IsCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        IsCombatEnd = true;

        //패배
        List<CharacterData> chDataL = new List<CharacterData>(batchDictionary.Values);
        int randIdx = UnityEngine.Random.Range(0, chDataL.Count);

        resultPopupWindow.OpenDoubleButtonWithResult(
            stageDataOnLoad.StageName,
            null,
            "확인", LoadPreviousScene,
            "재도전", () => {

                GameManager.OverlayUIManager.OpenDoubleInfoPopup("재도전하시겠습니까?", "아니요", "네",
                    null, () => {
                        //TODO : 용도에 따라서 지우거나 이용
                        //prevSceneData.stageData = prevSceneData.stageData;
                        //prevSceneData.prevScene = prevSceneData.prevScene;
                        GameManager.Instance.LoadBattleFormationScene(prevSceneData);
                    });

            },
            true, false,
            "패배..", chDataL[randIdx].FaceIconSprite,
            AdvencedPopupInCombatResult.ColorType.DEFEAT
        );
    }

    protected void LoadPreviousScene() => GameManager.Instance.LoadMenuScene(PrevScene);

    bool CheckCharactersWait()
    {
        foreach (CharacterCombatable chara in characterManager.CharList)
        {
            if (!chara.IsWaiting())
                return false;
        }

        return true;
    }

    protected virtual void StartGame()
    {
        if (monsterWaveQueue.Count <= 0)
        {
            Debug.LogWarning("몬스터 정보가 비어있음");
            return;
        }

        IsCombatEnd = false;

        //파티 공유 게이지 충전 시작.
        StartCoroutine(StartPartyCostCO());

        costSlider.value = 0;
        costText.text = PartyCost.ToString();

        StartCoroutine(FirstWaveSetCO());

        leftTimeText.text = "웨이브 대기중...";

    }

    protected virtual IEnumerator FirstWaveSetCO()
    {
        scroller.StartScroll();
        yield return new WaitForSeconds(3f);//순수이동시간
        monsterWaveQueue[0].gameObject.SetActive(true);
        yield return StartCoroutine(WaitMonsterWaveCO(monsterWaveQueue[0].transform));//몬스터 웨이브 이동시간
        scroller.StopScroll();

        characterManager.StartCombat(monsterWaveQueue[0]);
        monsterWaveQueue[0].StartCombat(characterManager);

        IsInChangeWave = false;

        StartCoroutine(StartTimerCO());
    }

    protected virtual IEnumerator StartTimerCO()
    {
        TimeSpan leftTimeSpan = TimeSpan.FromSeconds(timeLimit);
        leftTimeText.text = $"{leftTimeSpan.Minutes:D2} : {leftTimeSpan.Seconds:D2}";

        while (timeLimit > 0)
        {
            yield return new WaitForSeconds(1);
            timeLimit -= 1;
            leftTimeSpan = TimeSpan.FromSeconds(timeLimit);
            leftTimeText.text = $"{leftTimeSpan.Minutes:D2} : {leftTimeSpan.Seconds:D2}";
        }

        Debug.Log("타임 오버!");
        OnDefeat();

    }

    [ContextMenu("StartGame")]
    public void S()
    {
        StartGame();
    }

    [ContextMenu("FinishGame")]
    public void F()
    {
        Debug.Log("F눌림");
        characterManager.EndCombat();
        monsterWaveQueue[0].EndCombat();
        monsterWaveQueue.RemoveAt(0);
    }

}
