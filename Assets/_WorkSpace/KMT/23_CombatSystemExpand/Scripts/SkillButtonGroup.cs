using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SkillButtonGroup : MonoBehaviour
{
    [SerializeField]
    Button toggleButton;

    bool isActive = false;
    public event Action hideAllGroups = null;

    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        toggleButton.onClick.AddListener(OnClicked);
        canvas.sortingOrder = 0;
    }

    public void SetCanvasOrder(int orderCount)
    {
        canvas.sortingOrder = orderCount;
    }

    public void OnClicked()
    {
        if (isActive)
        {
            isActive = false;
            canvas.sortingOrder = 0;
        }
        else 
        {
            hideAllGroups?.Invoke();
            isActive = true;
            canvas.sortingOrder = 2;
        }
    }

    public void HideButtonGroup()
    {
        isActive = false;
        canvas.sortingOrder = 0;
    }

}
