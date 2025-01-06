using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoUI : BaseUI
{
    [HideInInspector] private Sprite[] _enhanceResultIcons = new Sprite[3];
    public Sprite[] EnhanceResultIcons => _enhanceResultIcons;
    
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
    [HideInInspector] public TextMeshProUGUI _afterMaxTitleText;
    [HideInInspector] public TextMeshProUGUI _afterMaxHpText;
    [HideInInspector] public TextMeshProUGUI _afterMaxAtkText;
    [HideInInspector] public TextMeshProUGUI _afterMaxDefText;
    
    [HideInInspector] public Slider _mileageSlider;
    [HideInInspector] public GameObject _beforeMax;
    [HideInInspector] public GameObject _afterMax;
    [HideInInspector] public GameObject _enhanceResultPopup;
    [HideInInspector] public TextMeshProUGUI _enhanceResultText;
    [HideInInspector] public Image _enhanceResultIcon;
    [HideInInspector] public Button _enhanceResultConfirm;
    [HideInInspector] public Button _enhanceButton;
    
    //EvolutionTab
    
    //MeanTab
     
    private CharacterInfoController _controller;
    
    private Button _detailTabButton;
    private Button _enhanceTabButton;
    private Button _evolutionTabButton;
    private Image _detailTabColor;
    private Image _enhanceTabColor;
    private Image _evolutionTabColor;
    
    
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
        _enhanceResultIcons[0] = Resources.Load<Sprite>("Sprites/Icon/White/Icon_White_128/Icon_White_128_Emoji_Smile_02");
        _enhanceResultIcons[1] = Resources.Load<Sprite>("Sprites/Icon/White/Icon_White_128/Icon_White_128_Emoji_Sad_02");
        _enhanceResultIcons[2] = Resources.Load<Sprite>("Sprites/Icon/White/Icon_White_128/Icon_White_128_Error_Png");
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
        
        _detailTabColor = _detailTabButton.GetComponent<Image>();
        _enhanceTabColor = _enhanceTabButton.GetComponent<Image>();
        _evolutionTabColor = _evolutionTabButton.GetComponent<Image>(); 
    }
    
    /// <summary>
    /// EnhanceTab UI
    /// </summary>
    private void EnhanceTabUI()
    {
        _beforeMax = GetUI("BeforeMax");
        _afterMax = GetUI("AfterMax");
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
        _enhanceResultIcon = GetUI<Image>("EnhanceResultImage");
        _mileageValueText = GetUI<TextMeshProUGUI>("MileageValueText");
        
        _afterMaxTitleText = GetUI<TextMeshProUGUI>("AfterMaxTitleText");
        _afterMaxHpText = GetUI<TextMeshProUGUI>("AfterMaxHpText");
        _afterMaxAtkText = GetUI<TextMeshProUGUI>("AfterMaxAtkText");
        _afterMaxDefText = GetUI<TextMeshProUGUI>("AfterMaxDefText");
        
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
        _detailTabButton.onClick.AddListener(() => TabButtonClick(InfoTabType.DETAIL));
        _enhanceTabButton.onClick.AddListener(() => TabButtonClick(InfoTabType.ENHANCE));
        _evolutionTabButton.onClick.AddListener(() => TabButtonClick(InfoTabType.EVOLUTION));
        
        _enhanceResultConfirm.onClick.AddListener(()=> _enhanceResultPopup.SetActive(false));
    }

    private void TabButtonClick(InfoTabType tabType)
    {
        _controller.CurInfoTabType = tabType;
        _detailTabColor.color = _controller.CurInfoTabType == InfoTabType.DETAIL ? Color.cyan : Color.white;
        _enhanceTabColor.color = _controller.CurInfoTabType == InfoTabType.ENHANCE ? Color.cyan : Color.white;
        _evolutionTabColor.color = _controller.CurInfoTabType == InfoTabType.EVOLUTION ? Color.cyan : Color.white;
        
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