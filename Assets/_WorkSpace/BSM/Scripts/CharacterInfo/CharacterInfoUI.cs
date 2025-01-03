using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoUI : BaseUI
{
    //DetailTab
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
    
    //EnhanceTab
    [HideInInspector] public TextMeshProUGUI _curEnhanceLevelText;
    [HideInInspector] public TextMeshProUGUI _beforeUpGradeText;
    [HideInInspector] public TextMeshProUGUI _beforeHpText;
    [HideInInspector] public TextMeshProUGUI _beforeDefText;
    [HideInInspector] public TextMeshProUGUI _beforeAtkText;
    [HideInInspector] public TextMeshProUGUI _afterUpGradeText;
    [HideInInspector] public TextMeshProUGUI _afterHpText;
    [HideInInspector] public TextMeshProUGUI _afterAtkText;
    [HideInInspector] public TextMeshProUGUI _afterDefText;
    [HideInInspector] public TextMeshProUGUI _mileageValueText;
    [HideInInspector] public Slider _mileageSlider;

    [HideInInspector] public GameObject _enhanceResultPopup;
    [HideInInspector] public TextMeshProUGUI _enhanceResultText;
    [HideInInspector] public Button _enhanceResultConfirm;
    [HideInInspector] public Button _enhanceButton;
    
    //EvolutionTab
    
    //MeanTab
     
    private CharacterInfoController _controller;
    
    private Button _detailTabButton;
    private Button _enhanceTabButton;
    private Button _evolutionTabButton;
     
    private Button _exitButton;
    private GameObject _infoPopup;

    public TextMeshProUGUI _tempElemetTypeText;
    public TextMeshProUGUI _tempRoleTypeText;
    public TextMeshProUGUI _tempDragonVeinTypeText;
    
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
        _tempRoleTypeText = GetUI<TextMeshProUGUI>("RoleTypeText");
        _tempDragonVeinTypeText = GetUI<TextMeshProUGUI>("DragonVeinTypeText");
        DetailTabUI();
        EnhanceTabUI();
        CommonUI();
    }

    /// <summary>
    /// 공통적으로 쓰이는 UI
    /// </summary>
    private void CommonUI()
    {
        _infoPopup = GetUI("InfoPopup");  
        _enhanceText = GetUI<TextMeshProUGUI>("EnhanceText"); 
        _exitButton = GetUI<Button>("ExitButton");
 
        //좌측 Tab 버튼 바인딩
        _detailTabButton = GetUI<Button>("DetailTabButton");
        _enhanceTabButton = GetUI<Button>("EnhanceTabButton");
        _evolutionTabButton = GetUI<Button>("EvolutionTabButton");
    }
    
    /// <summary>
    /// EnhanceTab UI
    /// </summary>
    private void EnhanceTabUI()
    {
        _curEnhanceLevelText = GetUI<TextMeshProUGUI>("CurEnhanceLevelText");
        _beforeUpGradeText = GetUI<TextMeshProUGUI>("BeforeUpGradeText");
        _beforeHpText = GetUI<TextMeshProUGUI>("BeforeHpText");
        _beforeAtkText = GetUI<TextMeshProUGUI>("BeforeAtkText");
        _beforeDefText = GetUI<TextMeshProUGUI>("BeforeDefText");
        _afterUpGradeText = GetUI<TextMeshProUGUI>("AfterUpGradeText");
        _afterHpText = GetUI<TextMeshProUGUI>("AfterHpText");
        _afterAtkText = GetUI<TextMeshProUGUI>("AfterAtkText");
        _afterDefText = GetUI<TextMeshProUGUI>("AfterDefText");
        _enhanceResultText = GetUI<TextMeshProUGUI>("EnhanceResultText");
        _mileageValueText = GetUI<TextMeshProUGUI>("MileageValueText");
        
        _mileageSlider = GetUI<Slider>("MileageSlider");
        _enhanceButton = GetUI<Button>("EnhanceButton"); 
        _enhanceResultConfirm = GetUI<Button>("EnhanceResultConfirm");
        
        _enhanceResultPopup = GetUI("EnhanceResultPopup");
    }
    
    /// <summary>
    /// DetailTab UI
    /// </summary>
    private void DetailTabUI()
    {
        _coinText = GetUI<TextMeshProUGUI>("CoinText");
        _jewelryText = GetUI<TextMeshProUGUI>("JewelryText");
        _levelText = GetUI<TextMeshProUGUI>("LevelText");
        _nameText = GetUI<TextMeshProUGUI>("NameText");
        _powerLevelText = GetUI<TextMeshProUGUI>("PowerLevelText");
        _atkText = GetUI<TextMeshProUGUI>("AtkText");
        _hpText = GetUI<TextMeshProUGUI>("HpText");
        _defText = GetUI<TextMeshProUGUI>("DefText");
        _skillADescText = GetUI<TextMeshProUGUI>("SkillDescAText");
        _skillBDescText = GetUI<TextMeshProUGUI>("SkillDescBText");
        
        _skillAIconImage = GetUI<Image>("SkillIconA");
        _skillBIconImage = GetUI<Image>("SkillIconB");
        _characterImage = GetUI<Image>("CharacterImage");
        
        _levelUpButton = GetUI<Button>("LevelUpButton");
    }
     
    private void ButtonAddListener()
    {
        _exitButton.onClick.AddListener(InfoPopupClose);
        _detailTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.DETAIL);
        _enhanceTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.ENHANCE);
        _evolutionTabButton.onClick.AddListener(() => _controller.CurInfoTabType = InfoTabType.EVOLUTION);
        
        _enhanceResultConfirm.onClick.AddListener(()=> _enhanceResultPopup.SetActive(false));
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