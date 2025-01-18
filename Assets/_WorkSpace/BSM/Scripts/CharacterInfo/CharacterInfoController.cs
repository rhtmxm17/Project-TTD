using System;
using System.Collections.Generic;
using System.Linq; 
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoController : BaseUI
{
    public List<CharacterData> CharacterDatas;
    
    public Camera _renderCamera;
    public GameObject ModelParent;
    public List<Sprite> TokenIcons;
    
    [HideInInspector] public CharacterInfoUI _infoUI;
    [HideInInspector] public List<CharacterInfo> _characterInfos;
    [HideInInspector] public CharacterInfo CurCharacterInfo;
    [HideInInspector] public CharacterEnhance CurCharacterEnhance;
    [HideInInspector] public TextMeshProUGUI SortButtonText;
    [HideInInspector] public GameObject _infoPopup;
    [HideInInspector] public TextMeshProUGUI ElementFilterText;
    [HideInInspector] public TextMeshProUGUI RoleFilterText;
    [HideInInspector] public TextMeshProUGUI DragonVeinFilterText;
    [HideInInspector] public CharacterFilter _characterFilter;
    
    [HideInInspector] public UserDataInt UserGoldData;
    /// <summary>
    /// 용 강화 재화
    /// </summary>
    [HideInInspector] public UserDataInt UserDragonCandyData;
    
    /// <summary>
    /// 용 레벨업 재화
    /// </summary>
    [HideInInspector] public UserDataInt UserYongGwaData; 
    
    [HideInInspector] public int CurIndex = 0;
    private int _evolutionIndex;
    
    private (Vector3, float) _evolutionRender;
    private Vector3 _dulkPos = new Vector3(0.3f, 1.6f, -10f);
    private Vector3 _nepewPos = new Vector3(0.3f, 2f, -10f);
    private Vector3 _sunnyPos = new Vector3(0, 1.5f, -10f);
    private Vector3 _commonPos = new Vector3(0, 1f, -10f);

    private CharacterInfoPopup _characterInfoPopupCs;
    private CharacterSort _characterSort;

    private TextMeshProUGUI _evolutionText;
    private Image _sortingImage;
    
    private Button _leftEvolutionButton;
    private Button _rightEvolutionButton;
    private Button _prevButton;
    private Button _nextButton;
    private Button _sortButton;
    private Button _sortingButton;
    private Button _elementFilterButton;
    private Button _roleFilterButton;
    private Button _dragonVeinFilterButton;
    
    private GameObject _characterUIPanel;
    private GameObject _detailTab;
    private GameObject _enhanceTab;
    private GameObject _evolutionTab;
    
    private SortType _lastSortType;
    private InfoTabType _curInfoTabType;

    public InfoTabType CurInfoTabType
    {
        get => _curInfoTabType;
        set
        {
            _curInfoTabType = value;
            TabSwitch();
        }
    }

    private int _userYongGwa;
    
    /// <summary>
    /// 용 레벨업 재화
    /// </summary>
    public int UserYongGwa
    {
        get => _userYongGwa;
        set
        {
            _userYongGwa = value;
            _infoUI._levelYongGwaAmountText.text = CurrencyFormat.Trans(_userYongGwa);
        }
    }
    
    private int _userGold;
    
    /// <summary>
    /// 용 레벨업 골드 재화
    /// </summary>
    public int UserGold { 
        get => _userGold;
        set
        {
            _userGold = value;
            _infoUI._amountGoldText.text = CurrencyFormat.Trans(_userGold);
            _infoUI._levelGoldAmountText.text = CurrencyFormat.Trans(_userGold); 
        } 
    }
    
    private int _userDragonCandy;
    
    /// <summary>
    /// 용 강화 재화
    /// </summary>
    public int UserDragonCandy
    {
        get => _userDragonCandy;
        set
        {
            _userDragonCandy = value;
            _infoUI._amountCommonTokenText.text = CurrencyFormat.Trans(_userDragonCandy);
        }
    }

    private int _userCharacterToken;
    
    public int UserCharacterToken
    {
        get => _userCharacterToken;
        set
        {
            _userCharacterToken = value;
            _infoUI._amountCharacterTokenText.text = CurrencyFormat.Trans(_userCharacterToken);
        }
    }
    
    private GridLayoutGroup _gridLayout;
    private Scrollbar _verticalBar;
    
    protected override void Awake()
    {
        base.Awake(); 
        Init();
        ButtonOnClickEvent();
        GetUserMaterials();
        RatioPaddingModify();
        SetVerticalScrollbar();
    } 
    
    private void OnEnable()
    { 
        UpdateCharacterList(); 
    }
    
    private void Init()
    {
        _detailTab = GetUI("DetailTab");
        _enhanceTab = GetUI("EnhanceTab");
        _evolutionTab = GetUI("EvolutionTab");
        _infoPopup = GetUI("InfoPopup");
        _characterUIPanel = GetUI("CharacterUIPanel");

        _verticalBar = GetUI<Scrollbar>("Scrollbar Vertical");
        _gridLayout = GetUI<GridLayoutGroup>("Content");
        
        //Sort, Filter GetBind
        _characterFilter = GetUI<CharacterFilter>("FilterUI");
        _characterSort = GetUI<CharacterSort>("SortUI"); 
        _infoUI = GetUI<CharacterInfoUI>("InfoUI");
        _characterInfoPopupCs = GetUI<CharacterInfoPopup>("InfoPopup");
        
        _prevButton = GetUI<Button>("PreviousButton");
        _nextButton = GetUI<Button>("NextButton");
        _sortButton = GetUI<Button>("SortButton");
        _elementFilterButton = GetUI<Button>("ElementFilterButton");
        _roleFilterButton = GetUI<Button>("RoleFilterButton");
        _dragonVeinFilterButton = GetUI<Button>("DragonVeinFilterButton");
        _sortingButton = GetUI<Button>("SortingButton");
        _leftEvolutionButton = GetUI<Button>("LeftEvolutionButton");
        _rightEvolutionButton = GetUI<Button>("RightEvolutionButton");
        
        SortButtonText = _sortButton.GetComponentInChildren<TextMeshProUGUI>();

        _evolutionText = GetUI<TextMeshProUGUI>("EvolutionText");
        _sortingImage = GetUI<Image>("SortingButtonImage");
        ElementFilterText = GetUI<TextMeshProUGUI>("ElementFilterText"); 
        RoleFilterText = GetUI<TextMeshProUGUI>("RoleFilterText");
        DragonVeinFilterText = GetUI<TextMeshProUGUI>("DragonVeinFilterText");
        
    }
 
    private void SetVerticalScrollbar()
    {
        _verticalBar.value = 10;
    }
    
    private void RatioPaddingModify()
    {
        float ratio = (float)Screen.width / Screen.height;
        _gridLayout.padding.left = (Mathf.Floor(ratio * 10f) / 10f) switch
        {
            2.4f => 180,
            2.7f => 165,
            1.7f => 125,
            _ => 50
        };
    }
    
    /// <summary>
    /// 현재 유저가 보유한 골드 가져옴
    /// </summary>
    private void GetUserMaterials()
    {
        UserGoldData = GameManager.TableData.GetItemData(1).Number;
        UserDragonCandyData = GameManager.TableData.GetItemData(2).Number;
        UserYongGwaData = GameManager.TableData.GetItemData(5).Number;
        
        _userGold = UserGoldData.Value;
        _userDragonCandy = UserDragonCandyData.Value;
        _userYongGwa = UserYongGwaData.Value; 
    }
    
    private void ButtonOnClickEvent()
    {
        _leftEvolutionButton.onClick.AddListener(EvolutionPrevCharacter);
        _rightEvolutionButton.onClick.AddListener(EvolutionNextCharacter);
        _prevButton.onClick.AddListener(DetailPreviousCharacter);
        _nextButton.onClick.AddListener(DetailNextCharacter);
        _sortButton.onClick.AddListener(() => _characterSort.transform.GetChild(0).gameObject.SetActive(true));
        
        _sortingButton.onClick.AddListener(()=> _characterSort.SortingLayerEvent());
        
        _elementFilterButton.onClick.AddListener(() => _characterFilter.transform.GetChild(0).gameObject.SetActive(true));
        
        _roleFilterButton.onClick.AddListener(() => _characterFilter.transform.GetChild(1).gameObject.SetActive(true));
        
        _dragonVeinFilterButton.onClick.AddListener(() => _characterFilter.transform.GetChild(2).gameObject.SetActive(true));

    }

    /// <summary>
    /// 보유 캐릭터 리스트 업데이트
    /// </summary>
    private void UpdateCharacterList()
    { 
        _characterSort.CurSortType = PlayerPrefs.HasKey("SortType") ? (SortType)PlayerPrefs.GetInt("SortType") : SortType.LEVEL;
        
        _characterInfos = GetComponentsInChildren<CharacterInfo>(true).ToList();

        for (int i = 0; i < CharacterDatas.Count; i++)
        {
            _characterInfos[i]._CharacterData = CharacterDatas[i];
        }
        
        for (int i = 0; i < _characterInfos.Count; i++)
        {
            _characterInfos[i].SetCharacterData();
        }

        _characterSort._sortCharacterInfos= _characterInfos;
        _characterSort.CharacterInfoController = this;
        _characterSort.SortingImage = _sortingImage;
        _characterFilter._filterCharacterInfos = _characterInfos;
        _characterFilter.CharacterController = this;
        StartListSort();
    }
    
    /// <summary>
    /// 마지막 정렬 방식 저장 후 시작했을 때 해당 방식으로 정렬
    /// </summary>
    public void StartListSort()
    {
        //1(True) : 내림차순 , 0(False) : 오름차순
        _characterSort.IsSorting = PlayerPrefs.HasKey("IsSorting") ? PlayerPrefs.GetInt("IsSorting") == 1 : true;
        _characterSort.CharacterListSort();
    }
    
    /// <summary>
    /// 상세 탭 : 이전 캐릭터 정보로 변경
    /// </summary>
    private void DetailPreviousCharacter()
    {
        while (true)
        {
            if (CurIndex == 0)
            {
                CurIndex = _characterInfos.Count - 1;
            }
            else
            {
                CurIndex--;
            }

            if (_characterInfos[CurIndex].gameObject.activeSelf) break;
        }
        
        if (_infoUI._enhanceResultPopup.activeSelf)
        {
            _infoUI._enhanceResultPopup.SetActive(false);
        }
        
        CurCharacterInfo = _characterInfos[CurIndex];
        CurCharacterInfo.UpdateInfo();
    }

    /// <summary>
    /// 상세 탭 : 다음 캐릭터 정보로 변경
    /// </summary>
    private void DetailNextCharacter()
    {
        while (true)
        {
            if (CurIndex == _characterInfos.Count - 1)
            {
                CurIndex = 0;
            }
            else
            {
                CurIndex++;
            }
            
            if (_characterInfos[CurIndex].gameObject.activeSelf) break;
        }
        
        if (_infoUI._enhanceResultPopup.activeSelf)
        {
            _infoUI._enhanceResultPopup.SetActive(false);
        }
        
        CurCharacterInfo = _characterInfos[CurIndex];
        CurCharacterInfo.UpdateInfo();
    }

    /// <summary>
    /// 캐릭터 상세 팝업창 > 탭 전환 기능
    /// </summary>
    private void TabSwitch()
    {
        if (_infoUI._enhanceResultPopup.activeSelf)
        {
            _infoUI._enhanceResultPopup.SetActive(false);
        }

        if (CurCharacterInfo.CharacterModels.Count != 0)
        {
            CurCharacterInfo.CharacterModels[_characterInfoPopupCs.ListIndex].gameObject.SetActive(_curInfoTabType.Equals(InfoTabType.EVOLUTION));
        }
       
        
        _evolutionIndex = 0;
        
        _characterInfoPopupCs.ListIndex = _evolutionIndex;
        
        _evolutionRender = CurCharacterInfo._CharacterData.Name switch
        {
            "덜크" => (_dulkPos, 20f),
            "네퓨" => (_nepewPos, 28f),
            "하트핑" =>  (_sunnyPos, 20f),
            "써니" => (_sunnyPos, 20f),
            _ => (_commonPos, 13f)
        };

        RenderCamera(_evolutionRender.Item1, _evolutionRender.Item2); 
        EvolutionCharacterUI();
        
        //강화 탭 등록한 토큰 개수 UI
        _infoUI._characterTokenCountText.text = 0.ToString();
        _infoUI._commonTokenCountText.text = 0.ToString();
        
        UserGold = UserGoldData.Value;
        UserDragonCandy = UserDragonCandyData.Value;
        UserCharacterToken = GameManager.TableData.GetItemData(CurCharacterInfo._CharacterData.EnhanceItemID).Number.Value;
        
        _infoUI._beforeMax.SetActive(_curInfoTabType.Equals(InfoTabType.ENHANCE));
        _nextButton.gameObject.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL));
        _prevButton.gameObject.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL));
        _characterUIPanel.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL) || _curInfoTabType.Equals(InfoTabType.ENHANCE));
        _detailTab.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL));
        _enhanceTab.SetActive(_curInfoTabType.Equals(InfoTabType.ENHANCE));
        _evolutionTab.SetActive(_curInfoTabType.Equals(InfoTabType.EVOLUTION));
    }

    /// <summary>
    /// 진화 탭 : 이전 레벨 진화 캐릭터
    /// </summary>
    private void EvolutionPrevCharacter()
    {
        if (_evolutionIndex > 0)
        {
            CurCharacterInfo.CharacterModels[_evolutionIndex].gameObject.SetActive(false);
            _evolutionIndex--;
        }
        
        CurCharacterInfo.CharacterModels[_evolutionIndex].gameObject.SetActive(true);
        _characterInfoPopupCs.ListIndex = _evolutionIndex;
        
        EvolutionCharacterUI();
    }

    /// <summary>
    /// 진화 탭 : 다음 레벨 진화 캐릭터
    /// </summary>
    private void EvolutionNextCharacter()
    {
        if (_evolutionIndex < CurCharacterInfo.CharacterModels.Count)
        {
            CurCharacterInfo.CharacterModels[_evolutionIndex].gameObject.SetActive(false);
            _evolutionIndex++;
        }
         
        CurCharacterInfo.CharacterModels[_evolutionIndex].gameObject.SetActive(true);
        _characterInfoPopupCs.ListIndex = _evolutionIndex;
        
        EvolutionCharacterUI();
    }

    /// <summary>
    /// 진화 캐릭터 노출 조건
    /// </summary>
    private void EvolutionCharacterUI()
    {
        _leftEvolutionButton.gameObject.SetActive(_evolutionIndex != 0);
        _rightEvolutionButton.gameObject.SetActive(_evolutionIndex != CurCharacterInfo.CharacterModels.Count - 1);
        _evolutionText.text = $"진화 Lv.{_evolutionIndex + 1}";
    }

    private void RenderCamera(Vector3 pos, float angle)
    {
        _renderCamera.transform.position = pos;
        _renderCamera.fieldOfView = angle;
    }
    
}
