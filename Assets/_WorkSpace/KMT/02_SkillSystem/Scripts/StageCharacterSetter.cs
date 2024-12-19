using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CombManager))]
public class StageCharacterSetter : MonoBehaviour
{

    //스테이지 진입 시, 진형 설정에서 캐릭터 정보들을 받아올 것.
    [SerializeField] private List<CharacterData> characterDataList;
    //TODO : 필요시 나중에 타입 지정
    [SerializeField] GameObject characterPrefab;

    [Header("skills")]
    [SerializeField]
    GameObject skillPanel;
    [SerializeField]
    GameObject skillButtonPrefab;


    List<Vector2> position = new List<Vector2>()
    {
        new Vector2(-7.223999f,  5.509857f),
        new Vector2(-7.223999f,  1.889847f),
        new Vector2(-7.223999f, -1.930145f),
        new Vector2(-10.754f,  5.509857f),
        new Vector2(-10.754f,  1.889847f),
        new Vector2(-10.754f, -1.930145f),
        new Vector2(-14.474f,  5.509857f),
        new Vector2(-14.474f,  1.889847f),
        new Vector2(-14.474f, -1.930145f)
    };

    private void Start()
    {
        if (characterDataList.Count <= 0)
            return;

        SpawnCharacterDataSet();
    }

    private void SpawnCharacterDataSet()
    {

        List<GameObject> characters = new List<GameObject>();

        for (int i = 0; i < characterDataList.Count; i++)
        {

            GameObject charObj = Instantiate(characterPrefab, position[i], Quaternion.identity, transform);
            characters.Add(charObj);
            Instantiate(characterDataList[i].ModelPrefab, charObj.transform).name = "Model";

            //테스트용 코드?
            charObj.GetComponent<SpriteRenderer>().sprite = characterDataList[i].FaceIconSprite;

            //스킬 버튼 생성.
            GameObject skillBtn = Instantiate(skillButtonPrefab, skillPanel.transform);
            Skill skillData = characterDataList[i].SkillDataSO;
            skillBtn.transform.GetChild(0).GetComponent<Image>().sprite = characterDataList[i].SkillSprite;
            skillBtn.GetComponent<Button>().onClick.AddListener(() => {
                charObj.GetComponent<Combatable>().OnSkillCommanded(skillData);
            });

        }

        GetComponent<CombManager>().charList = characters;

    }

}
