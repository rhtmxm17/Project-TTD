using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusableTab : MonoBehaviour
{
    [SerializeField]
    Color defaultColor;
    [SerializeField]
    Color focusedColor;

    Image image = null;

    void initImage()
    {
        image = GetComponent<Image>();
    }

    public void Focus()
    { 
        if(image == null) initImage();
        image.color = focusedColor;
    }

    public void Relese()
    {
        if (image == null) initImage();
        image.color = defaultColor;
    }
}
