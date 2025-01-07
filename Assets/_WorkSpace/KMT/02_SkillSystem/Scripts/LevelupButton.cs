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

    [SerializeField]
    Image levelBgPanel;
    [SerializeField]
    TextMeshProUGUI levelText;

    [Header("Text BG Color")]
    [SerializeField]
    Color defaultColor;
    [SerializeField]
    Color ableColor;
    [SerializeField]
    Color disableColor;

    int cost = 100;

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
            if (cost == int.MaxValue)
            {
                skillButton.interactable = false;
                levelBgPanel.color = defaultColor;
                yield break;
            }

            if (StageManager.Instance.PartyCost > cost)
            {
                skillButton.interactable = true;
                levelBgPanel.color = ableColor;
            }
            else
            {
                skillButton.interactable = false;
                levelBgPanel.color = disableColor;
            }

            yield return null;
        }
    }

    public void SetLevel(int level)
    {
        levelText.text = $"Lv{level.ToString()}";
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
