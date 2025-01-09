using Firebase.Database;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HYJ_SelectManager : MonoBehaviour
{
    [SerializeField] public int curPos;
    [SerializeField] public int curUnitIndex;

    [Header("buttonList")]
    [SerializeField]
    List<Transform> buttonsTransformList;

    [SerializeField] Button enterStageButton;
    [SerializeField] Button cancelButton;

    [Header("이미지 프리팹")]
    [SerializeField] public GameObject ChImagePrefab;

    [SerializeField]
    HYJ_CharacterSelect batchButtonPrefab;
    [SerializeField]
    Transform batchButtonsTransform;
    [SerializeField]
    int buttonCnt;
    [SerializeField] public GameObject CharacterSelectPanel; // 캐릭터 선택 창 
    [SerializeField] public GameObject CantPosUI; // 선택 불가 팝업 -> 5개 유닛이 이미 다 배치 되었을 때의 팝업
    [SerializeField] GameObject UnitChangeUI; // 유닛 변경 확인 팝업 -> 변경하시겠습니까?
    
    // 키 값은 위치 / 밸류 값은 유닛 고유번호;
    public Dictionary<int, int> battleInfo = new Dictionary<int, int>();

    private void Start()
    {
        enterStageButton.onClick.AddListener(LoadBattleScene);
        cancelButton.onClick.AddListener(CancelEnterStage);
        
        

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
        else if (battleInfo.Count > 0)
        {
            //SetBattleInfo(battleInfo);
        }
        
        for (int i = 0; i < buttonCnt; i++)
        {
            var obj = Instantiate(batchButtonPrefab, batchButtonsTransform);
            buttonsTransformList.Add(obj.transform);
            obj.InitDataPosBTN(i, CharacterSelectPanel, CantPosUI);
            
            //obj.GetComponent<HYJ_PosBTN>().BuffInput(true,true,true);
        }
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
    
    /// <summary>
    /// 캐릭터 이미지 생성 - AddBatch에 사용
    /// </summary>
    /// <param name="posIdx"></param> 이미지의 생성 위치
    /// <param name="charIdx"></param> 생성할 캐릭터의 고유 번호
    public void SetCharacterImage(int posIdx, int charIdx)
    {
        var chImage = Instantiate(ChImagePrefab, buttonsTransformList[posIdx]);
        CharacterData chData = GameManager.TableData.GetCharacterData(charIdx);
        chImage.transform.localPosition = Vector3.zero;
        chImage.GetComponent<Image>().sprite = chData.FaceIconSprite;
        chImage.GetComponent<HYJ_CharacterImage>().curPos = posIdx;
        chImage.GetComponent<HYJ_CharacterImage>().unitIndex = charIdx;
    }

    /// <summary>
    /// 캐릭터 이미지 삭제 - RemoveBatch에 사용
    /// </summary>
    /// <param name="posIdx"></param> 삭제할 이미지의 위치
    public void RemoveCharacterImage(int posIdx)
    {
        Destroy(buttonsTransformList[posIdx].GetComponentInChildren<HYJ_CharacterImage>().gameObject);
    }

    /// <summary>
    /// 캐릭터 이미지 이동 - changeBatch에 사용
    /// </summary>
    /// <param name="fromIdx"></param> 이동할 위치 번호
    /// <param name="destIdx"></param> 출발 번호
    public void ChangeImagePos(int fromIdx, int destIdx)
    {
        RemoveCharacterImage(destIdx);
        buttonsTransformList[fromIdx].GetComponentInChildren<HYJ_CharacterImage>().transform.SetParent(buttonsTransformList[destIdx].transform);

        HYJ_CharacterImage[] transL = buttonsTransformList[destIdx].GetComponentsInChildren<HYJ_CharacterImage>();
        RectTransform trans = transL[transL.Length - 1].GetComponent<RectTransform>();
        trans.anchoredPosition = Vector3.zero;
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

    public void CancelEnterStage()
    {
        // 뒤로가기(배치 창 닫기)
        SaveBatch(result =>
        {
            if (false == result)
            {
                Debug.LogWarning("db 접속에 실패");
                return;
            }

            GameManager.Instance.LoadMainScene();
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
}
