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

    public void InitMonsterData(StageData.MonsterInfo monsterInfo)
    {
        CharacterData.Status monsterStatus = monsterInfo.character.StatusTable;
        monsterImage.GetComponent<Image>().sprite = monsterInfo.character.FaceIconSprite;
        SetMonsterRace(monsterStatus.type);
        SetMonsterClass(monsterStatus.roleType);
        
        levelText.text = "Lv." + monsterInfo.level.ToString();
        nameText.text = monsterInfo.character.Name;
    }

    private void SetMonsterRace(ElementType monsterElementType)
    {
        switch (monsterElementType.ToString())
        {
            case "NONE" :
                raceImage.GetComponent<Image>().color = Color.white;
                break;
            case "EARTH" :
                raceImage.GetComponent<Image>().color = Color.green;
                break;
            case "WATER" :
                raceImage.GetComponent<Image>().color = Color.blue;
                break;
            case "METAL" :
                raceImage.GetComponent<Image>().color = Color.yellow;
                break;
            case "WOOD" :
                raceImage.GetComponent<Image>().color = Color.gray;
                break;
            case "FIRE" :
                raceImage.GetComponent<Image>().color = Color.red;
                break;
        }
    }

    private void SetMonsterClass(RoleType monsterRoleType)
    {
        switch (monsterRoleType.ToString())
        {
            case "NONE" :
                classImage.GetComponent<Image>().color = Color.white;
                break;
            case "ATTACKER" :
                classImage.GetComponent<Image>().color = Color.red;
                break;
            case "DEFENDER" :
                classImage.GetComponent<Image>().color = Color.blue;
                break;
            case "SUPPORTER" :
                classImage.GetComponent<Image>().color = Color.green;
                break;
        }
    }

    public void SetBoss()
    {
        bossImage.SetActive(true);
    }
}
