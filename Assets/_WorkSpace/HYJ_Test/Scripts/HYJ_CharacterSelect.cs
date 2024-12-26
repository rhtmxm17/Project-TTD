using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] GameObject CantPosUI; // 선택 불가 팝업
    [SerializeField] int posNum; // 위치 번호
    [Header("유닛 버튼 설정")]
    [SerializeField] int unitIndex; // 유닛 번호
    [SerializeField] GameObject UnitChangeUI; // 유닛 변경 확인 팝업



    public void InitData(HYJ_SelectManager manager, int unitIdx, GameObject unitChangeUI)
    { 
        SelectM = manager;
        GetComponentInChildren<TextMeshProUGUI>().text = $"{unitIdx.ToString()}번 유닛";
        unitIndex = unitIdx;
        UnitChangeUI = unitChangeUI;
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
        //
        if (CheckUnit(unitIndex))
        {
            // 유닛이 이미 선택되어 있다면
            SelectM.curUnitIndex = unitIndex;
            UnitChangeUI.SetActive(true);
        }
        else
        {
            // 유닛이 선택이 되어 있지 않다면
            if (CheckPos(SelectM.curPos)) // 현재 위치가 이미 키 값으로 저장이 되어 있다면
            {
                // 키 값을 가지고 있는 값을 제거
                //SelectM.battleInfo.Remove(SelectM.curPos);
                RemoveBatch(SelectM.curPos);
            }
            //SelectM.battleInfo.Add(SelectM.curPos, unitIndex);
            AddBatch(SelectM.curPos, unitIndex);
        }
    }

    public void ChangeUnit()
    {
        int unitPos = SelectM.battleInfo.FirstOrDefault(x => x.Value == SelectM.curUnitIndex).Key; // 딕셔너리 밸류값(유닛 고유번호)를 갖고 있는 키 값을 찾기
/*        SelectM.battleInfo.Remove(unitPos);
        SelectM.battleInfo[SelectM.curPos] = SelectM.curUnitIndex;*/
        ChangeBatch(unitPos, SelectM.curPos, SelectM.curUnitIndex);
    }

    public bool CheckUnit(int unitIndex)
    {
        if (SelectM.battleInfo.ContainsValue(unitIndex))
        {
            return true;
        }

        return false;
    }

    public void ReleaseUnit()
    {
        // 배치된 유닛을 해제하기
        //SelectM.battleInfo.Remove(SelectM.curPos);     // 현재 유닛 고유번호를 갖고 있는 키 값을 삭제
        RemoveBatch(SelectM.curPos);
    }

    public void ChangeColorBTN()
    {
        // TODO : 선택되어 있는 버튼은 색을 변경해주기
        Image btnImage = transform.gameObject.GetComponent<Image>();
        btnImage.color = Color.red;
    }

    void AddBatch(int key, int value)
    {
        SelectM.battleInfo.Add(key, value);
        SelectM.SetCharcterSprite(key, value);
        Debug.Log(key + "색 추가");
        //캐릭터 생성
    }

    void RemoveBatch(int key)
    {
        SelectM.battleInfo.Remove(key);     // 현재 유닛 고유번호를 갖고 있는 키 값을 삭제
        SelectM.RemoveCharacterSprite(key);
        Debug.Log(key + "색 제거");
        //캐릭터 제거
    }

    void ChangeBatch(int victimKey, int newKey, int newValue)
    {
        SelectM.battleInfo.Remove(victimKey);     // 현재 유닛 고유번호를 갖고 있는 키 값을 삭제
        SelectM.battleInfo[newKey] = newValue;
        SelectM.ChangeTo(victimKey, newKey);
    }

}
