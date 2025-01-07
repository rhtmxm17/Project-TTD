using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HYJ_CharacterSelect : MonoBehaviour
{
    // 캐릭터 (9종)
    // 캐릭터 데이터 구조 : index(ID)
    [SerializeField] GameObject CharacterSelectPanel; // 캐릭터 선택 창 
    GameObject CantPosUI; // 선택 불가 팝업 -> 5개 유닛이 이미 다 배치 되었을 때의 팝업
    public int posNum; // 위치 번호

    [Header("공용 설정")]
    [SerializeField]
    HYJ_SelectManager SelectM;

    [Header("위치 버튼 설정")]
    [SerializeField] GameObject posBTNImage;
    
    [Header("유닛 버튼 설정")]
    [SerializeField] int unitIndex; // 유닛 번호
    [SerializeField] Image characterImage; // 캐릭터 이미지
    [SerializeField] TMP_Text levelText; // 캐릭터 레벨 텍스트
    [SerializeField] TMP_Text raceText; //캐릭터 종족 텍스트
    [SerializeField] TMP_Text classText; // 캐릭터 역할 텍스트(탱커/딜러/힐러)
    [SerializeField] TMP_Text attacktypeText; // 캐릭터 공격 타입 텍스트 (단일/광역)
    [SerializeField] GameObject UnitChangeUI; // 유닛 변경 확인 팝업 -> 변경하시겠습니까?


    public void InitDataPosBTN(int PosIdx, GameObject CharacterSelectPanel, GameObject CantPosUI)
    {
        SelectM = gameObject.GetComponentInParent<Transform>().GetComponentInParent<HYJ_SelectManager>();
        this.CharacterSelectPanel = CharacterSelectPanel;
        this.CantPosUI = CantPosUI;

        posNum = PosIdx;
        transform.GetComponentInChildren<TextMeshProUGUI>().text = PosIdx.ToString();
        if (CheckPos(posNum))
        {
            SetBTNChImage(true, SelectM.battleInfo[posNum]);
        }
    }

    public void InitDataUnitBTN(HYJ_SelectManager manager, int unitIdx, GameObject unitChangeUI)
    {
        SelectM = manager;

        CharacterData chData = GameManager.TableData.GetCharacterData(unitIdx);
        //chType.FaceIconSprite.texture;
        // Text. ~~~ chType.Level.Value.ToString();
        GetComponentInChildren<TextMeshProUGUI>().text = $"{unitIdx.ToString()}번 유닛";
        unitIndex = unitIdx;
        UnitChangeUI = unitChangeUI;

        characterImage.GetComponent<Image>().sprite = chData.FaceIconSprite;
        levelText.text = chData.Level.Value.ToString();
        //raceText.text = chData.
        //classText.text = chData.
        //attacktypeText.text = chData.
    }

    public void SelectPos()
    {
        // 위치를 결정하는 버튼을 눌렀을 때
        if (SelectM.battleInfo.Count >= 5 && !CheckPos(posNum)) // 위치 리스트가 5개 초과 & 이미 선택한 위치가 아닐 경우
        {
            CantPosUI.SetActive(true);
        }
        //else if (CheckPos(posNum)) // 위치 리스트가 5개 초과 & 이미 선택한 위치일 경우 
        //{
        //    SelectM.curPos = posNum;
        //    CharacterSelectPanel.SetActive(true);
        //}
//
        //else if (SelectM.battleInfo.Count < 5 && !CheckPos(posNum)) // 위치 리스트가 5개 보다 적고
        //{
        //    SelectM.curPos = posNum;
        //    CharacterSelectPanel.SetActive(true);
        //}

        else
        {
            SelectM.curPos = posNum;
            CharacterSelectPanel.SetActive(true);
        }
    }

    public bool CheckPos(int posNum) // 위치가 선택되어 있는지 확인하기
    {
        if (SelectM.battleInfo.ContainsKey(posNum))
        {
            return true;
        }

        return false;
    }

    public void SelectUnit()
    {
        SelectM.curUnitIndex = unitIndex;
        if (CheckPos(SelectM.curPos)) // 선택한 위치에 유닛이 배치되어 있을 경우
        {
            if (CheckUnitPos(SelectM.curUnitIndex)) // 선택한 위치에 선택된 유닛이 이미 배치되어 있는 경우
            {
                // 아무동작 x
                Debug.Log("이미 이 위치에 해당 유닛이 배치되어 있습니다.");
                return;
            }
            else // 선택한 위치에 선택한 유닛이 다른 위치에 배치되어 있는 경우
            {
                UnitChangeUI.SetActive(true);
            }
        }
        else // 선택한 위치에 유닛이 배치되어 있지 않은 경우
        {
            if (CheckUnit(SelectM.curUnitIndex)) // 선택한 유닛이 다른 위치에 배치된 경우
            {
                UnitChangeUI.SetActive(true);
            }
            else // 선택한 유닛이 어느 위치에도 배치되지 않은 경우
            {
                AddBatch(SelectM.curPos, SelectM.curUnitIndex);
                SelectM.CharacterSelectPanel.SetActive(false);
            }
        }
    }

    public void ChangeUnit() // 유닛변경 확인 버튼
    {
        if (CheckPos(SelectM.curPos)) // 선택된 위치에 유닛이 배치되어 있을 경우
        {
            RemoveBatch(SelectM.curPos);
        }
        
        
        int unitPos = SelectM.battleInfo.FirstOrDefault(x => x.Value == SelectM.curUnitIndex).Key; // 딕셔너리 밸류값(유닛 고유번호)를 갖고 있는 키 값을 찾기
        ChangeBatch(unitPos, SelectM.curPos, SelectM.curUnitIndex); //
        UnitChangeUI.SetActive(false);
        CharacterSelectPanel.SetActive(false);
    }

    public bool CheckUnit(int unitIndex)
    {
        // 선택한 유닛이 이미 배치되어 있는지 검색
        if (SelectM.battleInfo.ContainsValue(unitIndex))
        {
            return true;
        }

        return false;
    }

    public bool CheckUnitPos(int unitIndex)
    {
        // 딕셔너리 밸류값(유닛 고유번호)를 갖고 있는 키 값을 찾기
        unitIndex = SelectM.battleInfo.FirstOrDefault(x => x.Value == SelectM.curUnitIndex).Key;

        // 이미 해당 위치에 선택한 유닛이 배치되어 있을 경우
        if (unitIndex == SelectM.curPos)
        {
            return true;
        }
        return false;
    }

    public void ReleaseUnit()
    {
        // 배치된 유닛을 해제하기
        if (CheckPos(SelectM.curPos))
        {
            // 선택한 키 값이 이미 등록되어 있을 때
            RemoveBatch(SelectM.curPos);
        }
    }

    void AddBatch(int key, int value)
    {
        SelectM.battleInfo.Add(key, value);     // 현재 키 / 밸류 딕셔너리에 추가
        
        //SelectM.SetCharacterImage(key, value);  // 스프라이트 생성
        SetBTNChImage(true,value);
    }

    void RemoveBatch(int key)
    {
        SelectM.battleInfo.Remove(key);     // 현재 유닛 고유번호를 갖고 있는 키와 밸류 삭제
        
        //SelectM.RemoveCharacterImage(key); // 스프라이트 제거
        SetBTNChImage(false,unitIndex);
    }

    void ChangeBatch(int victimKey, int newKey, int newValue)
    {
        SelectM.battleInfo.Remove(victimKey);   // 현재 유닛 고유번호를 갖고 있는 키와 밸류 삭제
        SelectM.battleInfo[newKey] = newValue;  // 현재 키값의 밸류 값을 추가하기
        
        //SelectM.ChangeImagePos(victimKey, newKey);    // 스프라이트 위치 변경
        //이걸 어케해야할까...
    }
    
    /// <summary>
    /// 음.. 이미지를 생성 삭제하는게 아니라 위치 버튼에 유닛용 이미지가 있어서 그걸 바꾸는 방식
    ///     1. 이미지를 바꾼다.
    ///     2. 이미지를 원래대로 만든다.
    /// </summary>
    /// <param name="isOn"></param>true면 캐릭터 이미지로 변경, false면 초기화
    public void SetBTNChImage(bool isOn,int unitIdx)
    {
        Color color = new Color32(255, 255, 255, 0);
        if (isOn)
        {
            CharacterData chData = GameManager.TableData.GetCharacterData(unitIdx);
            color = new Color(255,255,255,255);
            posBTNImage.GetComponent<Image>().color = color;
            posBTNImage.GetComponent<Image>().sprite = chData.FaceIconSprite;
        }
        else
        {
            color = new Color32(255, 255, 255, 0);
            posBTNImage.GetComponent<Image>().color = color;
        }
    }
}
