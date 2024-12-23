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

    public IEnumerator StartCoolDown(float coolTime)
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
    }
}
