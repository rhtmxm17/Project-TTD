using TMPro;
using UnityEngine;

public class HYJ_SortBtn : MonoBehaviour
{
    [SerializeField] private TMP_Text btnText;
    [SerializeField] TMP_Text curSortBtnText;
    [SerializeField] HYJ_ListFilterController listFilterController;
    
    public void ChangeCurSortBtnText()
    {
        curSortBtnText.text = btnText.text;
        if (curSortBtnText.text == "Level")
        {
            listFilterController.SortListBtn(0);
        }
        else if (curSortBtnText.text == "Power")
        {
            listFilterController.SortListBtn(1);
        }
    }
}
