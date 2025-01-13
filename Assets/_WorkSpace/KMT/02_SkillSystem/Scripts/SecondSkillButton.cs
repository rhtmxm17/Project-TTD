using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondSkillButton : MonoBehaviour
{
    [SerializeField]
    Image cooldownImg;
    [SerializeField]
    Button skillButton;

    //TODO : 타겟이 없음을 알리는 텍스트 추가

    [SerializeField]
    GameObject yellowImg;
    [SerializeField]
    GameObject blackImg;

    Coroutine skillCooldownCoroutine = null;
    float waitedCooltime = 0;

    float coolTime = float.MaxValue;
    public bool LevelArrived { get; private set; } = false;

    Coroutine autoCoroutine = null;
    WaitForSeconds autoDelay = new WaitForSeconds(0.1f);
    Func<bool> isTargetExistFunc;

    Canvas buttonCanvas;

    public bool IsInAuto
    {
        get { return isAuto; }
        set
        {
            if (value == true)//오토를 켜는 경우
            {
                if (autoCoroutine != null)
                {
                    StopCoroutine(autoCoroutine);
                    autoCoroutine = null;
                }

                autoCoroutine = StartCoroutine(AutoModeCO());

            }
            else//오토를 끄는 경우
            {
                if (autoCoroutine != null)
                {
                    StopCoroutine(autoCoroutine);
                    autoCoroutine = null;
                }
            }
            isAuto = value;
        }
    }
    bool isAuto = false;

    public bool Interactable => skillButton.interactable;

    private void Awake()
    {
        skillButton.interactable = false;
        cooldownImg.fillAmount = 1;
        SetEdgeImg(false);
        buttonCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        SetEdgeImg(false);
    }

    public void InitTargetingFunc(Func<bool> isTargetExistFunc)
    {
        this.isTargetExistFunc = isTargetExistFunc;
    }

    /// <summary>
    /// 내부 쿨타임 시작 [ 스킬 사용이 확정되면 실행 ]
    /// </summary>
    public void StartCoolDown()
    {
        SetEdgeImg(false);
        if (skillCooldownCoroutine == null)
        {
            skillCooldownCoroutine = StartCoroutine(StartCoolDownCO(coolTime));
        }
    }

    IEnumerator StartCoolDownCO(float coolTime)
    {
        yield return null;

        if (!LevelArrived)
        {
            skillButton.interactable = false;
            cooldownImg.fillAmount = 1;
            yield break;
        }

        waitedCooltime = 0;
        skillButton.interactable = false;

        while (waitedCooltime < coolTime)
        {
            cooldownImg.fillAmount = 1 - (waitedCooltime / coolTime);
            waitedCooltime += Time.deltaTime;

            yield return null;
        }

        cooldownImg.fillAmount = 0;

        skillButton.interactable = true;
        SetEdgeImg(true);
        skillCooldownCoroutine = null;
    }


    public void SetEdgeImg(bool isYellow)
    {
        if (isYellow)
        {
            yellowImg.SetActive(true);
            blackImg.SetActive(false);
        }
        else
        {
            yellowImg.SetActive(false);
            blackImg.SetActive(true);
        }
    }


    public void SetSkillCooltime(float coolTime)
    {
        this.coolTime = coolTime;
    }

    public void ArrivedReqLevel()
    {
        LevelArrived = true;
        SetEdgeImg(true);
        skillButton.interactable = true;
        cooldownImg.fillAmount = 0;
        buttonCanvas.sortingOrder = 20;
    }

    public void OffSkillButton(Combatable obj)
    {
        if (skillCooldownCoroutine != null)
        {
            StopCoroutine(skillCooldownCoroutine);
            skillCooldownCoroutine = null;
        }

        if (autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
            autoCoroutine = null;
        }

        skillButton.interactable = false;
        SetEdgeImg(false);
        cooldownImg.fillAmount = 1;
    }

    IEnumerator AutoModeCO()
    {
        yield return null;

        while (true)
        {
            if (Interactable && !StageManager.Instance.IsInChangeWave && isTargetExistFunc.Invoke() && LevelArrived)
            {
                skillButton.onClick.Invoke();
            }
            else
            {
                //Debug.Log("스킬 비활성화 상태 또는 타겟이 없는 상태");
            }

            yield return autoDelay;
        }
    }

}
