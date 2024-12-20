using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : BaseUI
{
    [HideInInspector] public TextMeshProUGUI _levelText;
    [HideInInspector] public TextMeshProUGUI _nameText;
    [HideInInspector] public TextMeshProUGUI _atkText;
    [HideInInspector] public TextMeshProUGUI _hpText;
    [HideInInspector] public Image _characterImage;
    [HideInInspector] public Button _levelUpButton;
    [HideInInspector] public CharacterInfo _characterInfo;
 
    private Button _exitButton;

    protected override void Awake()
    {
        base.Awake();
        UIBind();
        ButtonAddListener(); 
    }

    private void UIBind()
    {
        _levelText = GetUI<TextMeshProUGUI>("LevelText");
        _nameText = GetUI<TextMeshProUGUI>("NameText");
        _atkText = GetUI<TextMeshProUGUI>("AtkText");
        _hpText = GetUI<TextMeshProUGUI>("HpText");

        _exitButton = GetUI<Button>("ExitButton");
        _levelUpButton = GetUI<Button>("LevelUpButton");
        _characterImage = GetUI<Image>("CharacterImage");
    }

    private void ButtonAddListener()
    {
        _exitButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
}