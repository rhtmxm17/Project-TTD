using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
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
        SetMonsterRace(monsterStatus.type.ToString());
        SetMonsterClass(monsterStatus.roleType.ToString());
        
        levelText.text = monsterInfo.level.ToString();
        nameText.text = monsterInfo.character.Name;
    }

    private void SetMonsterRace(string monsterRace)
    {
        switch (monsterRace)
        {
            case "무속성" :
                monsterImage.GetComponent<Image>().color = Color.white;
                break;
            case "풀" :
                monsterImage.GetComponent<Image>().color = Color.green;
                break;
            case "물" :
                monsterImage.GetComponent<Image>().color = Color.blue;
                break;
            case "금" :
                monsterImage.GetComponent<Image>().color = Color.yellow;
                break;
            case "땅" :
                monsterImage.GetComponent<Image>().color = Color.gray;
                break;
            case "불" :
                monsterImage.GetComponent<Image>().color = Color.red;
                break;
        }
    }

    private void SetMonsterClass(string monsterClass)
    {
        switch (monsterClass)
        {
            case "공격형" :
                monsterImage.GetComponent<Image>().color = Color.red;
                break;
            case "방어형" :
                monsterImage.GetComponent<Image>().color = Color.blue;
                break;
            case "지원형" :
                monsterImage.GetComponent<Image>().color = Color.green;
                break;
        }
    }

    public void SetBoss()
    {
        bossImage.SetActive(true);
    }
}
