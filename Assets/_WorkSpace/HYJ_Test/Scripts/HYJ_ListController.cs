using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_ListController : MonoBehaviour
{
    [SerializeField] private List<int> defaultUnitList;
    //[SerializeField] private List<int> changedUnitList;
    private List<int> unitInfo = new List<int>();
    [SerializeField] public Dictionary<int,List<int>> unitInfoDict = new Dictionary<int, List<int>>();  
    [SerializeField] private HYJ_SelectManager selectM;


    public void AddListUnit(int unitIdx, int unitLevel, int unitPower)
    {
        
    }

    public void FilterList()
    {
        
    }

    public void SortList()
    {
        
    }
}
