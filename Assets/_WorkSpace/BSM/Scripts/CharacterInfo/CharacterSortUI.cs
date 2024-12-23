using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSortUI : BaseUI
{
    public Button NameSortButton;
    
    
    private Button _closeButton;
    
    protected override void Awake()
    {
        base.Awake();
        UIBind();
        SubscribeEvent();
    }

    private void UIBind()
    {
        _closeButton = GetUI<Button>("CloseButton");
        NameSortButton = GetUI<Button>("NameSort");
    }

    private void SubscribeEvent()
    {
        _closeButton.onClick.AddListener(() => transform.GetChild(0).gameObject.SetActive((false)));
    }
    
    
    
    
}
