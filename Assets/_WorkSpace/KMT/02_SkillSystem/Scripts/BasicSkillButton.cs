using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicSkillButton : MonoBehaviour
{
    [SerializeField]
    Image cooldownImg;
    [SerializeField]
    Button skillButton;

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

        skillButton.enabled = false;
        cooldownImg.fillAmount = 1;
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
        skillButton.enabled = false;
        yield return null;

        while (waitedCooltime < coolTime)
        {
            cooldownImg.fillAmount = 1 - (waitedCooltime / coolTime);
            waitedCooltime += Time.deltaTime;

            yield return null;
        }

        cooldownImg.fillAmount = 0;

        skillButton.enabled = true;
        skillCooldownCoroutine = null;
    }

    public void DecreseCooltime(float amount)
    {

        if (skillCooldownCoroutine != null)
        {
            waitedCooltime += amount;
        }

    }
}
