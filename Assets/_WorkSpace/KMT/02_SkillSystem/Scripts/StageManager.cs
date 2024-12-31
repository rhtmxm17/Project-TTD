using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance = null;

    public float PartyCost { get; private set; } = 0;

    [SerializeField] StageData stageData;
    private StageData stageDataOnLoad;

    [Header("Characters")]
    [SerializeField]
    CombManager characterManager;
    [SerializeField]
    StageCharacterSetter characterSetter;

    //데이터 테스트용 임시 구조체
    [System.Serializable]
    struct monsterData
    {
        [SerializeField] public List<CharacterData> monsters;

    }

    [Header("Monsters")]
    [SerializeField] Transform monsterWaveParent;
    [SerializeField] MonsterWaveSetter monsterWavePrefab;
    [SerializeField]
    List<CombManager> monsterWaveQueue = new List<CombManager>();

    [Header("Party Gauge")]
    [SerializeField]
    float MaxPartyCost;

    [Header("Reward UI")]
    [SerializeField] ItemGainPopup itemGainPopupPrefab;
    protected ItemGainPopup ItemGainPopupPrefab => itemGainPopupPrefab;

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

        // ============= 플레이어 캐릭터 초기화 =============

        //키 : 배치정보, 값 : 캐릭터 고유 번호(ID)
        Dictionary<string, long> batchData = GameManager.UserData.PlayData.BatchInfo.Value;
        Dictionary<int, CharacterData> batchDictionary = new Dictionary<int, CharacterData>(batchData.Count);

        foreach (var pair in batchData)
        {
            batchDictionary[int.Parse(pair.Key)] =
                GameManager.TableData.GetCharacterData((int)pair.Value);
        }

        characterSetter.InitCharacters(batchDictionary);

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
        //yield return new WaitForSeconds(3f);

        monsterWaveQueue[0].gameObject.SetActive(true);
        characterManager.StartCombat(monsterWaveQueue[0]);
        monsterWaveQueue[0].StartCombat(characterManager);
    }

    protected virtual void OnClear()
    {
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
            ItemGainPopup popupInstance = Instantiate(itemGainPopupPrefab, GameManager.PopupCanvas);
            popupInstance.Initialize(stageDataOnLoad.Reward);
            popupInstance.Title.text = "스테이지 클리어!";
            popupInstance.onPopupClosed += GameManager.Instance.LoadMainScene;
        });
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

        //파티 공유 게이지 충전 시작.
        StartCoroutine(StartPartyCostCO());

        costSlider.value = 0;
        costText.text = PartyCost.ToString();

        monsterWaveQueue[0].gameObject.SetActive(true);
        characterManager.StartCombat(monsterWaveQueue[0]);
        monsterWaveQueue[0].StartCombat(characterManager);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F눌림");
            characterManager.EndCombat();
            monsterWaveQueue[0].EndCombat();
            monsterWaveQueue.RemoveAt(0);
        }
    }
}
