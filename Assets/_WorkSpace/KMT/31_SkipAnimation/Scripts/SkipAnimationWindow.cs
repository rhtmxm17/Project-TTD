using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkipAnimationWindow : OpenableWindow
{
    [SerializeField]
    TextMeshProUGUI headerText;
    [SerializeField]
    Image skipImg;
    [SerializeField]
    Slider progressSlider;
    [SerializeField]
    float closeDelay;

    [Header("randomize sprites")]
    [SerializeField]
    List<Sprite> skipSprites;

    WaitForSeconds closeDelayT;
    Coroutine waitingCoroutine = null;
    Action finishedCallback = null;

    string waiting = "소탕중..";
    string complete = "완료!";

    private void Awake()
    {
        closeDelayT = new WaitForSeconds(closeDelay);
    }

    public void SetAndRunSkipAnimation(float waitTime, Action onFinishedCallback)
    {
        if (skipSprites.Count <= 0)
        {
            Debug.Log("랜덤지정 스프라이트가 존재하지 않음.");
            return;
        }
        Sprite skipSprite = skipSprites[UnityEngine.Random.Range(0, skipSprites.Count)];

        SetAndRunSkipAnimation(skipSprite, waitTime, onFinishedCallback);
    }

    public void SetAndRunSkipAnimation(Sprite skipSprite, float waitTime, Action onFinishedCallback)
    {
        skipImg.sprite = skipSprite;

        if (waitingCoroutine != null)
        {
            Debug.Log("이미 진행중.");
            return;
        }

        headerText.text = waiting;
        progressSlider.value = 0;
        finishedCallback = onFinishedCallback;

        OpenWindow();
        waitingCoroutine = StartCoroutine(WaitingTimerCO(waitTime));
    }

    public void OnBackBoarderClicked()
    {
        if (waitingCoroutine != null)
        {//로딩중 클릭
            WaitingFinish();
        }
    }

    void WaitingFinish()
    {
        progressSlider.value = 1;
        headerText.text = complete;

        if (waitingCoroutine != null)
        {
            StopCoroutine(waitingCoroutine);
            waitingCoroutine = null;
        }

        StartCoroutine(CloseDelayCO());

    }

    IEnumerator WaitingTimerCO(float waitTime)
    {
        yield return null;

        float curTime = 0;

        while (curTime < waitTime)
        {
            curTime += Time.deltaTime;
            progressSlider.value = curTime / waitTime;
            yield return null;
        }

        WaitingFinish();

    }

    IEnumerator CloseDelayCO()
    {
        yield return closeDelayT;
        finishedCallback?.Invoke();
        CloseWindow();
    }

}
