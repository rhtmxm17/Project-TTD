using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicSkillButton : MonoBehaviour
{
    [SerializeField]
    Image cooldownImg;
    [SerializeField]
    Button skillButton;

    [SerializeField]
    TextMeshProUGUI nonTargetText;
    [SerializeField]
    TextMeshProUGUI levelText;

    [field: SerializeField]
    public Image iconImg {  get; private set; }

    Coroutine autoCoroutine = null;
    WaitForSeconds autoDelay = new WaitForSeconds(0.1f);
    Func<bool> isTargetExistFunc;

    public bool IsInAuto { 
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

    Coroutine textCoroutine = null;
    WaitForSeconds displayTime = new WaitForSeconds(0.5f);

    public bool Interactable => skillButton.interactable;

    Coroutine skillCooldownCoroutine = null;
    float waitedCooltime = 0;

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
        cooldownImg.fillAmount = 1;
    }

    public void DisplayNonTargetText()
    {
        if (textCoroutine != null)
        { 
            StopCoroutine(textCoroutine);
        }
        textCoroutine = StartCoroutine(TextDisplayCO());
    }

    public void InitTargetingFunc(Func<bool> isTargetExistFunc)
    {
        this.isTargetExistFunc = isTargetExistFunc;
    }

    public void SetLevel(int level)
    { 
        levelText.text = $"Lv{level.ToString()}";
    }

    IEnumerator TextDisplayCO()
    {
        nonTargetText.gameObject.SetActive(true);
        yield return displayTime;
        nonTargetText.gameObject.SetActive(false);
        textCoroutine = null;
    }

    public void StartCoolDown(float coolTime)
    {
        if (skillCooldownCoroutine == null)
        {
            skillCooldownCoroutine = StartCoroutine(StartCoolDownCO(coolTime));
        }
    }

    public IEnumerator StartCoolDownCO(float coolTime)
    {
        waitedCooltime = 0;
        skillButton.interactable = false;
        yield return null;

        while (waitedCooltime < coolTime)
        {
            cooldownImg.fillAmount = 1 - (waitedCooltime / coolTime);
            waitedCooltime += Time.deltaTime;

            yield return null;
        }

        cooldownImg.fillAmount = 0;

        skillButton.interactable = true;
        skillCooldownCoroutine = null;
    }

    public void DecreseCooltime(float amount)
    {

        if (skillCooldownCoroutine != null)
        {
            waitedCooltime += amount;
        }

    }

    IEnumerator AutoModeCO()
    {
        yield return null;

        while (true)
        {
            if (Interactable && !StageManager.Instance.IsInChangeWave && isTargetExistFunc.Invoke())
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
