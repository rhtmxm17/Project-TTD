using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSortUI : BaseUI
{
    [HideInInspector] public Button LevelSortButton;
    [HideInInspector] public Button PowerLevelSortButton;
    [HideInInspector] public Button EnhanceLevelSortButton;
    [HideInInspector] public Button AttackPowerSortButton;
    [HideInInspector] public Button DefensePowerSortButton;
    [HideInInspector] public Button HealthPowerSortButton;
    
    
    private Button _sortCloseButton;

    
    protected override void Awake()
    {
        base.Awake();
        UIBind();
        SubscribeEvent();
    }

    private void UIBind()
    {
        _sortCloseButton = GetUI<Button>("SortCloseButton");
        LevelSortButton = GetUI<Button>("LevelSort");
        PowerLevelSortButton = GetUI<Button>("PowerLevelSort");
        EnhanceLevelSortButton = GetUI<Button>("EnhanceLevelSort");
        AttackPowerSortButton = GetUI<Button>("AttackPowerSortButton");
        DefensePowerSortButton = GetUI<Button>("DefensePowerSortButton");
        HealthPowerSortButton = GetUI<Button>("HealthPowerSortButton"); 
    }

    private void SubscribeEvent()
    {
        _sortCloseButton.onClick.AddListener(() => transform.GetChild(0).gameObject.SetActive((false)));
    }
}