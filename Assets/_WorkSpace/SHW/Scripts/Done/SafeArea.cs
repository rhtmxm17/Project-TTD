using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Windows.Input;

public class SafeArea : MonoBehaviour
{
    private Vector2 maxAnchor;
    private Vector2 minAnchor;

    private void Start()
    {
        SetSafeArea();
    }
    
    private void SetSafeArea()
    {
        var Myrect = GetComponent<RectTransform>();

        Rect useableRect = Screen.safeArea;
        if (Screen.safeArea.width > Screen.safeArea.height * 16f / 9f)
        {
            useableRect.width = Screen.safeArea.height * 16f / 9f;
        }
        else
        {
            useableRect.height = Screen.safeArea.width * 9f / 16f;
        }
        useableRect.center = Screen.safeArea.center;

        minAnchor = useableRect.min;
        maxAnchor = useableRect.max;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;
        
        Myrect.anchorMin = minAnchor;
        Myrect.anchorMax = maxAnchor;

    }
}