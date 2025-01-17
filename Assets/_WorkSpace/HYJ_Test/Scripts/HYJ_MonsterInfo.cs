using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class HYJ_MonsterInfo : MonoBehaviour
{
    [SerializeField] Image monsterImage;
    [SerializeField] GameObject bossImage;
    [SerializeField] Image raceImage;
    [SerializeField] Image classImage;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text nameText;

    /// <summary>
    /// 몬스터 정보 초기설정
    /// </summary>
    /// <param name="monsterInfo"></param>
    public void InitMonsterData(StageData.MonsterInfo monsterInfo)
    {
        CharacterData.Status monsterStatus = monsterInfo.character.StatusTable;
        monsterImage.GetComponent<Image>().sprite = monsterInfo.character.FaceIconSprite;
        SetMonsterRace(monsterStatus.type);//몬스터 속성 표기
        SetMonsterClass(monsterStatus.roleType);//몬스터 역할군 표기
        
        levelText.text = "Lv." + monsterInfo.level.ToString();
        nameText.text = monsterInfo.character.Name;
    }

    /// <summary>
    /// 몬스터 속성 표기
    /// </summary>
    /// <param name="monsterElementType"></param>
    private void SetMonsterRace(ElementType monsterElementType)
    {
        //FixMe:현재는 색상으로 표현하고있는데 다른 표현법이 생기면 변경
        Image raceImg = raceImage.GetComponent<Image>(); 
        switch (monsterElementType.ToString())
        {
            case "NONE" :
                raceImg.color = Color.white;
                break;
            case "EARTH" :
                raceImg.color = Color.green;
                break;
            case "WATER" :
                raceImg.color = Color.blue;
                break;
            case "METAL" :
                raceImg.color = Color.yellow;
                break;
            case "WOOD" :
                raceImg.color = Color.gray;
                break;
            case "FIRE" :
                raceImg.color = Color.red;
                break;
        }
    }

    /// <summary>
    /// 몬스터 역할군 표기
    /// </summary>
    /// <param name="monsterRoleType"></param>
    private void SetMonsterClass(RoleType monsterRoleType)
    {
        //FixMe:현재는 색상으로 표현하고있는데 다른 표현법이 생기면 변경
        Image classImg = classImage.GetComponent<Image>();
        switch (monsterRoleType.ToString())
        {
            case "NONE" :
                classImg.color = Color.white;
                break;
            case "ATTACKER" :
                classImg.color = Color.red;
                break;
            case "DEFENDER" :
                classImg.color = Color.blue;
                break;
            case "SUPPORTER" :
                classImg.color = Color.green;
                break;
        }
    }

    public void SetBoss()
    {
        bossImage.SetActive(true);
    }
}
