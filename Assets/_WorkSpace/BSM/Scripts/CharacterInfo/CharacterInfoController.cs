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

    [HideInInspector] public GameObject _infoPopup;

    [HideInInspector] public int CurIndex = 0;

    private List<int> _holdingCharactersIndex = new List<int>();
    
    private GameObject _characterUISet;
    private CharacterSort _characterSort;
    private CharacterFilter _characterFilter;

    private TextMeshProUGUI _orderText;
    private Button _prevButton;
    private Button _nextButton;
    private Button _sortButton;
    private Button _orderButton;
    private Button _elementFilterButton;
    private Button _roleFilterButton;
    private Button _dragonVeinFilterButton;
     
    private SortType _lastSortType;
    private RectTransform _contentForm;
    private Vector2 _characterCellSize;
    private Vector2 _characterSpacing;

    private int _characterCount;

    private GameObject _detailTab;
    private GameObject _enhanceTab;
    private GameObject _evolutionTab;

    private int CharacterCount
    {
        get => _characterInfos.Count;
        set
        {
            _characterCount = value;
            ResizeContentForm();
        }
    }

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
     
    public UserDataInt UserGoldData;
    public TextMeshProUGUI SortButtonText { get; set; }

    protected override void Awake()
    {
        base.Awake(); 
        Init();
        ButtonOnClickEvent();
        SetContentFormGridLayOut();
        GetUserGold(); 
    }

    private void OnEnable()
    { 
        UpdateCharacterList(); 
    }
    
    private void Init()
    {
        _infoPopup = GetUI("InfoPopup");
        _characterUISet = GetUI("CharacterUISet");

        //Sort, Filter GetBind
        _characterFilter = GetUI<CharacterFilter>("FilterUI");
        _characterSort = GetUI<CharacterSort>("SortUI"); 
        
        _infoUI = GetUI<CharacterInfoUI>("InfoUI");

        _contentForm = GetUI<RectTransform>("Content");
        _prevButton = GetUI<Button>("PreviousButton");
        _nextButton = GetUI<Button>("NextButton");

        _sortButton = GetUI<Button>("SortButton");
        SortButtonText = _sortButton.GetComponentInChildren<TextMeshProUGUI>();
        _orderButton = GetUI<Button>("OrderButton");
        _orderText = GetUI<TextMeshProUGUI>("OrderButtonText");
        
        _elementFilterButton = GetUI<Button>("ElementFilterButton");
        _roleFilterButton = GetUI<Button>("RoleFilterButton");
        _dragonVeinFilterButton = GetUI<Button>("DragonVeinFilterButton");
         
        
        _detailTab = GetUI("DetailTab");
        _enhanceTab = GetUI("EnhanceTab");
        _evolutionTab = GetUI("EvolutionTab");

        _characterCellSize = _contentForm.GetComponent<GridLayoutGroup>().cellSize;
        _characterSpacing = _contentForm.GetComponent<GridLayoutGroup>().spacing;
    }

    /// <summary>
    /// 현재 유저가 보유한 골드 가져옴
    /// </summary>
    private void GetUserGold()
    {
        UserGoldData = GameManager.TableData.GetItemData(1).Number;
        _userGold = UserGoldData.Value;
    }
    
    private void ButtonOnClickEvent()
    {
        _prevButton.onClick.AddListener(PreviousCharacter);
        _nextButton.onClick.AddListener(NextCharacter);
        _sortButton.onClick.AddListener(() => _characterSort.transform.GetChild(0).gameObject.SetActive(true));
        _orderButton.onClick.AddListener(()=> Debug.Log("정렬 버튼 클릭"));
        
        _elementFilterButton.onClick.AddListener(() => _characterFilter.transform.GetChild(0).gameObject.SetActive(true));
        _roleFilterButton.onClick.AddListener(() => Debug.Log("역할 필터 클릭"));
        _dragonVeinFilterButton.onClick.AddListener(() => Debug.Log("용맥 버튼 클릭"));
        
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
        //TODO: characterInfos.Where(x=> GameManager.UserData.HasCharacter(x._CharacterData.Id)) 조건 삭제 필요
        
        CharacterCount = _characterInfos.Count;
        _characterSort._sortCharacterInfos= _characterInfos.Where(x=> GameManager.UserData.HasCharacter(x._CharacterData.Id)).ToList();
        _characterSort.CharacterInfoController = this;
        _characterSort.OrderText = _orderText;
        _characterFilter._filterCharacterInfos = _characterInfos.Where(x=> GameManager.UserData.HasCharacter(x._CharacterData.Id)).ToList(); 
        StartListSort();
        
        for (int i = 0; i < _characterInfos.Count; i++)
        {
            if (!GameManager.UserData.HasCharacter(_characterInfos[i]._CharacterData.Id))
            {
                _characterInfos[i].gameObject.SetActive(false);  
            }
            else
            {
                _characterInfos[i].gameObject.SetActive(true);
            }
        }

        _holdingCharactersIndex = _characterInfos.Where(x => GameManager.UserData.HasCharacter(x._CharacterData.Id))
            .Select(x => _characterInfos.IndexOf(x)).ToList();

    }

    /// <summary>
    /// 마지막 정렬 방식 저장 후 시작했을 때 해당 방식으로 정렬
    /// </summary>
    private void StartListSort()
    {
        //1 : 내림차순 , 0 : 오름차순
        _characterSort.IsSorting = PlayerPrefs.GetInt("IsSorting") == 1;
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

            if (_holdingCharactersIndex.Contains(CurIndex)) break;
            
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
            
            if (_holdingCharactersIndex.Contains(CurIndex)) break;
        }
         
        CurCharacterInfo = _characterInfos[CurIndex];
        CurCharacterInfo.UpdateInfo();
    }

    /// <summary>
    /// ContentForm GridLayout 설정
    /// </summary>
    private void SetContentFormGridLayOut()
    {
        GridLayoutGroup contentFormGrid = _contentForm.GetComponent<GridLayoutGroup>();
        contentFormGrid.cellSize = new Vector2(300, 467);
        contentFormGrid.spacing = new Vector2(80, 0);
    }

    /// <summary>
    /// 캐릭터 리스트 Content Form ReSize 기능
    /// </summary>
    private void ResizeContentForm()
    {
        //Width 300 + space 80  
        //(캐릭터 Width + spacing) * List.Count + 1 > Content.SizeDelta.x -> ReSize 진행
        float totalWidth = (_characterCellSize.x + _characterSpacing.x) * _characterInfos.Count + 1;
        float contentWidth = _contentForm.sizeDelta.x;

        if (totalWidth < contentWidth) return;

        _contentForm.sizeDelta = new Vector2(totalWidth, _contentForm.sizeDelta.y);
    }

    /// <summary>
    /// 캐릭터 상세 팝업창 > 탭 전환 기능
    /// </summary>
    private void TabSwitch()
    {
        _characterUISet.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL) || _curInfoTabType.Equals(InfoTabType.ENHANCE));
        _detailTab.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL));
        _enhanceTab.SetActive(_curInfoTabType.Equals(InfoTabType.ENHANCE));
        _evolutionTab.SetActive(_curInfoTabType.Equals(InfoTabType.EVOLUTION));
    }
}