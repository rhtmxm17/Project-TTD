using UnityEngine;
using UnityEngine.UI;

public class HYJ_ChangeBtnColor : MonoBehaviour
{
    
    private Color onColor = Color.gray;
    private Color offColor = Color.white;
    Image btnImg;
    
    private void Awake()
    {
        btnImg = GetComponent<Image>();
    }

    /// <summary>
    /// 배치버튼 색상 변경
    /// </summary>
    /// <param name="isOn">true:배치버튼을 선택했을 때, false:배치버튼을 선택하지 않았을 때</param>
    public void ChangeBtnColor(bool isOn)
    {
        btnImg.color = isOn ? onColor : offColor;
    }
}
