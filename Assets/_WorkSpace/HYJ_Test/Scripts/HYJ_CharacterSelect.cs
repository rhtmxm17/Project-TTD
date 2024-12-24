using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_CharacterSelect : MonoBehaviour
{
    // 캐릭터 (9종)
    // 캐릭터 데이터 구조 : index(ID)
    [SerializeField] GameObject CharacterPanel; // 캐릭터 선택 창
    [SerializeField] int posNum; // 위치 번호
    [SerializeField] int unitIndex; // 유닛 번호
    [SerializeField] HYJ_SelectManager SelectM; // 캐릭터 선택 정보
    [SerializeField] GameObject CantPosUI; // 선택 불가 팝업
    [SerializeField] GameObject UnitChangeUI; // 유닛 변경 확인 팝업
    
    

    public void SelectPos()
    {
        // 위치를 결정하는 버튼을 눌렀을 때
        if (SelectM.battleInfo.Count >= 5 && !PosCheck(posNum)) // 위치 리스트가 5개 초과 & 이미 선택한 위치가 아닐 경우
        {
            CantPosUI.SetActive(true);
        }
        else if (PosCheck(posNum)) // 위치 리스트가 5개 초과 & 이미 선택한 위치일 경우 
        {
            SelectM.curPos = posNum;
            CharacterPanel.SetActive(true);
        }

        else if(SelectM.battleInfo.Count < 5 && !PosCheck(posNum)) // 위치 리스트가 5개 보다 적고
        {
            SelectM.curPos = posNum;
            CharacterPanel.SetActive(true);
        }
    }

    public bool PosCheck(int posNum) // 위치가 선택되어 있는지 확인하기
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
        if (UnitCheck(unitIndex))
        {
            // 유닛이 이미 선택되어 있다면
        }
        else
        {
            // 유닛이 선택이 되어 있지 않다면
        }
    }

    public bool UnitCheck(int unitIndex)
    {
        if (SelectM.battleInfo.ContainsValue(unitIndex))
        {
            return true;
        }

        return false;
    }

    public void AddInfo()
    {
        SelectM.battleInfo.Add(SelectM.curPos,unitIndex);
    }

    public void UnitRelese()
    {
        // 배치된 유닛을 해제하기
    }

    public void UnitChange()
    {
        // 이미 배치되어 있는 유닛을 교체하기
    }
}
