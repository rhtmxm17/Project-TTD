using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoController : BaseUI
{
    [HideInInspector] public CharacterInfoUI _infoUI;
    [HideInInspector] public List<CharacterInfo> _characterInfos;
    [HideInInspector] public CharacterInfo CurCharacterInfo;

    [HideInInspector] public GameObject _infoPopup;

    [HideInInspector] public int CurIndex = 0;

    private CharacterSort _characterSort;
 
    private Button _prevButton;
    private Button _nextButton;
    private Button _filterButton;

    private SortType _lastSortType;
    private RectTransform _contentForm;
    private Vector2 _characterCellSize;
    private Vector2 _characterSpacing;

    private int _characterCount;
    
    private GameObject _detailTab;
    private GameObject _enhanceTab;
    private GameObject _evolutionTab;
    private GameObject _meanTab; 
    
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
    

    protected override void Awake()
    {
        base.Awake();
        Init();
        SubscribeEvent();

        /////// 더미 인증 테스트 코드
        StartCoroutine(UserDataManager.InitDummyUser(9));
    }

    private void OnEnable() => UpdateCharacterList();

    private void Start()
    {
        //TODO: 테스트용 -> 추후 메인과 붙이면 UpdateCharacterList()에서 넘겨주는거로
        _characterSort._sortCharacterInfos = _characterInfos;
        StartListSort();
    }
    
    private void Init()
    {
        _infoPopup = GetUI("InfoPopup");

        _characterSort = GetUI<CharacterSort>("SortUI");
        _infoUI = GetUI<CharacterInfoUI>("InfoUI");

        _contentForm = GetUI<RectTransform>("Content");
        _prevButton = GetUI<Button>("PreviousButton");
        _nextButton = GetUI<Button>("NextButton");
        _filterButton = GetUI<Button>("FilterButton");
         
        _detailTab = GetUI("DetailTab");
        _enhanceTab = GetUI("EnhanceTab");
        _evolutionTab = GetUI("EvolutionTab");
        _meanTab = GetUI("MeanTab");
        
        _characterCellSize = _contentForm.GetComponent<GridLayoutGroup>().cellSize;
        _characterSpacing = _contentForm.GetComponent<GridLayoutGroup>().spacing;
    }
    
    private void SubscribeEvent()
    {
        _prevButton.onClick.AddListener(PreviousCharacter);
        _nextButton.onClick.AddListener(NextCharacter);
        _filterButton.onClick.AddListener(() => _characterSort.transform.GetChild(0).gameObject.SetActive(true));
    }

    /// <summary>
    /// 보유 캐릭터 리스트 업데이트
    /// </summary>
    private void UpdateCharacterList()
    {
        _characterInfos = GetComponentsInChildren<CharacterInfo>().ToList();
        CharacterCount = _characterInfos.Count;
        //_characterSort._sortCharacterInfos = _characterInfos;
    }

    /// <summary>
    /// 마지막 정렬 방식 저장 후 시작했을 때 해당 방식으로 정렬
    /// </summary>
    private void StartListSort()
    {
        _characterSort.CharacterListSort();
    }

    /// <summary>
    /// 이전 캐릭터 정보로 변경
    /// </summary>
    private void PreviousCharacter()
    {
        if (CurIndex == 0)
        {
            CurIndex = _characterInfos.Count - 1;
        }
        else
        {
            CurIndex--;
        }

        CurCharacterInfo = _characterInfos[CurIndex];
        CurCharacterInfo.UpdateInfo();
    }

    /// <summary>
    /// 다음 캐릭터 정보로 변경
    /// </summary>
    private void NextCharacter()
    {
        if (CurIndex == _characterInfos.Count - 1)
        {
            CurIndex = 0;
        }
        else
        {
            CurIndex++;
        }

        CurCharacterInfo = _characterInfos[CurIndex];
        CurCharacterInfo.UpdateInfo();
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
        _detailTab.SetActive(_curInfoTabType.Equals(InfoTabType.DETAIL));
        _enhanceTab.SetActive(_curInfoTabType.Equals(InfoTabType.ENHANCE));
        _evolutionTab.SetActive(_curInfoTabType.Equals(InfoTabType.EVOLUTION));
        _meanTab.SetActive(_curInfoTabType.Equals(InfoTabType.MEAN)); 
    }
    
}