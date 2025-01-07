using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance = null;

    public float PartyCost { get; private set; } = 0;

    [SerializeField] StageData stageData;
    private StageData stageDataOnLoad;

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

    //데이터 테스트용 임시 구조체
/*    [System.Serializable]
    struct monsterData
    {
        [SerializeField] public List<CharacterData> monsters;

    }*/

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

    [Header("Debug")]
    [Space(20)]
    [SerializeField]
    TextMeshProUGUI costText;
    [SerializeField]
    Slider costSlider;
    [SerializeField]
    TextMeshProUGUI wavesText;

    int curWave;
    int maxWave;
    string MAX_WAVE;

    protected bool isCombatEnd;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {

        if (stageDataOnLoad == null)
            Initialize(stageData);
    }

    public void Initialize(StageData _stageData)
    {
        if (null == _stageData)
            return;

        if (null != stageDataOnLoad)
        {
            Debug.Log("스테이지 초기화가 두 번 진행됨");
        }

        stageDataOnLoad = _stageData;

        monsterWaveQueue.Clear();
        for (int i = monsterWaveParent.childCount - 1; i >= 0; i--)
        {
            Destroy(monsterWaveParent.GetChild(i).gameObject);
        }

        scroller.SetScroller(_stageData.BackgroundTypePrefab);


        // ============= 플레이어 캐릭터 초기화 =============

        //키 : 배치정보, 값 : 캐릭터 고유 번호(ID)
        Dictionary<string, long> batchData = GameManager.UserData.PlayData.BatchInfo.Value;
        Dictionary<int, CharacterData> batchDictionary = new Dictionary<int, CharacterData>(batchData.Count);

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



        AddWaveText();
        Debug.Log("다음 웨이브로 이동중...");

        scroller.StartScroll();
        yield return StartCoroutine(combatCamera.StartFocusCO());

        StartCoroutine(combatCamera.ReleaseFocusCO());//줌아웃과 동시에 다음 웨이브 활성화
        monsterWaveQueue[0].gameObject.SetActive(true);
        yield return StartCoroutine(WaitMonsterWaveCO(monsterWaveQueue[0].transform));//몬스터 웨이브 이동시간

        scroller.StopScroll();


        characterManager.StartCombat(monsterWaveQueue[0]);
        monsterWaveQueue[0].StartCombat(characterManager);
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
        if (isCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        isCombatEnd = true;

        Debug.Log("클리어!");
        var stream = GameManager.UserData.StartUpdateStream();
        foreach (var item in stageDataOnLoad.Reward)
        {
            stream.AddDBValue(item.item.Number, item.gain);
        }

        stream.Submit(result =>
        {
            if (false == result)
            {
                Debug.Log("요청 전송에 실패했습니다");
                return;
            }

            // 아이템 획득 팝업 + 확인 클릭시 메인 화면으로
            ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(stageDataOnLoad.Reward);
            popupInstance.Title.text = "스테이지 클리어!";
            popupInstance.onPopupClosed += GameManager.Instance.LoadMainScene;
        });
    }

    protected virtual void OnDefeat()
    {
        if (isCombatEnd)
        {
            Debug.Log("이미 전투가 종료됨");
            return;
        }

        isCombatEnd = true;

        ItemGainPopup popupInstance = GameManager.OverlayUIManager.PopupItemGain(null);
        popupInstance.Title.text = "패배...";
        popupInstance.onPopupClosed += GameManager.Instance.LoadMainScene;
    }

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

        isCombatEnd = false;

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
