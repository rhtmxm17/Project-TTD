using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoUI : BaseUI
{
    [HideInInspector] public TextMeshProUGUI _levelText;
    [HideInInspector] public TextMeshProUGUI _nameText;
    [HideInInspector] public TextMeshProUGUI _atkText;
    [HideInInspector] public TextMeshProUGUI _hpText;
    [HideInInspector] public TextMeshProUGUI _defText;
    [HideInInspector] public TextMeshProUGUI _powerLevelText; 
    [HideInInspector] public TextMeshProUGUI _coinText;
    [HideInInspector] public TextMeshProUGUI _jewelryText;
    [HideInInspector] public TextMeshProUGUI _enhanceText;
    [HideInInspector] public TextMeshProUGUI _skillADescText;
    [HideInInspector] public TextMeshProUGUI _skillBDescText;
     
    [HideInInspector] public Image _skillAIconImage;
    [HideInInspector] public Image _skillBIconImage;
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

    public TextMeshProUGUI _tempElemetTypeText;
    
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
        //TODO: 임시 용 타입 UI 특성 지우기
        _tempElemetTypeText = GetUI<TextMeshProUGUI>("ElementTypeText");
        
        
        _infoPopup = GetUI("InfoPopup");
        
        _levelText = GetUI<TextMeshProUGUI>("LevelText");
        _nameText = GetUI<TextMeshProUGUI>("NameText");
        _powerLevelText = GetUI<TextMeshProUGUI>("PowerLevelText");
        _atkText = GetUI<TextMeshProUGUI>("AtkText");
        _hpText = GetUI<TextMeshProUGUI>("HpText");
        _defText = GetUI<TextMeshProUGUI>("DefText");
        _coinText = GetUI<TextMeshProUGUI>("CoinText");
        _jewelryText = GetUI<TextMeshProUGUI>("JewelryText");
        _enhanceText = GetUI<TextMeshProUGUI>("EnhanceText");
        _skillADescText = GetUI<TextMeshProUGUI>("SkillDescAText");
        _skillBDescText = GetUI<TextMeshProUGUI>("SkillDescBText");
        
        _exitButton = GetUI<Button>("ExitButton");
        _levelUpButton = GetUI<Button>("LevelUpButton");
        _enhanceButton = GetUI<Button>("EnhanceButton");
        
        //좌측 Tab 버튼 바인딩
        _detailTabButton = GetUI<Button>("DetailTabButton");
        _enhanceTabButton = GetUI<Button>("EnhanceTabButton");
        _evolutionTabButton = GetUI<Button>("EvolutionTabButton");
        _meanTabButton = GetUI<Button>("MeanTabButton");

        _skillAIconImage = GetUI<Image>("SkillIconA");
        _skillBIconImage = GetUI<Image>("SkillIconB");
        _characterImage = GetUI<Image>("CharacterImage");
        
    }

    private void ButtonAddListener()
    {
        _exitButton.onClick.AddListener(InfoPopupClose);
        _detailTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.DETAIL);
        _enhanceTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.ENHANCE);
        _evolutionTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.EVOLUTION);
        _meanTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.MEAN);
    }
    
    /// <summary>
    /// 상세 팝업 종료 후 탭 타입 변경
    /// </summary>
    private void InfoPopupClose()
    {
        _infoPopup.SetActive(false);
        _controller.CurInfoTabType = InfoTabType.DETAIL;
    }
    
}