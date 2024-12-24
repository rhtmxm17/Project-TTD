using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondSkillButton : MonoBehaviour
{
    [SerializeField]
    Image cooldownImg;
    [SerializeField]
    Button skillButton;

    [SerializeField]
    TextMeshProUGUI costText;

    Coroutine skillCostCoroutine = null;

    float cost = float.MaxValue;

    public bool Interactable => skillButton.interactable;

    private void Awake()
    {
        skillButton.interactable = false;
    }

    private void Start()
    {
        skillCostCoroutine = StartCoroutine(SkillCostCO());
    }

    IEnumerator SkillCostCO()
    {
        while (true)
        {
            cooldownImg.fillAmount = 1 - Math.Clamp(StageManager.Instance.PartyCost / cost, 0, 1);
            if (cooldownImg.fillAmount <= 0.02f)
                skillButton.interactable = true;
            else
                skillButton.interactable = false;

            yield return null;
        }
    }


    public void SetSkillCost(float cost)
    { 
        this.cost = cost;
        costText.text = cost.ToString();
    }

    public void OffSkillButton(Combatable obj)
    {
        StopCoroutine(skillCostCoroutine);
        skillButton.enabled = false;
        cooldownImg.fillAmount = 1;
    }

}
