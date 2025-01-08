using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.DemiLib;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoController : BaseUI
{
    [HideInInspector] public CharacterInfoUI _infoUI;
    [HideInInspector] public List<CharacterInfo> _characterInfos;
    [HideInInspector] public CharacterInfo CurCharacterInfo;
    [HideInInspector] public CharacterEnhance CurCharacterEnhance;
    [HideInInspector] public TextMeshProUGUI SortButtonText;
    [HideInInspector] public GameObject _infoPopup;
    [HideInInspector] public int CurIndex = 0;
    [HideInInspector] public TextMeshProUGUI ElementFilterText;
    [HideInInspector] public TextMeshProUGUI RoleFilterText;
    [HideInInspector] public TextMeshProUGUI DragonVeinFilterText;
    
    private List<int> _holdingCharactersIndex = new List<int>();
    
    private GameObject _characterUIPanel;
    private CharacterSort _characterSort;
    private CharacterFilter _characterFilter;
     
    private TextMeshProUGUI _sortingText; 
    private Button _prevButton;
    private Button _nextButton;
    private Button _sortButton;
    private Button _sortingButton;
    private Button _elementFilterButton;
    private Button _roleFilterButton;
    private Button _dragonVeinFilterButton;
     
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

    private int _userGold;
    public int UserGold { 
        get => _userGold;
        set
        {
            _userGold = value;
        } 
    }

    private int _userLevelMaterial;

    public int UserLevelMaterial
    {
        get => _userLevelMaterial;
        set
        {
            _userLevelMaterial = value;
        }
    }

    private int _userEnhanceMaterial; 
    public int UserEnhanceMaterial
    {
        get => _userEnhanceMaterial;
        set
        {
            _userEnhanceMaterial = value;
        }
    }
    
    
    public UserDataInt UserGoldData;
    public UserDataInt UserLevelMaterialData;
    public UserDataInt UserEnhanceMaterialData;
    
    
    protected override void Awake()
    {
        base.Awake(); 
        Init();
        ButtonOnClickEvent();
        GetUserMaterials(); 
    }

    private void OnEnable()
    { 
        UpdateCharacterList(); 
    }
    
    private void Init()
    {
        _infoPopup = GetUI("InfoPopup");
        _characterUIPanel = GetUI("CharacterUIPanel");

        //Sort, Filter GetBind
        _characterFilter = GetUI<CharacterFilter>("FilterUI");
        _characterSort = GetUI<CharacterSort>("SortUI"); 
        
        _infoUI = GetUI<CharacterInfoUI>("InfoUI");
        
        _prevButton = GetUI<Button>("PreviousButton");
        _nextButton = GetUI<Button>("NextButton");

        _sortButton = GetUI<Button>("SortButton");
        SortButtonText = _sortButton.GetComponentInChildren<TextMeshProUGUI>();
        _sortingButton = GetUI<Button>("SortingButton");
        _sortingText = GetUI<TextMeshProUGUI>("SortingButtonText");
        
        _elementFilterButton = GetUI<Button>("ElementFilterButton");
        _roleFilterButton = GetUI<Button>("RoleFilterButton");
        _dragonVeinFilterButton = GetUI<Button>("DragonVeinFilterButton");
        ElementFilterText = GetUI<TextMeshProUGUI>("ElementFilterText"); 
        RoleFilterText = GetUI<TextMeshProUGUI>("RoleFilterText");
        DragonVeinFilterText = GetUI<TextMeshProUGUI>("DragonVeinFilterText");
        
        _detailTab = GetUI("DetailTab");
        _enhanceTab = GetUI("EnhanceTab");
        _evolutionTab = GetUI("EvolutionTab");
        
    }

    /// <summary>
    /// 현재 유저가 보유한 골드 가져옴
    /// </summary>
    private void GetUserMaterials()
    {
        UserGoldData = GameManager.TableData.GetItemData(1).Number;
        
        UserLevelMaterialData = GameManager.TableData.GetItemData(2).Number;
        
        UserEnhanceMaterialData = GameManager.TableData.GetItemData(3).Number;

        _userGold = UserGoldData.Value;
        _userLevelMaterial = UserLevelMaterialData.Value;
        _userEnhanceMaterial = UserEnhanceMaterialData.Value; 
    }
    
    private void ButtonOnClickEvent()
    {
        _prevButton.onClick.AddListener(PreviousCharacter);
        _nextButton.onClick.AddListener(NextCharacter);
        _sortButton.onClick.AddListener(() => _characterSort.transform.GetChild(0).gameObject.SetActive(true));
        _sortingButton.onClick.AddListener(()=> _characterSort.SortingLayerEvent());
        
        _elementFilterButton.onClick.AddListener(() => _characterFilter.transform.GetChild(0).gameObject.SetActive(true));
        _roleFilterButton.onClick.AddListener(() => _characterFilter.transform.GetChild(1).gameObject.SetActive(true));
        _dragonVeinFilterButton.onClick.AddListener(() =>_characterFilter.transform.GetChild(2).gameObject.SetActive(true));
        
    }

    /// <summary>
    /// 보유 캐릭터 리스트 업데이트
    /// </summary>
    private void UpdateCharacterList()
    { 
        _characterSort.CurSortType = PlayerPrefs.HasKey("SortType") ? (SortType)PlayerPrefs.GetInt("SortType") : SortType.LEVEL;
        
        _characterInfos = GetComponentsInChildren<CharacterInfo>(true).ToList();
        
        for (int i = 0; i < _characterInfos.Count; i++)
        {
            _characterInfos[i].SetCharacterData();
        }

        _characterSort._sortCharacterInfos= _characterInfos;
        _characterSort.CharacterInfoController = this;
        _characterSort.SortingText = _sortingText;
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
    /// 이전 캐릭터 정보로 변경
    /// </summary>
    private void PreviousCharacter()
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
    /// 다음 캐릭터 정보로 변경
    /// </summary>
    private void NextCharacter()
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
        
        _nextButton.gameObject.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL));
        _prevButton.gameObject.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL));
        _characterUIPanel.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL) || _curInfoTabType.Equals(InfoTabType.ENHANCE));
        _detailTab.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL));
        _enhanceTab.SetActive(_curInfoTabType.Equals(InfoTabType.ENHANCE));
        _evolutionTab.SetActive(_curInfoTabType.Equals(InfoTabType.EVOLUTION));
    }
}