using System;
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
    public bool IsMaxLevel { get; private set; } = false;
    public bool IsAlive { get; private set; } = true;
    public int CurLevel { get; private set; } = -1;

    public bool IsCooldown { get; private set; } = false;

    public Button LvButton { get { return skillButton; } private set { } }

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
            if (IsMaxLevel)
            {
                skillButton.interactable = false;
                levelBgPanel.color = defaultColor;
                yield break;
            }


            if (StageManager.Instance.PartyCost > cost && !IsCooldown)
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

    public void StartCooldown() => StartCoroutine(StartCooldownCO());

    IEnumerator StartCooldownCO()
    {
        IsCooldown = true;
        yield return new WaitForSeconds(3);
        IsCooldown = false;
    }

    public void SetLevel(int level)
    {
        CurLevel = level;
        levelText.text = $"Lv{level.ToString()}";
    }

    public void SetLevelupCost(int cost)
    {
        this.cost = cost;
        if (cost == int.MaxValue)
        {
            IsMaxLevel = true;
            costText.text = "Max";
        }
        else
        { 
            costText.text = cost.ToString();
        }
    }


    public void OffSkillButtonOnDead(Combatable obj)
    {
        StopCoroutine(levelupCoroutine);
        skillButton.interactable = false;
        IsAlive = false;
    }

}
