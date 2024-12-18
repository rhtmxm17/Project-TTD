using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : BaseUI
{
    [SerializeField] private TestSO _test;
    
    private Image _characterSprite;
    private Button _characterInfoBtn;
    private Button _backBtn;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _atkText;
    private TextMeshProUGUI _hpText;
    
    private GameObject _infoPopup;
    
    protected override void Awake()
    {
        base.Awake();
        InfoBind();
        BtnAddListener();
        
    }

    private void OnEnable() => CharacterInfoUpdate();
    
    private void InfoBind()
    {
        _characterInfoBtn = GetUI<Button>("CharacterBtn");
        _backBtn = GetUI<Button>("BackBtn");
        _infoPopup = GetUI("InfoPopup");
        _nameText = GetUI<TextMeshProUGUI>("Name");
        _atkText = GetUI<TextMeshProUGUI>("Atk");
        _hpText = GetUI<TextMeshProUGUI>("Hp");
         
        _characterSprite = _characterInfoBtn.GetComponent<Image>();
    }

    private void BtnAddListener()
    {
        _backBtn.onClick.AddListener(()=> _infoPopup.SetActive(false));
        _characterInfoBtn.onClick.AddListener(()=> _infoPopup.SetActive(true));
    }

    private void CharacterInfoUpdate()
    {
        _characterSprite.sprite = _test.sprite;
        _nameText.text = "이름 : " + _test.name;
        _atkText.text = "공격력 : " + _test.atk;
        _hpText.text = "체력 : " + _test.hp; 
    }
    
    
}
