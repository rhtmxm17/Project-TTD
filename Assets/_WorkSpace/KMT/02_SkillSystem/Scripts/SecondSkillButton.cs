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

    [SerializeField]
    TextMeshProUGUI costText;

    [SerializeField]
    GameObject yellowImg;
    [SerializeField]
    GameObject blackImg;

    Coroutine skillCostCoroutine = null;

    float cost = float.MaxValue;
    bool levelArrived = false;

    public bool Interactable => skillButton.interactable;

    private void Awake()
    {
        skillButton.interactable = false;
        blackImg.SetActive(true);
        costText.text = "Lv3 필요";
    }

    private void Start()
    {
        skillCostCoroutine = StartCoroutine(SkillCostCO());
    }

    IEnumerator SkillCostCO()
    {
        while (true)
        {
            if (!levelArrived)
            {
                skillButton.interactable = false;
                cooldownImg.fillAmount = 1;
            }
            else
            {
                cooldownImg.fillAmount = 1 - Math.Clamp(StageManager.Instance.PartyCost / cost, 0, 1);
                if (cooldownImg.fillAmount <= 0.02f)
                {
                    skillButton.interactable = true;
                    SetEdgeImg(true);
                }
                else
                {
                    skillButton.interactable = false;
                    SetEdgeImg(false);
                }
            }

            yield return null;
        }
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


    public void SetSkillCost(float cost)
    {
        this.cost = cost;
    }

    public void ArrivedReqLevel()
    {
        levelArrived = true;
        costText.text = cost.ToString();
    }

    public void OffSkillButton(Combatable obj)
    {
        StopCoroutine(skillCostCoroutine);
        skillButton.enabled = false;
        cooldownImg.fillAmount = 1;
    }

}
