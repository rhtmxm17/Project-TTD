using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGScroller : MonoBehaviour
{

    [SerializeField]
    Image scrollImg;
    [SerializeField]
    float scrollSpeed;

    Coroutine scrollCoroutine = null;

    public void StartScroll()
    {
        if (scrollCoroutine == null)
        {
            scrollCoroutine = StartCoroutine(ScrollCO());
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

    IEnumerator ScrollCO()
    {
        yield return null;

        while (true)
        {
            scrollImg.material.mainTextureOffset += Vector2.right * scrollSpeed * Time.deltaTime;
            yield return null;
        }
    }

}
