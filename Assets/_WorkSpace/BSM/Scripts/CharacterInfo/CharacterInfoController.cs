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
    
    protected override void Awake()
    {
        base.Awake();
        Init();
        SubscribeEvent();

        /////// 더미 인증 테스트 코드
        StartCoroutine(UserDataManager.InitDummyUser(7));
    }

    private void OnEnable() => UpdateCharacterList();

    private void Start()
    {
        //TODO: 테스트용 -> 추후 메인과 붙이면 OnEnable에서 넘겨주는거로
        _characterSort._sortCharacterInfos = _characterInfos;
        StartListSort(); 
    } 
    
    private void Init()
    {
        _infoPopup = GetUI("InfoPopup");
        
        _characterSort = GetUI<CharacterSort>("SortUI"); 
        _infoUI = GetUI<CharacterInfoUI>("InfoUI");
 
        _prevButton = GetUI<Button>("PreviousButton");
        _nextButton = GetUI<Button>("NextButton");
        _filterButton = GetUI<Button>("FilterButton");
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
        //_characterSort._sortCharacterInfos = _characterInfos;
    }

    /// <summary>
    /// 마지막 정렬 방식 저장 후 시작했을 때 해당 방식으로 정렬
    /// </summary>
    private void StartListSort()
    {
        _lastSortType = (SortType)PlayerPrefs.GetInt("SortType"); 
        switch (_lastSortType)
        {
            case SortType.NAME:
                _characterSort.NameSort();
                break;
            
            case SortType.LEVEL:
                _characterSort.LevelSort();
                break;
            
        }
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
 
}