using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HYJ_FilterBtnColor : MonoBehaviour
{
    private Color onColor = Color.white; // 필터가 켜져 있을때 색상
    private Color offColor = Color.gray;
    [SerializeField] List<Image> elementImages;
    
    
    public void FilterBtnColorOn(List<bool> fList)
    {
        for (int i = 0; i < elementImages.Count; i++)
        {
            if (fList[i])
            {
                elementImages[i].color = onColor;
            }
            else
            {
                elementImages[i].color = offColor;
            }
        }
    }
}
