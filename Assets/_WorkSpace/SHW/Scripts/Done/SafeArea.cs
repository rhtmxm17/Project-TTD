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

        minAnchor = Screen.safeArea.min;
        maxAnchor = Screen.safeArea.max;
        
        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;
        
        Myrect.anchorMin = minAnchor;
        Myrect.anchorMax = maxAnchor;
    }
}