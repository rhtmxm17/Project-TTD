using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_CharacterSelect : MonoBehaviour
{
    // 캐릭터 (9종)
    // 캐릭터 데이터 구조 : index(ID)
    [SerializeField] GameObject CharacterPanel;
    [SerializeField] int posNum;
    [SerializeField] int unitIndex;
    [SerializeField] HYJ_SelectManager SelectM;
    [SerializeField] GameObject CantPosUI;

    public void SelectPos()
    {
        // 위치를 결정하는 버튼을 눌렀을 때
        if(SelectM.posList.Count >= 5 && !PosCheck(posNum)) // 위치 리스트가 5개 초과 & 이미 선택한 위치가 아닐 경우
        {
            CantPosUI.SetActive(true);
        }
        else if (PosCheck(posNum)) // 위치 리스트가 5개 초과 & 이미 선택한 위치일 경우
        {
            CharacterPanel.SetActive(true);
            SelectUnit(posNum,unitIndex);
        }

        else if(SelectM.posList.Count < 5 && !PosCheck(posNum)) // 위치 리스트가 5개 보다 적고
        {
            CharacterPanel.SetActive(true);
            SelectUnit(posNum, unitIndex);
            
        }
    }

    public bool PosCheck(int posNum)
    {
        for(int i = 0; i < SelectM.posList.Count; i++)
        {
            if (SelectM.posList[i][0] == posNum)
            {
                return true;
            }
        }

        return false;
    }

    public void SelectUnit(int posNum,int unitIndex)
    {
        // TODO : 캐릭터 선택 창이 뜨고, 캐릭터를 선택
        // TODO : 이미 선택한 캐릭터는 선택 불가

        //SelectM.posList.Add(posNum);
    }

    public void AddUnitPos()
    {
        SelectM.posList[0].Add(posNum);
    }
}
