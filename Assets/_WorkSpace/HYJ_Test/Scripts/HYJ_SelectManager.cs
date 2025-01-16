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
        updateBatchUnitsPower();
    }

    public void LookLog()
    {
        DatabaseReference baseref = BackendManager.CurrentUserDataRef;

        SaveBatch(result =>
        {
            Debug.Log($"요청 결과:{result}");
        });
    }

    private void SaveBatch(UnityAction<bool> onCompleteCallback) // 배치 저장
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

    public void LoadBattleScene()
    {
        // 전투 시작
        if (battleInfo.Count < 1)
        {
            // TODO : 팝업창 만들어서 하나 이상의 캐릭터를 배치해야 시작할 수 있다고 알려주기
            Debug.Log("하나 이상의 캐릭터를 배치해야 게임을 시작할 수 있습니다.");
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
    /// 스테이지 진입 취소시 동작
    /// </summary>
    public void OnCancelEnterStage()
    {
        // 뒤로가기(배치 창 닫기)
        SaveBatch(result =>
        {
            if (false == result)
            {
                Debug.LogWarning("db 접속에 실패");
                return;
            }
        });
    }

    public void AddPosBtnImage()
    {
        buttonsTransformList[curPos].GetComponent<HYJ_CharacterSelect>().AddBatch(curPos,curUnitIndex);
    }
    
    public void ResetPosBtnImage()
    {
        buttonsTransformList[curPos].GetComponent<HYJ_CharacterSelect>().RemoveBatch(curPos);
    }

    public void ChangePosBtnImage()
    {
        if (battleInfo.ContainsKey(curPos))
        {
            ResetPosBtnImage();
        }
        
        int curUnitIndexsPos = battleInfo.FirstOrDefault(x => x.Value == curUnitIndex).Key;
        buttonsTransformList[curUnitIndexsPos].GetComponent<HYJ_CharacterSelect>().RemoveBatch(curUnitIndexsPos);
        buttonsTransformList[curPos].GetComponent<HYJ_CharacterSelect>().AddBatch(curPos, curUnitIndex);
        
        UnitChangeUI.SetActive(false);
        CharacterSelectPanel.SetActive(false);
    }

    public void ResetBatch()
    {
        for (int i = 0; i < buttonCnt; i++)
        {
            buttonsTransformList[i].GetComponent<HYJ_CharacterSelect>().RemoveBatch(i);
        }
        updateBatchUnitsPower();
    }

    public void ChangeAllBtnColorOff()
    {
        foreach (Transform btn in buttonsTransformList)
        {
            btn.GetComponent<HYJ_ChangeBtnColor>().ChangeBtnColor(false);
            updateBatchUnitsPower();
        }
    }

    public void updateBatchUnitsPower()
    {
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
        
        StageData curStageData = GameManager.Instance.sceneChangeArgs.stageData;
        foreach (var iWave in curStageData.Waves)
        {
            foreach (var iWaveMonster in iWave.monsters)
            {
                stagePower =
                iWaveMonster.character.StatusTable.defensePointBase +
                iWaveMonster.character.StatusTable.healthPointBase +
                iWaveMonster.character.StatusTable.attackPointBase + 
                (iWaveMonster.character.StatusTable.healthPointGrouth +
                 iWaveMonster.character.StatusTable.attackPointGrowth +
                 iWaveMonster.character.StatusTable.defensePointGrouth) 
                * iWaveMonster.level;
            }
        }
        
        userAndStagePower.text = batchUnitsPower+ "/" + stagePower;
    }
}
