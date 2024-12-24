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
        skillButton.enabled = false;
        yield return null;

        float time = 0;
        while (time < coolTime)
        {
            cooldownImg.fillAmount = 1 - (time / coolTime);
            time += Time.deltaTime;

            yield return null;
        }

        skillButton.enabled = true;
        skillCooldownCoroutine = null;
    }
}
