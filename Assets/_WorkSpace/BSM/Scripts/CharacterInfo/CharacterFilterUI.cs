using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFilterUI : BaseUI
{
    [HideInInspector] public Button _fireFilterButton;
    
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
        _fireFilterButton = GetUI<Button>("FireFilter");
    }
    
    private void SubscribeEvent()
    {
        _filterCloseButton.onClick.AddListener(() => transform.GetChild(0).gameObject.SetActive((false)));
    }
}
