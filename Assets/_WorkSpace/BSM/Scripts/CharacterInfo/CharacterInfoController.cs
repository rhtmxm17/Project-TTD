using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoController : BaseUI
{
    [HideInInspector] public CharacterInfoUI _infoUI;
    public List<CharacterInfo> _characterInfos;
    [HideInInspector] public CharacterInfo CurCharacterInfo;
    [HideInInspector] public CharacterEnhance CurCharacterEnhance;

    [HideInInspector] public GameObject _infoPopup;

    [HideInInspector] public int CurIndex = 0;

    private GameObject _characterUISet;
    private CharacterSort _characterSort;
    private CharacterFilter _characterFilter;

    private Button _prevButton;
    private Button _nextButton;
    private Button _sortButton;
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

    private int _userGold;

    public int UserGold
    {
        get => _userGold;
        set { _userGold = value; }
    }

    public UserDataInt UserGoldData;
    public TextMeshProUGUI SortButtonText { get; set; }

    protected override void Awake()
    {
        base.Awake(); 
        Init();
        ButtonOnClickEvent();
        SetContentFormGridLayOut(); 
        SetDatabase(); 
    }

    private void SetDatabase()
    {
        UserGoldData = GameManager.TableData.GetItemData(1).Number;

        GameManager.UserData.StartUpdateStream()
            .SetDBValue(UserGoldData, UserGoldData.Value + 10000)
            .Submit(result =>
                {
                    if (false == result)
                    {
                        Debug.LogWarning("요청 전송에 실패했습니다");
                        return;
                    }
        
                    _userGold = UserGoldData.Value;
                    Debug.Log($"보유 골드 :{_userGold}");
                }
            );
        
        /////// 더미 인증 테스트 코드 
        GameManager.UserData.TryInitDummyUserAsync(38, () =>
        {
            // TODO: 유저데이터 불러오기 완료시 처리 
        });
    }

    private void OnEnable() => UpdateCharacterList();

    private void Init()
    {
        _infoPopup = GetUI("InfoPopup");
        _characterUISet = GetUI("CharacterUISet");

        //Sort, Filter GetBind
        _characterFilter = GetUI<CharacterFilter>("FilterUI");
        _characterSort = GetUI<CharacterSort>("SortUI");
        _characterSort.CurSortType = (SortType)PlayerPrefs.GetInt("SortType", 1);
        
        _infoUI = GetUI<CharacterInfoUI>("InfoUI");

        _contentForm = GetUI<RectTransform>("Content");
        _prevButton = GetUI<Button>("PreviousButton");
        _nextButton = GetUI<Button>("NextButton");

        _sortButton = GetUI<Button>("SortButton");
        SortButtonText = _sortButton.GetComponentInChildren<TextMeshProUGUI>();
        _filterButton = GetUI<Button>("FilterButton");


        _detailTab = GetUI("DetailTab");
        _enhanceTab = GetUI("EnhanceTab");
        _evolutionTab = GetUI("EvolutionTab");
        _meanTab = GetUI("MeanTab");

        _characterCellSize = _contentForm.GetComponent<GridLayoutGroup>().cellSize;
        _characterSpacing = _contentForm.GetComponent<GridLayoutGroup>().spacing;
    }

    private void ButtonOnClickEvent()
    {
        _prevButton.onClick.AddListener(PreviousCharacter);
        _nextButton.onClick.AddListener(NextCharacter);
        _sortButton.onClick.AddListener(() => _characterSort.transform.GetChild(0).gameObject.SetActive(true));
        _filterButton.onClick.AddListener(() => _characterFilter.transform.GetChild(0).gameObject.SetActive(true));
    }

    /// <summary>
    /// 보유 캐릭터 리스트 업데이트
    /// </summary>
    private void UpdateCharacterList()
    {
        _characterInfos = GetComponentsInChildren<CharacterInfo>().ToList();

        //TODO: 임시로 레벨 정보 업데이트, 추후에 로딩과 붙으면 해줄 필요 없음
        for (int i = 0; i < _characterInfos.Count; i++)
        {
            _characterInfos[i].SetCharacterData();
        }

        CharacterCount = _characterInfos.Count;
        _characterSort._sortCharacterInfos = _characterInfos;
        _characterSort.CharacterInfoController = this;
        _characterFilter._filterCharacterInfos = _characterInfos;
        StartListSort();
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
        //TODO: 상세 팝업창 좌,우 버튼 노출 조건 추가 필요할 것으로 예상중
        //조건 추가가 필요하다면 _characterInfos 카운트 2 미만이면 노출x

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
        _meanTab.SetActive(_curInfoTabType.Equals(InfoTabType.MEAN));
    }
}