using System;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_SortController : MonoBehaviour
{
    private bool isDescending = true;
    private int curSortType;

    private void Awake()
    {
        curSortType = 0;
    }

    /// <summary>
    /// 필터된 
    /// </summary>
    /// <param name="sortType"></param> 정렬 기준 / 0:레벨, 1:전투력   
    /// <param name="Descending"></param> 정렬 방식 / true = 내림차순, false = 오름차순
    public void SortList(List<GameObject> unitInfos,int sortType)
    {
        curSortType = sortType;
        if (curSortType == 0) // 레벨 기준 정렬
        {
            if (isDescending) //내림차순
            {
                for (int i = unitInfos.Count-1; i >0 ; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (unitInfos[j].GetComponent<HYJ_UnitInfo>().unitLevel < unitInfos[j+1].GetComponent<HYJ_UnitInfo>().unitLevel) // j번째가 j+1번째보다 작을 때 아래로간다?
                        {
                            //Debug.Log(unitInfos[j].GetComponent<HYJ_UnitInfo>().unitLevel+ " 바뀜"+unitInfos[j+1].GetComponent<HYJ_UnitInfo>().unitLevel);
                            GameObject temp = unitInfos[j];
                            unitInfos[j] = unitInfos[j+1];
                            unitInfos[j+1] = temp;
                        }
                        
                    }
                    Debug.Log(unitInfos[i].GetComponent<HYJ_UnitInfo>().unitLevel+"정렬 됨");
                }
                
            }
            else //오름차순
            {
                for (int i = unitInfos.Count-1; i >0 ; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (unitInfos[j].GetComponent<HYJ_UnitInfo>().unitLevel > unitInfos[j+1].GetComponent<HYJ_UnitInfo>().unitLevel)
                        {
                            GameObject temp = unitInfos[j];
                            unitInfos[j] = unitInfos[j+1];
                            unitInfos[j+1] = temp;
                            
                        }
                    }
                }
            }
        }
        else if (curSortType == 1) // 전투력 기준 정렬
        {
            if (isDescending)
            {
                for (int i = unitInfos.Count - 1; i > 0; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (unitInfos[j].GetComponent<HYJ_UnitInfo>().unitPower < unitInfos[j+1].GetComponent<HYJ_UnitInfo>().unitPower)
                        {
                            GameObject temp = unitInfos[j];
                            unitInfos[j] = unitInfos[j+1];
                            unitInfos[j+1] = temp;
                        }
                    }
                }
            }
            else
            {
                for (int i = unitInfos.Count - 1; i > 0; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (unitInfos[j].GetComponent<HYJ_UnitInfo>().unitPower > unitInfos[j+1].GetComponent<HYJ_UnitInfo>().unitPower)
                        {
                            GameObject temp = unitInfos[j];
                            unitInfos[j] = unitInfos[j+1];
                            unitInfos[j+1] = temp;
                        }
                    }
                }
            }
        }
        
        UpdateList(unitInfos);
    }

    public void ChangeSort(List<GameObject> unitInfos)
    {
        isDescending = !isDescending;
        SortList(unitInfos,curSortType);
    }

    private void UpdateList(List<GameObject> unitInfos)
    {
        for (int i = 0; i < unitInfos.Count; i++)
        {
            unitInfos[i].transform.SetSiblingIndex(i);
        }
    }
}
