using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFilterUI : BaseUI
{
    [HideInInspector] public Button _fireFilterButton;
    [HideInInspector] public Button _waterFilterButton;
    [HideInInspector] public Button _grassFilterButton;
    [HideInInspector] public Button _groundFilterButton;
    [HideInInspector] public Button _electricFilterButton;
    [HideInInspector] public Button _allFilterButton;
    [HideInInspector] public Button _deffenseFilterButton;
    [HideInInspector] public Button _attackFilterButton;
    [HideInInspector] public Button _supportFilterButton;
    [HideInInspector] public Button _singleFilterButton;
    [HideInInspector] public Button _multiFilterButton;
    
    
    private Button _filterCloseButton;
    
    
    
    protected override void Awake()
    {
        base.Awake();
        UIBind();
        SubscribeEvent();
    }
    
    private void UIBind()
    {
        _filterCloseButton = GetUI<Button>("FilterCloseButton");
        
        //각 필터 버튼 Bind
        _fireFilterButton = GetUI<Button>("FireFilter");
        _waterFilterButton = GetUI<Button>("WaterFilter");
        _grassFilterButton = GetUI<Button>("GrassFilter");
        _groundFilterButton = GetUI<Button>("GroundFilter");
        _electricFilterButton = GetUI<Button>("ElectricFilter");
        _allFilterButton = GetUI<Button>("AllFilter");

        _deffenseFilterButton = GetUI<Button>("DefensiveFilter");
        _attackFilterButton = GetUI<Button>("OffensiveFilter");
        _supportFilterButton = GetUI<Button>("SupportiveFilter");
        _singleFilterButton = GetUI<Button>("SingleFilter");
        _multiFilterButton = GetUI<Button>("MultiFilter");
    }
    
    private void SubscribeEvent()
    {
        _filterCloseButton.onClick.AddListener(() => transform.GetChild(0).gameObject.SetActive((false)));
    }
}
