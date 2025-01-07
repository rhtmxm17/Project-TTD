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
}
