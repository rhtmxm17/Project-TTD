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
    }
    
    private void SubscribeEvent()
    {
        _filterCloseButton.onClick.AddListener(() => transform.GetChild(0).gameObject.SetActive((false)));
    }
}
