using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HYJ_StageEnemyInfo : MonoBehaviour
{
    [SerializeField] TMP_Text stageNameText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] GameObject monsterPrefab;
    private List<int> monsterList;

    private void Awake()
    {
        monsterList = new List<int>();
    }

    private void Start()
    {
        InitStageData();
    }

    /// <summary>
    /// 스테이지 정보 초기 설정(스테이지 이름, 몬스터와 아이템 정보)
    /// </summary>
    private void InitStageData()
    {
        StageData curStageData = GameManager.Instance.sceneChangeArgs.stageData;
        stageNameText.text = curStageData.StageName;//스테이지 이름 표기

        foreach (var iWave in curStageData.Waves)
        {
            foreach (var iWaveMonster in iWave.monsters)
            {
                if (!monsterList.Contains(iWaveMonster.character.Id))
                {
                    monsterList.Add(iWaveMonster.character.Id);
                    GameObject iMonster = Instantiate(monsterPrefab, transform);
                    iMonster.GetComponent<HYJ_MonsterInfo>().InitMonsterData(iWaveMonster);
                
                    if (GameManager.Instance.sceneChangeArgs.stageType == StageType.BOSS)
                    {
                        iMonster.GetComponent<HYJ_MonsterInfo>().SetBoss();
                    }    
                }
            }
        }
    }
}
