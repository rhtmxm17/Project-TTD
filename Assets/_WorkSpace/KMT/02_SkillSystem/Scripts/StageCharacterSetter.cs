using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CombManager))]
public class StageCharacterSetter : MonoBehaviour
{

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

    public void InitCharacters(Dictionary<int, CharacterData> characterDatas)
    {
        if (characterDatas.Count <= 0)
            return;

        GetComponent<CombManager>().ListClearedEvent.AddListener(() => { Debug.Log("전☆멸"); });

        SpawnCharacterDataSet(characterDatas);

    }


    private void SpawnCharacterDataSet(Dictionary<int, CharacterData> characterDataList)
    {
        CombManager group = GetComponent<CombManager>();
        List<Combatable> characters = new List<Combatable>();

        foreach (var pair in characterDataList)
        {

            CharacterCombatable charObj = Instantiate(characterPrefab, position[pair.Key], Quaternion.identity, transform);
            characters.Add(charObj);

            charObj.Initialize(group, pair.Value);

            charObj.InitCharacterData(
                            Instantiate(basicSkillButtonPrefab, skillPanel.transform),
                            Instantiate(secondSkillButtonPrefab, secondSkillPanel.transform));



        }

        GetComponent<CombManager>().CharList = characters;

    }

}
