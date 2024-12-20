using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWaveSetter : MonoBehaviour
{
    //스테이지 진입 시, 진형 설정에서 캐릭터 정보들을 받아올 것.
    //[SerializeField] private List<CharacterData> characterDataList;
    //TODO : 필요시 나중에 타입 지정
    [SerializeField] Combatable monsterPrefab;


    List<Vector2> position = new List<Vector2>()
    {
        new Vector2(7.223999f,  5.509857f),
        new Vector2(7.223999f,  1.889847f),
        new Vector2(7.223999f, -1.930145f),
        new Vector2(10.754f,  5.509857f),
        new Vector2(10.754f,  1.889847f),
        new Vector2(10.754f, -1.930145f),
        new Vector2(14.474f,  5.509857f),
        new Vector2(14.474f,  1.889847f),
        new Vector2(14.474f, -1.930145f)
    };

    public void InitCharacters(List<CharacterData> characterDatas)
    {
        if (characterDatas.Count <= 0)
            return;

        SpawnCharacterDataSet(characterDatas);
    }

    private void SpawnCharacterDataSet(List<CharacterData> characterDataList)
    {
        CombManager group = GetComponent<CombManager>();
        List<Combatable> characters = new List<Combatable>();

        for (int i = 0; i < characterDataList.Count; i++)
        {

            Combatable charObj = Instantiate(monsterPrefab, position[i], Quaternion.identity, transform);
            characters.Add(charObj);
            GameObject model = Instantiate(characterDataList[i].ModelPrefab, charObj.transform);
            model.name = "Model";

            charObj.Initialize(model.GetComponent<Animator>(), group, characterDataList[i]);

            //테스트용 코드?
            charObj.GetComponent<SpriteRenderer>().sprite = characterDataList[i].FaceIconSprite;

        }

        GetComponent<CombManager>().CharList = characters;

    }
}