using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HYJ_CharacterSelect : MonoBehaviour
{
    // 캐릭터 (9종)
    // 캐릭터 데이터 구조 : index(ID)
    [Header("공용 설정")]
    [SerializeField] HYJ_SelectManager SelectM; // 캐릭터 선택 정보

    [Header("위치 버튼 설정")]
    [SerializeField] GameObject CharacterPanel; // 캐릭터 선택 창 
    [SerializeField] GameObject CantPosUI; // 선택 불가 팝업 -> 5개 유닛이 이미 다 배치 되었을 때의 팝업
    [SerializeField] int posNum; // 위치 번호

    [Header("유닛 버튼 설정")]
    [SerializeField] int unitIndex; // 유닛 번호
    [SerializeField] Image characterImage; // 캐릭터 이미지
    [SerializeField] TMP_Text levelText; // 캐릭터 레벨 텍스트
    [SerializeField] TMP_Text raceText; //캐릭터 종족 텍스트
    [SerializeField] TMP_Text classText; // 캐릭터 역할 텍스트(탱커/딜러/힐러)
    [SerializeField] TMP_Text attacktypeText; // 캐릭터 공격 타입 텍스트 (단일/광역)
    [SerializeField] GameObject UnitChangeUI; // 유닛 변경 확인 팝업 -> 변경하시겠습니까?

    public void InitData(HYJ_SelectManager manager, int unitIdx, GameObject unitChangeUI)
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
        else if (CheckPos(posNum)) // 위치 리스트가 5개 초과 & 이미 선택한 위치일 경우 
        {
            SelectM.curPos = posNum;
            CharacterPanel.SetActive(true);
        }

        else if(SelectM.battleInfo.Count < 5 && !CheckPos(posNum)) // 위치 리스트가 5개 보다 적고
        {
            SelectM.curPos = posNum;
            CharacterPanel.SetActive(true);
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
        // 유닛을 선택하는 버튼을 누를 때
        if (CheckUnit(unitIndex))
        {
            // 유닛이 이미 선택되어 있다면
            if (CheckUnitPos(unitIndex))
            {
                Debug.Log("이미 이 위치에 해당 유닛이 배치되어 있습니다.");
            }
            else
            {
                // TODO : 조건 추가?
                SelectM.curUnitIndex = unitIndex;
                UnitChangeUI.SetActive(true);
            }
        }

        else if (!CheckUnit(unitIndex))
        {
            // 유닛이 선택이 되어 있지 않다면
            if (CheckPos(SelectM.curPos)) // 현재 위치가 이미 키 값으로 저장이 되어 있다면
            {
                // 키 값을 가지고 있는 값을 제거
                RemoveBatch(SelectM.curPos);
            }
            AddBatch(SelectM.curPos, unitIndex);
        }
    }

    public void ChangeUnit()
    {
        // TODO : 조건 추가?
        int unitPos = SelectM.battleInfo.FirstOrDefault(x => x.Value == SelectM.curUnitIndex).Key; // 딕셔너리 밸류값(유닛 고유번호)를 갖고 있는 키 값을 찾기
        ChangeBatch(unitPos, SelectM.curPos, SelectM.curUnitIndex);
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
        unitIndex = SelectM.battleInfo.FirstOrDefault(x => x.Value == SelectM.curUnitIndex).Key; // 딕셔너리 밸류값(유닛 고유번호)를 갖고 있는 키 값을 찾기
        // 이미 해당 위치에 
        if(unitIndex == SelectM.curPos)
        {
            return true;
        }
        return false;
    }

    public void ReleaseUnit()
    {
        // 배치된 유닛을 해제하기
        if (CheckPos(SelectM.curPos)) // 선택한 키 값이 이미 등록되어 있을 때
        {
            RemoveBatch(SelectM.curPos);
        }
    }

    void AddBatch(int key, int value)
    {
        SelectM.battleInfo.Add(key, value);     // 현재 키 / 밸류 딕셔너리에 추가
        SelectM.SetCharacterImage(key, value);  // 스프라이트 생성
        //Debug.Log(key + "색 추가");
    }

    void RemoveBatch(int key)
    {
        SelectM.battleInfo.Remove(key);     // 현재 유닛 고유번호를 갖고 있는 키와 밸류 삭제
        SelectM.RemoveCharacterImage(key); // 스프라이트 제거
        //Debug.Log(key + "색 제거");
    }

    void ChangeBatch(int victimKey, int newKey, int newValue)
    {
        SelectM.battleInfo.Remove(victimKey);   // 현재 유닛 고유번호를 갖고 있는 키와 밸류 삭제
        SelectM.battleInfo[newKey] = newValue;      
        SelectM.ChangeImagePos(victimKey, newKey);    // 스프라이트 위치 변경
    }

    
}
