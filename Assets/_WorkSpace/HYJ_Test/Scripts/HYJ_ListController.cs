using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_ListController : MonoBehaviour
{
    [SerializeField] private List<int> defaultUnitList;
    //[SerializeField] private List<int> changedUnitList;
    private List<int> unitInfo = new List<int>();
    [SerializeField] private List<HYJ_UnitInfo> UnitInfos;

    public void InitUnitsList()
    {
        // TODO : 게임 시작 시, 초기화
        
    }

    public void FilterRoleTypeList(RoleType unitRoleType, bool isOn)
    {
        if (isOn)
        {
            isOn = false;
            
        }
        else
        {
            isOn = true;
            
        }
    }

    public void FilterElementTypeList(ElementType unitElementType, bool isOn)
    {
        if (isOn) // 이미 
        {
            isOn = false;
            
        }
        else
        {
            isOn = true;
            
        }
    }
    
    public void SortList()
    {
        
    }
}
