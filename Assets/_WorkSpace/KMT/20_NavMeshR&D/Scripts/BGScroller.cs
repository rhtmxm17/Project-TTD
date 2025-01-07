using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGScroller : MonoBehaviour
{

    [SerializeField]
    Transform scrollerParentCanvas;

    ScrollableBG scrollableBG;

    Coroutine scrollCoroutine = null;

    public void SetScroller(ScrollableBG scrollableBGPrefab)
    {
        scrollableBG = Instantiate(scrollableBGPrefab);
    }

    public void StartScroll()
    {
        if (scrollCoroutine == null)
        {
           scrollCoroutine = StartCoroutine(scrollableBG.ScrollCO());
        }
    }

    public void StopScroll()
    {

        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
            scrollCoroutine = null;
        }

    }


}
