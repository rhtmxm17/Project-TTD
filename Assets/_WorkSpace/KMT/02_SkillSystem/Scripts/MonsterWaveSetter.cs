using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWaveSetter : MonoBehaviour
{
    //스테이지 진입 시, 진형 설정에서 캐릭터 정보들을 받아올 것.
    //[SerializeField] private List<CharacterData> characterDataList;
    //TODO : 필요시 나중에 타입 지정
    [SerializeField] Combatable monsterPrefab;

    public void InitCharacters(StageData.WaveInfo waveData)
    {
        if (waveData.monsters.Count <= 0)
            return;

        CombManager group = GetComponent<CombManager>();
        List<Combatable> characters = new List<Combatable>();

        foreach (StageData.MonsterInfo monsterData in waveData.monsters)
        {

            Combatable charObj = Instantiate(monsterPrefab, monsterData.pose, Quaternion.identity, transform);
            charObj.onDeadEvent.AddListener((charobj) => { StageManager.Instance.AddPartyCost(1); });
            charObj.InitializeWithLevel(group, monsterData.character, monsterData.level); // 스테이지 정보에 입력된 레벨로 생성

            //테스트용 코드?
            charObj.GetComponent<SpriteRenderer>().sprite = monsterData.character.FaceIconSprite;

            characters.Add(charObj);
        }

        GetComponent<CombManager>().CharList = characters;
    }
}
