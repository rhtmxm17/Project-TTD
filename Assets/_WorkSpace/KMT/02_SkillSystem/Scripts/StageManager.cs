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

    [Header("Characters")]
    [SerializeField]
    CombManager characterManager;
    [SerializeField]
    StageCharacterSetter characterSetter;
    //스테이지 진입 시, 진형 설정에서 캐릭터 정보들을 받아올 것.
    [SerializeField] private List<CharacterData> characterDataList;

    //데이터 테스트용 임시 구조체
    [System.Serializable]
    struct monsterData
    {
        [SerializeField] public List<CharacterData> monsters;

    }

    [Header("Monsters")]
    [SerializeField] Transform monsterWaveParent;
    [SerializeField] List<monsterData> monsterDataList;
    [SerializeField] MonsterWaveSetter monsterWavePrefab;
    [SerializeField]
    List<CombManager> monsterWaveQueue = new List<CombManager>();

    [Header("Party Gauge")]
    [SerializeField]
    float MaxPartyCost;

    [Header("Debug")]
    [Space(20)]
    [SerializeField]
    TextMeshProUGUI costText;
    [SerializeField]
    Slider costSlider;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        //테스트용 로그인 초기화
        StartCoroutine(UserDataManager.InitDummyUser(0));

        characterSetter.InitCharacters(characterDataList);
        InitMonsterWaves(monsterDataList);
    }

    private void InitMonsterWaves(List<monsterData> monsterDataList)
    {

        foreach (monsterData monster in monsterDataList)
        {
            MonsterWaveSetter monsterWave = Instantiate(monsterWavePrefab, monsterWaveParent);
            monsterWave.InitCharacters(monster.monsters);
            monsterWave.gameObject.SetActive(false);
            monsterWave.GetComponent<CombManager>().ListClearedEvent.AddListener(() =>
            {
                StartCoroutine(GoNextWaveCO());
            });
            monsterWaveQueue.Add(monsterWave.GetComponent<CombManager>());
        }

    }

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

    IEnumerator GoNextWaveCO()
    {
        monsterWaveQueue.RemoveAt(0);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (CheckCharactersWait())
                break;

            Debug.Log("캐릭터 복귀중...");

        }


        if (monsterWaveQueue.Count <= 0)
        {
            Debug.Log("클리어!");
            yield break;
        }

        Debug.Log("다음 웨이브로 이동중...");
        //yield return new WaitForSeconds(3f);

        monsterWaveQueue[0].gameObject.SetActive(true);
        characterManager.StartCombat(monsterWaveQueue[0]);
        monsterWaveQueue[0].StartCombat(characterManager);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (monsterWaveQueue.Count <= 0)
            {
                Debug.Log("클리어!");
                return;
            }

            //파티 공유 게이지 충전 시작.
            StartCoroutine(StartPartyCostCO());

            costSlider.value = 0;
            costText.text = PartyCost.ToString();

            Debug.Log("S눌림");
            monsterWaveQueue[0].gameObject.SetActive(true);
            characterManager.StartCombat(monsterWaveQueue[0]);
            monsterWaveQueue[0].StartCombat(characterManager);
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
