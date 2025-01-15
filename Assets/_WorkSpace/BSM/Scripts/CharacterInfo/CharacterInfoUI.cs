using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoUI : BaseUI
{
    private readonly Sprite[] _enhanceResultIcons = new Sprite[3];
    public Sprite[] EnhanceResultIcons => _enhanceResultIcons;
    
    //DetailTab
    [HideInInspector] public TextMeshProUGUI _levelText;
    [HideInInspector] public TextMeshProUGUI _nameText;
    [HideInInspector] public TextMeshProUGUI _atkText;
    [HideInInspector] public TextMeshProUGUI _hpText;
    [HideInInspector] public TextMeshProUGUI _defText;
    [HideInInspector] public TextMeshProUGUI _powerLevelText; 
    [HideInInspector] public TextMeshProUGUI _levelUpCoinText;
    [HideInInspector] public TextMeshProUGUI _levelUpYongGwaText;
    [HideInInspector] public TextMeshProUGUI _enhanceText;
    [HideInInspector] public TextMeshProUGUI _skillNormalDescText;
    [HideInInspector] public TextMeshProUGUI _skillSpecialDescText;
    [HideInInspector] public TextMeshProUGUI _beforeBonusAtkText;
    [HideInInspector] public TextMeshProUGUI _beforeBonusDefText;
    [HideInInspector] public TextMeshProUGUI _beforeBonusHpText;
    [HideInInspector] public TextMeshProUGUI _afterBonusAtkText;
    [HideInInspector] public TextMeshProUGUI _afterBonusDefText;
    [HideInInspector] public TextMeshProUGUI _afterBonusHpText;
    [HideInInspector] public TextMeshProUGUI _bonusLevelText;
    [HideInInspector] public TextMeshProUGUI _levelGoldAmountText;
    [HideInInspector] public TextMeshProUGUI _levelYongGwaAmountText;
    [HideInInspector] public TextMeshProUGUI _skillNormalTitleText;
    [HideInInspector] public TextMeshProUGUI _skillSpecialTitleText;
    
    [HideInInspector] public Image _skillAIconImage;
    [HideInInspector] public Image _skillBIconImage;
    [HideInInspector] public Image _characterImage;
    [HideInInspector] public Image _levelUpNormalEffect;
    [HideInInspector] public Image _levelUpSpecialEffect;
     
    [HideInInspector] public Button _levelUpButton;
    [HideInInspector] public GameObject _materialGroup;
    [HideInInspector] public GameObject _bonusPopup;
    [HideInInspector] public GameObject _amountGroup;
    
    private Button _bonusExitButton;
    
    //EnhanceTab
    [HideInInspector] public TextMeshProUGUI _beforeUpGradeText;
    [HideInInspector] public TextMeshProUGUI _beforeHpText;
    [HideInInspector] public TextMeshProUGUI _beforeDefText;
    [HideInInspector] public TextMeshProUGUI _beforeAtkText;
    [HideInInspector] public TextMeshProUGUI _afterUpGradeText;
    [HideInInspector] public TextMeshProUGUI _afterHpText;
    [HideInInspector] public TextMeshProUGUI _afterAtkText;
    [HideInInspector] public TextMeshProUGUI _afterDefText;
    [HideInInspector] public TextMeshProUGUI _mileageValueText;
    [HideInInspector] public TextMeshProUGUI _afterMaxHpText;
    [HideInInspector] public TextMeshProUGUI _afterMaxAtkText;
    [HideInInspector] public TextMeshProUGUI _afterMaxDefText;
    [HideInInspector] public TextMeshProUGUI _enhanceCoinText;
    [HideInInspector] public TextMeshProUGUI _enhanceMaterialText;
    [HideInInspector] public TextMeshProUGUI _characterTokenCountText;
    [HideInInspector] public TextMeshProUGUI _commonTokenCountText;
    [HideInInspector] public TextMeshProUGUI _tokenPopupTitleText;
    [HideInInspector] public TextMeshProUGUI _amountGoldText;
    [HideInInspector] public TextMeshProUGUI _amountCharacterTokenText;
    [HideInInspector] public TextMeshProUGUI _amountCommonTokenText;
    
    [HideInInspector] public Slider _mileageSlider;
    [HideInInspector] public GameObject _beforeMax;
    [HideInInspector] public GameObject _afterMax;
    [HideInInspector] public GameObject _enhanceResultPopup;
    [HideInInspector] public TextMeshProUGUI _enhanceResultText;
    [HideInInspector] public Image _enhanceResultIcon;
    [HideInInspector] public Button _enhanceResultConfirm;
    [HideInInspector] public Button _enhanceButton;
    [HideInInspector] public Button _mileageUseButton;
    [HideInInspector] public Button _mileageUseConfirmButton;
    [HideInInspector] public GameObject _mileageUsePopup;
    [HideInInspector] public Button _characterTokenButton;
    [HideInInspector] public Button _commonTokenButton;
    [HideInInspector] public GameObject _enhanceTokenPopup;
    [HideInInspector] public Button _tokenIncreaseButton;
    [HideInInspector] public Button _tokenDecreaseButton;
    [HideInInspector] public Button _tokenConfirmButton;
    [HideInInspector] public TMP_InputField _tokenInputField;
    [HideInInspector] public Image _enhanceTokenIcon;
    [HideInInspector] public Button _tokenCancelButton;
    [HideInInspector] public Button _autoTokenButton;
    
    //Common UI
    [HideInInspector] public Button _enhanceTabButton;
    [HideInInspector] public Image _detailTabColor;
    [HideInInspector] public Image _enhanceTabColor;
    [HideInInspector] public Image _evolutionTabColor;
    
    private CharacterInfoController _controller;
    private Button _mileageCancelButton;
    private Button _detailTabButton; 
    private Button _evolutionTabButton;

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
        _enhanceText = GetUI<TextMeshProUGUI>("EnhanceText"); 
         
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
        _afterMaxHpText = GetUI<TextMeshProUGUI>("AfterMaxHpText");
        _afterMaxAtkText = GetUI<TextMeshProUGUI>("AfterMaxAtkText");
        _afterMaxDefText = GetUI<TextMeshProUGUI>("AfterMaxDefText");
        _enhanceCoinText = GetUI<TextMeshProUGUI>("EnhanceCoinText");
        _enhanceMaterialText = GetUI<TextMeshProUGUI>("EnhanceMaterialText");
        _characterTokenCountText = GetUI<TextMeshProUGUI>("CharacterTokenCountText");
        _commonTokenCountText = GetUI<TextMeshProUGUI>("CommonTokenCountText");
        _tokenPopupTitleText = GetUI<TextMeshProUGUI>("TokenPopupTitleText");
        _amountGoldText = GetUI<TextMeshProUGUI>("AmountGoldText");
        _amountCharacterTokenText = GetUI<TextMeshProUGUI>("AmountCharacterTokenText");
        _amountCommonTokenText = GetUI<TextMeshProUGUI>("AmountCommonTokenText");

        _autoTokenButton = GetUI<Button>("AutoTokenButton");
        _tokenInputField = GetUI<TMP_InputField>("TokenInputFiled");
        _enhanceTokenIcon = GetUI<Image>("TokenIconImage");
        _tokenCancelButton = GetUI<Button>("TokenCancelButton");
        _tokenIncreaseButton = GetUI<Button>("TokenIncreaseButton");
        _tokenDecreaseButton = GetUI<Button>("TokenDecreaseButton");
        _tokenConfirmButton = GetUI<Button>("TokenConfirmButton");
        _mileageSlider = GetUI<Slider>("MileageSlider");
        _enhanceButton = GetUI<Button>("EnhanceButton"); 
        _enhanceResultConfirm = GetUI<Button>("EnhanceResultConfirm");
        _mileageUseButton = GetUI<Button>("MileageUseButton");
        _mileageUseConfirmButton = GetUI<Button>("MileageConfirmButton");
        _mileageCancelButton = GetUI<Button>("MileageCancelButton");
        _characterTokenButton = GetUI<Button>("CharacterTokenButton");
        _commonTokenButton = GetUI<Button>("CommonTokenButton");

        _enhanceTokenPopup = GetUI("EnhanceTokenPopup");
        _mileageUsePopup = GetUI("MileageUsePopup");
        _enhanceResultPopup = GetUI("EnhanceResultPopup");
    }
    
    /// <summary>
    /// DetailTab UI
    /// </summary>
    private void DetailTabUI()
    {
        _levelUpCoinText = GetUI<TextMeshProUGUI>("LevelUpCoinText");
        _levelUpYongGwaText = GetUI<TextMeshProUGUI>("LevelUpMaterialText");
        _levelText = GetUI<TextMeshProUGUI>("LevelText");
        _nameText = GetUI<TextMeshProUGUI>("NameText");
        _powerLevelText = GetUI<TextMeshProUGUI>("PowerLevelText");
        _atkText = GetUI<TextMeshProUGUI>("AtkText");
        _hpText = GetUI<TextMeshProUGUI>("HpText");
        _defText = GetUI<TextMeshProUGUI>("DefText");
        _skillNormalDescText = GetUI<TextMeshProUGUI>("SkillDescAText");
        _skillSpecialDescText = GetUI<TextMeshProUGUI>("SkillDescBText");
        _skillNormalTitleText = GetUI<TextMeshProUGUI>("SkillTitleAText");
        _skillSpecialTitleText = GetUI<TextMeshProUGUI>("SkillTitleBText");
        
        _beforeBonusAtkText = GetUI<TextMeshProUGUI>("BeforeBonusAtkText");
        _beforeBonusDefText = GetUI<TextMeshProUGUI>("BeforeBonusDefText");
        _beforeBonusHpText = GetUI<TextMeshProUGUI>("BeforeBonusHpText");
        _afterBonusAtkText = GetUI<TextMeshProUGUI>("AfterBonusAtkText");
        _afterBonusDefText = GetUI<TextMeshProUGUI>("AfterBonusDefText");
        _afterBonusHpText = GetUI<TextMeshProUGUI>("AfterBonusHpText");
        _bonusLevelText = GetUI<TextMeshProUGUI>("BonusLevelText");
        _levelGoldAmountText = GetUI<TextMeshProUGUI>("LevelCoinAmountText");
        _levelYongGwaAmountText = GetUI<TextMeshProUGUI>("LevelYongGwaAmountText");

        _levelUpSpecialEffect = GetUI<Image>("LevelUpSpecialEffect");
        _levelUpNormalEffect = GetUI<Image>("LevelUpNormalEffect");
        _skillAIconImage = GetUI<Image>("SkillIconA");
        _skillBIconImage = GetUI<Image>("SkillIconB");
        _characterImage = GetUI<Image>("CharacterImage");

        _bonusExitButton = GetUI<Button>("BonusExitButton");
        _levelUpButton = GetUI<Button>("LevelUpButton");

        _amountGroup = GetUI("AmountTextGroup");
        _bonusPopup = GetUI("BonusPopup");
        _materialGroup = GetUI("MaterialTextGroup");
    }
     
    private void ButtonAddListener()
    {
        _detailTabButton.onClick.AddListener(() => TabButtonClick(InfoTabType.DETAIL));
        _enhanceTabButton.onClick.AddListener(() => TabButtonClick(InfoTabType.ENHANCE));
        _evolutionTabButton.onClick.AddListener(() => TabButtonClick(InfoTabType.EVOLUTION));
        
        _enhanceResultConfirm.onClick.AddListener(()=> _enhanceResultPopup.SetActive(false));
        _mileageUseButton.onClick.AddListener(MileageUsePopupException);
        _mileageCancelButton.onClick.AddListener(() => _mileageUsePopup.SetActive(false));
        _tokenCancelButton.onClick.AddListener(() => _enhanceTokenPopup.SetActive(false));
        _bonusExitButton.onClick.AddListener(() => _bonusPopup.SetActive(false));
    }

    private void TabButtonClick(InfoTabType tabType)
    {
        _controller.CurInfoTabType = tabType;
        _detailTabColor.color = _controller.CurInfoTabType == InfoTabType.DETAIL ? Color.cyan : Color.white;
        _enhanceTabColor.color = _controller.CurInfoTabType == InfoTabType.ENHANCE ? Color.cyan : Color.white;
        _evolutionTabColor.color = _controller.CurInfoTabType == InfoTabType.EVOLUTION ? Color.cyan : Color.white;
        
    }

    private void MileageUsePopupException()
    {
        if (_enhanceResultPopup.activeSelf)
        {
            _enhanceResultPopup.SetActive(false);
            return;
        }

        _mileageUsePopup.SetActive(true);
    }
}
