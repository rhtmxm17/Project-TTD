using System.Collections.Generic;
using UnityEngine;

public class HYJ_SortController : MonoBehaviour
{
    private bool isDescending = true;
    private int curSortType; //현재 정렬하는 기준

    private void Awake()
    {
        curSortType = 0;
    }

    /// <summary>
    /// 리스트 정렬하기
    /// </summary>
    /// <param name="unitInfos"></param>
    /// <param name="sortType">0:레벨, 1:전투력</param>
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
                            (unitInfos[j], unitInfos[j + 1]) = (unitInfos[j + 1], unitInfos[j]);
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
                            (unitInfos[j], unitInfos[j + 1]) = (unitInfos[j + 1], unitInfos[j]);
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
                            (unitInfos[j], unitInfos[j + 1]) = (unitInfos[j + 1], unitInfos[j]);
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
                            (unitInfos[j], unitInfos[j + 1]) = (unitInfos[j + 1], unitInfos[j]);
                        }
                    }
                }
            }
        }
        
        UpdateList(unitInfos); 
    }

    /// <summary>
    /// 정렬방식을 변경하고 변경된 정렬방식으로 리스트를 정렬 
    /// </summary>
    /// <param name="unitInfos"></param>
    public void ChangeSort(List<GameObject> unitInfos)
    {
        isDescending = !isDescending;
        SortList(unitInfos,curSortType);
    }

    /// <summary>
    /// 입력한 리스트로 업데이트
    /// </summary>
    /// <param name="unitInfos"></param>
    private void UpdateList(List<GameObject> unitInfos)
    {
        for (int i = 0; i < unitInfos.Count; i++)
        {
            unitInfos[i].transform.SetSiblingIndex(i);
        }
    }
}
