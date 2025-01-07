using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelupButton : MonoBehaviour
{

    [SerializeField]
    Button skillButton;

    [SerializeField]
    TextMeshProUGUI costText;

    int cost = int.MaxValue;

    Coroutine levelupCoroutine = null;

    public bool Interactable => skillButton.interactable;

    private void Awake()
    {
        skillButton.interactable = false;
    }

    private void Start()
    {
        levelupCoroutine = StartCoroutine(LevelupCostCO());
    }

    IEnumerator LevelupCostCO()
    {
        while (true)
        {
            if (StageManager.Instance.PartyCost > cost)
            {
                skillButton.interactable = true;
            }
            else
            {
                skillButton.interactable = false;
            }

            yield return null;
        }
    }

    public void SetLevelupCost(int cost)
    {
        this.cost = cost;
        if (cost == int.MaxValue)
        {
            costText.text = "Max";
        }
        else
        { 
            costText.text = cost.ToString();
        }
    }


    public void OffSkillButton(Combatable obj)
    {
        StopCoroutine(levelupCoroutine);
        skillButton.interactable = false;
    }

}
