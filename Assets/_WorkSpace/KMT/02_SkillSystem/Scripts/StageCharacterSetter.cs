using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CombManager))]
public class StageCharacterSetter : MonoBehaviour
{

    //스테이지 진입 시, 진형 설정에서 캐릭터 정보들을 받아올 것.
    //[SerializeField] private List<CharacterData> characterDataList;
    //TODO : 필요시 나중에 타입 지정
    [SerializeField] CharacterCombatable characterPrefab;

    [Header("skills")]
    [SerializeField]
    GameObject skillPanel;
    [SerializeField]
    BasicSkillButton basicSkillButtonPrefab;
    [SerializeField]
    SecondSkillButton secondSkillButtonPrefab;

    [Header("Second Skills")]
    [SerializeField]
    GameObject secondSkillPanel;

    [SerializeField]
    List<Transform> spawnPositions;

    List<Vector2> position = new List<Vector2>();

    private void Awake()
    {
        foreach (Transform t in spawnPositions)
        {
            position.Add(t.position);
        }
    }

    public void InitCharacters(List<CharacterData> characterDatas)
    {
        if (characterDatas.Count <= 0)
            return;

        GetComponent<CombManager>().ListClearedEvent.AddListener(() => { Debug.Log("전☆멸"); });

        SpawnCharacterDataSet(characterDatas);

    }


    private void SpawnCharacterDataSet(List<CharacterData> characterDataList)
    {
        CombManager group = GetComponent<CombManager>();
        List<Combatable> characters = new List<Combatable>();

        for (int i = 0; i < characterDataList.Count; i++)
        {

            CharacterCombatable charObj = Instantiate(characterPrefab, position[i], Quaternion.identity, transform);
            characters.Add(charObj);
            GameObject model = Instantiate(characterDataList[i].ModelPrefab, charObj.transform);
            model.name = "Model";

            charObj.Initialize(model.GetComponent<Animator>(), group, characterDataList[i]);

            //테스트용 코드?
            charObj.GetComponent<SpriteRenderer>().sprite = characterDataList[i].FaceIconSprite;

            charObj.InitCharacterData(characterDataList[i],
                                        Instantiate(basicSkillButtonPrefab, skillPanel.transform),
                                        Instantiate(secondSkillButtonPrefab, secondSkillPanel.transform));


        }

        GetComponent<CombManager>().CharList = characters;

    }

}
