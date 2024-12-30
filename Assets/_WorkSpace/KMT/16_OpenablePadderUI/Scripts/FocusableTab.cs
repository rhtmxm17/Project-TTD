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

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Focus()
    { 
        image.color = focusedColor;
    }

    public void Relese()
    { 
        image.color = defaultColor;
    }
}
