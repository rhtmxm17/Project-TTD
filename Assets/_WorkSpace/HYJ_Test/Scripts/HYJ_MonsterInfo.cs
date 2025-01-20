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

    [Header("속성 이미지")]
    [SerializeField] private Sprite nomalImage;
    [SerializeField] private Sprite earthImage;
    [SerializeField] private Sprite waterImage;
    [SerializeField] private Sprite metalImage;
    [SerializeField] private Sprite woodImage;
    [SerializeField] private Sprite fireImage;
    
    [Header("역할군 이미지")]
    [SerializeField] private Sprite attackerImage;
    [SerializeField] private Sprite defenderImage;
    [SerializeField] private Sprite supporterImage;
    
    private Color imgColor;
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

        levelText.text = $"Lv.{monsterInfo.level}";
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
                raceImage.sprite = nomalImage;
                raceImg.color = Color.white;
                ColorUtility.TryParseHtmlString("#FFFFFF", out imgColor);
                break;
            case "EARTH" :
                raceImage.sprite = earthImage;
                ColorUtility.TryParseHtmlString("#893D00", out imgColor);
                break;
            case "WATER" :
                raceImage.sprite = waterImage;
                ColorUtility.TryParseHtmlString("#009AFF", out imgColor);
                break;
            case "METAL" :
                raceImage.sprite = metalImage;
                ColorUtility.TryParseHtmlString("#FFF500", out imgColor);
                break;
            case "WOOD" :
                raceImage.sprite = woodImage;
                ColorUtility.TryParseHtmlString("#00D532", out imgColor);
                break;
            case "FIRE" :
                raceImage.sprite = fireImage;
                ColorUtility.TryParseHtmlString("#FF0C00", out imgColor);
                break;
        }
        raceImage.color = imgColor;
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
                classImage.sprite = attackerImage;
                ColorUtility.TryParseHtmlString("#FF0C00", out imgColor);
                break;
            case "DEFENDER" :
                classImage.sprite = defenderImage;
                ColorUtility.TryParseHtmlString("#0007FF", out imgColor);
                break;
            case "SUPPORTER" :
                classImage.sprite = supporterImage;
                ColorUtility.TryParseHtmlString("#F8FF00", out imgColor);
                break;
        }
        classImg.color = imgColor;
    }

    public void SetBoss()
    {
        bossImage.SetActive(true);
    }
}
