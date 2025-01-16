using System.Collections.Generic;
using UnityEngine;

public class HYJ_ListController : MonoBehaviour
{
    [SerializeField] public Transform characterSelectContent;
    [SerializeField] public List<GameObject> unitInfos;
    
    // 필터 조절용 변수
    [SerializeField] private List<bool> filterList; 
    [SerializeField] HYJ_FilterBtnColor filterBtnColor;
    private bool isLevel;
    private bool isPower;
    public void InitUnitsList()
    {
        for (int i = 0; i < 6; i++)
        {
            filterList.Add(true);
        }    
        
        for (int i = 0; i < characterSelectContent.childCount; i++)
        {
            unitInfos.Add(transform.GetChild(i).gameObject);
        }
    }

    private void FilterRoleTypeList()
    {
        
    }

    public void FilterElementTypeList(int typeNum)
    {
        switch (typeNum)
        {
            case 0:
                FilterElementTypeList2(ElementType.NONE, filterList[typeNum]);
                filterList[typeNum] = !filterList[typeNum];
                updateListAll();
                break;
            case 1:
                FilterElementTypeList2(ElementType.FIRE, filterList[typeNum]);
                filterList[typeNum] = !filterList[typeNum];
                updateListOther();
                break;
            case 2:
                FilterElementTypeList2(ElementType.WATER, filterList[typeNum]);
                filterList[typeNum] = !filterList[typeNum];
                updateListOther();
                break;
            case 3:
                FilterElementTypeList2(ElementType.WIND, filterList[typeNum]);
                filterList[typeNum] = !filterList[typeNum];
                updateListOther();
                break;
            case 4:
                FilterElementTypeList2(ElementType.EARTH, filterList[typeNum]);
                filterList[typeNum] = !filterList[typeNum];
                updateListOther();
                break;
            case 5:
                FilterElementTypeList2(ElementType.METAL, filterList[typeNum]);
                filterList[typeNum] = !filterList[typeNum];
                updateListOther();
                break;
        }
        filterBtnColor.FilterBtnColorOn(filterList);
    }

    private void FilterElementTypeList2(ElementType unitElementType, bool isType)
    {
        if (unitElementType == ElementType.NONE) //전체 활성화 or 비활성화
        {
            if (isType)
            {
                foreach (var unitInfo in unitInfos)
                {
                    unitInfo.SetActive(false);
                }
            }
            else
            {
                foreach (var unitInfo in unitInfos)
                {
                    unitInfo.SetActive(true);
                }
            }
            
        }
        else // 전체가 아닌 별개의 속성을 선택했을 때
        {
            if (isType)
            {
                foreach (GameObject unitInfo in unitInfos)
                {
                    if (unitInfo.GetComponent<HYJ_UnitInfo>().unitElementType == unitElementType)
                    {
                        unitInfo.SetActive(false);
                    }
                }
            }
            else
            {
                foreach (GameObject unitInfo in unitInfos)
                {
                    if (unitInfo.GetComponent<HYJ_UnitInfo>().unitElementType == unitElementType)
                    {
                        unitInfo.SetActive(true);
                    }
                }
            }
        }
    }

    private void updateListAll()
    {
        if (filterList[0])
        {
            for (int i = 1; i < 6; i++)
            {
                filterList[i] = true;
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                filterList[i] = false;
            }
        }
    }
    
    private void updateListOther()
    {
        int trueCondt = 0;
        for (int i = 1; i < 6; i++)
        {
            if (!filterList[i])
            {
                filterList[0] = false;
            }
            else
            {
                trueCondt++;
            }
        }

        if (trueCondt == 5)
        {
            filterList[0] = true;
        }
    }
    
    public void SortList()
    {
        
    }
}
