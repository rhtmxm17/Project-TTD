using TMPro;
using UnityEngine;

public class HYJ_SortBtn : MonoBehaviour
{
    [SerializeField] private TMP_Text btnText;
    [SerializeField] TMP_Text curSortBtnText;
    [SerializeField] HYJ_ListFilterController listFilterController;
    
    /// <summary>
    /// 유닛 선택창 리스트 정렬버튼
    /// </summary>
    /// <param name="sortType">0:레벨(Level), 1:전투력(Power)</param>
    public void ChangeCurSortBtnText(int sortType)
    {
        curSortBtnText.text = btnText.text;
        if (sortType == 0)
        {
            listFilterController.SortListBtn(0);
        }
        else if (sortType == 1)
        {
            listFilterController.SortListBtn(1);
        }
    }
}
