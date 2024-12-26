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
    [HideInInspector] public TextMeshProUGUI _coinText;
    [HideInInspector] public TextMeshProUGUI _jewelryText;
    [HideInInspector] public TextMeshProUGUI _enhanceText;
    
    [HideInInspector] public Image _characterImage;

    [HideInInspector] public Button _levelUpButton;
    [HideInInspector] public Button _enhanceButton;

    private CharacterInfoController _controller;
    
    private Button _detailTabButton;
    private Button _enhanceTabButton;
    private Button _evolutionTabButton;
    private Button _meanTabButton; 
     
    private Button _exitButton;
    private GameObject _infoPopup;
     
    protected override void Awake()
    {
        base.Awake();
        Init();
        UIBind();
        ButtonAddListener(); 
    }

    private void Init()
    {
        _controller = transform.GetComponentInParent<CharacterInfoController>();
    }
    
    private void UIBind()
    {
        _infoPopup = GetUI("InfoPopup");
        
        _levelText = GetUI<TextMeshProUGUI>("LevelText");
        _nameText = GetUI<TextMeshProUGUI>("NameText");
        _atkText = GetUI<TextMeshProUGUI>("AtkText");
        _hpText = GetUI<TextMeshProUGUI>("HpText");
        _coinText = GetUI<TextMeshProUGUI>("CoinText");
        _jewelryText = GetUI<TextMeshProUGUI>("JewelryText");
        _enhanceText = GetUI<TextMeshProUGUI>("EnhanceText");
        
        _exitButton = GetUI<Button>("ExitButton");
        _levelUpButton = GetUI<Button>("LevelUpButton");
        _enhanceButton = GetUI<Button>("EnhanceButton");
        
        //좌측 Tab 버튼 바인딩
        _detailTabButton = GetUI<Button>("DetailTabButton");
        _enhanceTabButton = GetUI<Button>("EnhanceTabButton");
        _evolutionTabButton = GetUI<Button>("EvolutionTabButton");
        _meanTabButton = GetUI<Button>("MeanTabButton");
         
        _characterImage = GetUI<Image>("CharacterImage");
        
    }

    private void ButtonAddListener()
    {
        _exitButton.onClick.AddListener(() => _infoPopup.SetActive(false));
        _detailTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.DETAIL);
        _enhanceTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.ENHANCE);
        _evolutionTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.EVOLUTION);
        _meanTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.MEAN);
    }
}