using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Windows.Input;

public class SafeArea : MonoBehaviour
{
    private Vector2 maxAnchor;
    private Vector2 minAnchor;

    private void OnEnable()
    {
        SetSafeArea();
    }
    
    private void SetSafeArea()
    {
        var Myrect = GetComponent<RectTransform>();

        Rect useableRect = Screen.safeArea;
        //float safeAreaRatio = Screen.safeArea.width / Screen.safeArea.height;

        //if (safeAreaRatio < 4f / 3f)
        //{
        //    // 화면비 4 : 3보다 위아래로 길다면 자르기
        //    useableRect.height = Screen.safeArea.width * (4f / 3f);
        //}
        //else if (safeAreaRatio > 21f / 9f)
        //{
        //    // 화면비 21 : 9보다 좌우로 길다면 자르기
        //    useableRect.width = Screen.safeArea.height * (21f / 9f);
        //}
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