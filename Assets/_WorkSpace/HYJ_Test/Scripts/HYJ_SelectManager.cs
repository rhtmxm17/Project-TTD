using Firebase.Database;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HYJ_SelectManager : MonoBehaviour
{
    [Header("현재 선택한 위치&유닛 정보")]
    [SerializeField] public int curPos; //현재 선택한 위치
    [SerializeField] public int curUnitIndex; //현재 선택한 유닛의 고유번호

    [Header("버튼 리스트")]
    [SerializeField] List<Transform> buttonsTransformList;
    
    [Header("필수 설정 사항")]
    [SerializeField] Button resetBatchButton; // 
    [SerializeField] Button enterStageButton; //스테이지 진입 버튼
    [SerializeField] OutskirtsUI outskirtsUI; //뒤돌아가기 버튼 등록용
    [SerializeField] HYJ_CharacterSelect batchButtonPrefab; //배치버튼 프리팹
    [SerializeField] Transform batchWindow; //배치버튼 윈도우
    [SerializeField] int buttonCnt; //버튼 생성 수
    [SerializeField] public GameObject CharacterSelectPanel; // 캐릭터 선택 창 
    [SerializeField] public GameObject CantPosUI; // 선택 불가 팝업 -> 5개 유닛이 이미 다 배치 되었을 때의 팝업
    [SerializeField] GameObject UnitChangeUI; // 유닛 변경 확인 팝업 -> 변경하시겠습니까?
    [SerializeField] GameObject cantStartUI;
    [SerializeField] private TMP_Text userAndStagePower;    
    
    // 키 값은 위치 / 밸류 값은 유닛 고유번호;
    public Dictionary<int, int> battleInfo = new Dictionary<int, int>();

    // 배치한 캐릭터 전투력 표시용
    private float batchUnitsPower;
    private float stagePower;
    
    private void Start()
    {
        enterStageButton.onClick.AddListener(LoadBattleScene);
        resetBatchButton.onClick.AddListener(ResetBatch);
        outskirtsUI.PrevMenu = GameManager.Instance.sceneChangeArgs.prevScene;

        // 편성 씬을 벗어날 때의 동작
        outskirtsUI.ReturnButton.onClick.AddListener(OnCancelEnterStage);
        outskirtsUI.HomeButton.onClick.AddListener(OnCancelEnterStage);
        
        GameManager.UserData.PlayData.BatchInfo.onValueChanged += (() =>
        {
            Debug.Log("편성 정보가 갱신됨");
        });
        GameManager.UserData.onLoadUserDataCompleted.AddListener(() =>
        {
            Debug.Log("유저 정보 불러오기 완료 확인");
        });

        // ============= 플레이어 캐릭터 초기화 =============
        //키 : 배치정보, 값 : 캐릭터 고유 번호(ID)
        Dictionary<string, long> batchData = GameManager.UserData.PlayData.BatchInfo.Value;

        foreach (var pair in batchData)
        {
            battleInfo[int.Parse(pair.Key)] = (int)pair.Value;
        }
        
        if (battleInfo.Count > 5)
        {
            Debug.LogError("불러온 유저 배치 정보 오류(5개 보다 많은 배치)");
        }
        
        // ============= 편성 타일 생성 ===================
        List<StageData.BuffInfo> curStageBuff = GameManager.Instance.sceneChangeArgs.stageData.TileBuff;
        for (int i = 0; i < buttonCnt; i++)
        {
            var obj = Instantiate(batchButtonPrefab, batchWindow);
            buttonsTransformList.Add(obj.transform);
            obj.InitDataPosBtn(i, CharacterSelectPanel, CantPosUI);
            
            foreach (StageData.BuffInfo checkBuff in curStageBuff)
            {
                if (checkBuff.tileIndex == i)
                {
                    obj.GetComponent<HYJ_BtnBuff>().BuffInput(checkBuff.type);
                }
            }
        }
        
        // ============= 유저 전투력 / 스테이지 전투력 ===================
        UpdateBatchUnitsPower();
    }

    /// <summary>
    /// 배치 저장
    /// </summary>
    /// <param name="onCompleteCallback"></param>
    private void SaveBatch(UnityAction<bool> onCompleteCallback) 
    {
        Dictionary<string, long> updates = new Dictionary<string, long>();
        foreach (var pair in battleInfo)
        {
            updates[pair.Key.ToString()] = pair.Value;
        }

        GameManager.UserData.StartUpdateStream()
            .SetDBDictionary(GameManager.UserData.PlayData.BatchInfo, updates)
            .Submit(onCompleteCallback);
    }

    /// <summary>
    /// 전투시작 버튼
    /// </summary>
    public void LoadBattleScene()
    {
        // 전투 시작
        if (battleInfo.Count < 1)
        {
            //FixMe: 팝업창 만들어서 하나 이상의 캐릭터를 배치해야 시작할 수 있다고 알려주기
            Debug.Log("하나 이상의 캐릭터를 배치해야 게임을 시작할 수 있습니다.");
            cantStartUI.SetActive(true);
        }
        else
        {
            SaveBatch(result =>
            {
                if (false == result)
                {
                    Debug.LogWarning("db 접속에 실패");
                    return;
                }

                GameManager.Instance.LoadStageScene();
            });
        }
    }

    /// <summary>
    /// 뒤로가기
    /// </summary>
    public void OnCancelEnterStage()
    {
        SaveBatch(result =>
        {
            if (false == result)
            {
                Debug.LogWarning("db 접속에 실패");
                return;
            }
        });
    }

    public void AddPosBtn()
    {
        buttonsTransformList[curPos].GetComponent<HYJ_CharacterSelect>().AddBatch(curPos,curUnitIndex);
    }
    
    public void ResetPosBtn()
    {
        buttonsTransformList[curPos].GetComponent<HYJ_CharacterSelect>().RemoveBatch(curPos);
    }

    /// <summary>
    /// 유닛 변경 확인 버튼
    /// </summary>
    public void ChangePosBtn()
    {
        if (battleInfo.ContainsKey(curPos)) //현재 위치에 배치되어 있는 유닛이 있을 경우 > 해당 유닛을 해제
        {
            Debug.Log("1111111");
            ResetPosBtn();
        }
        
        if(battleInfo.ContainsValue(curUnitIndex))
        {
            int curUnitIndexsPos = battleInfo.FirstOrDefault(x => x.Value == curUnitIndex).Key;
            Debug.Log($"현재 선택한 유닛의 위치{curUnitIndexsPos}");
            buttonsTransformList[curUnitIndexsPos].GetComponent<HYJ_CharacterSelect>().RemoveBatch(curUnitIndexsPos);
        }
        
        buttonsTransformList[curPos].GetComponent<HYJ_CharacterSelect>().AddBatch(curPos, curUnitIndex);
        Debug.Log($"현재 선택한 유닛의 위치{curPos}");
        UnitChangeUI.SetActive(false);
        CharacterSelectPanel.SetActive(false);
    }

    /// <summary>
    /// 배치 초기화
    /// </summary>
    public void ResetBatch()
    {
        for (int i = 0; i < buttonCnt; i++)
        {
            buttonsTransformList[i].GetComponent<HYJ_CharacterSelect>().RemoveBatch(i);
        }
        UpdateBatchUnitsPower();
    }

    /// <summary>
    /// 배치 버튼 색상 초기화
    /// </summary>
    public void ChangeAllBtnColorOff()
    {
        foreach (Transform btn in buttonsTransformList)
        {
            btn.GetComponent<HYJ_ChangeBtnColor>().ChangeBtnColor(false);
            UpdateBatchUnitsPower();
        }
    }

    /// <summary>
    /// 배치 유닛 전투력 업데이트
    /// </summary>
    private void UpdateBatchUnitsPower()
    {
        stagePower = 0;
        //배치 유닛 전투력 계산
        if (battleInfo.Count > 0)
        {
            batchUnitsPower = 0;
            foreach (int infoKey in battleInfo.Keys)
            {
                batchUnitsPower += GameManager.TableData.GetCharacterData(battleInfo[infoKey]).PowerLevel;
            }
        }
        else
        {
            batchUnitsPower = 0;
        }
        
        //스테이지 전투력 계산
        StageData curStageData = GameManager.Instance.sceneChangeArgs.stageData;
        foreach (var iWave in curStageData.Waves)
        {
            foreach (var iWaveMonster in iWave.monsters)
            {
                CharacterData.Status monsterStatus = iWaveMonster.character.StatusTable;
                stagePower +=
                    monsterStatus.defensePointBase + monsterStatus.healthPointBase + monsterStatus.attackPointBase + 
                (monsterStatus.healthPointGrouth + monsterStatus.attackPointGrowth + monsterStatus.defensePointGrouth) 
                * iWaveMonster.level;
            }
        }
        
        
        //배치 유닛 전투력&스테이지 전투력 표기
        if (batchUnitsPower < stagePower)
        {
            userAndStagePower.text =
                $"<color=white>적 전투력</color>\n<color=red>{(int)stagePower}</color>\n<color=white>내 전투력</color>\n<color=red>{(int)batchUnitsPower}</color>";
        }
        else
        {
            userAndStagePower.text =
                $"<color=white>적 전투력</color>\n<color=green>{(int)stagePower}</color>\n<color=white>내 전투력</color>\n<color=green>{(int)batchUnitsPower}</color>";
        }
        

        if (GameManager.Instance.sceneChangeArgs.stageType == StageType.BOSS)
        {
            userAndStagePower.text = $"{batchUnitsPower}/?????";
        }
    }
}
