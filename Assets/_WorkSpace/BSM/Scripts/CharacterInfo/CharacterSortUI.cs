using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSortUI : BaseUI
{
    [HideInInspector] public Button NameSortButton;
    [HideInInspector] public Button LevelSortButton;
    [HideInInspector] public Button PowerLevelSortButton;
    
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
        NameSortButton = GetUI<Button>("NameSort");
        LevelSortButton = GetUI<Button>("LevelSort");
        PowerLevelSortButton = GetUI<Button>("PowerLevelSort");
    }

    private void SubscribeEvent()
    {
        _sortCloseButton.onClick.AddListener(() => transform.GetChild(0).gameObject.SetActive((false)));
    }
}