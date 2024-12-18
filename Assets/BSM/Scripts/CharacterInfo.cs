using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : BaseUI
{
    private Button _characterInfoBtn;
    private Button _backBtn; 
    private GameObject _infoPopup;
    
    
    protected override void Awake()
    {
        base.Awake();
        InfoBind();
        BtnAddListener();
 
    }

    private void InfoBind()
    {
        _characterInfoBtn = GetUI<Button>("CharacterBtn");
        _backBtn = GetUI<Button>("BackBtn");
        _infoPopup = GetUI("InfoPopup");
    }

    private void BtnAddListener()
    {
        _backBtn.onClick.AddListener(()=> _infoPopup.SetActive(false));
        _characterInfoBtn.onClick.AddListener(()=> _infoPopup.SetActive(true));
    }
    
    
    
    
}
