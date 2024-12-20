using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class StageManager : MonoBehaviour
{
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

    private void Start()
    {
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

    IEnumerator GoNextWaveCO()
    {
        monsterWaveQueue.RemoveAt(0);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (CheckCharactersWait())
                break;

            Debug.Log("이동중...");

        }


        if (monsterWaveQueue.Count <= 0)
        {
            Debug.Log("클리어!");
            yield break;
        }

        Debug.Log("S눌림");
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
