using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWaveSetter : MonoBehaviour
{

    [SerializeField] Combatable monsterPrefab;

    public void InitCharacters(StageData.WaveInfo waveData)
    {
        if (waveData.monsters.Count <= 0)
            return;

        CombManager group = GetComponent<CombManager>();
        List<Combatable> characters = new List<Combatable>();

        foreach (StageData.MonsterInfo monsterData in waveData.monsters)
        {

            Combatable charObj = Instantiate(monsterPrefab, new Vector3(monsterData.pose.x, 0, monsterData.pose.y), Quaternion.identity, transform);
            charObj.onDeadEvent.AddListener((charobj) => { StageManager.Instance.AddPartyCost(5f); });
            charObj.InitializeWithLevel(group, monsterData.character, monsterData.level); // 스테이지 정보에 입력된 레벨로 생성

            characters.Add(charObj);
        }

        GetComponent<CombManager>().CharList = characters;
    }
}
