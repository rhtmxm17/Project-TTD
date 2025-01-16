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

    public void ChangeBtnColor(bool isOn)
    {
        btnImg.color = isOn ? onColor : offColor;
    }
}
