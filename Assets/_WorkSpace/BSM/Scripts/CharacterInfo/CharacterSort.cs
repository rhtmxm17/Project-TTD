using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class CharacterSort : MonoBehaviour
{
    [HideInInspector] public CharacterInfoController CharacterInfoController;
    [HideInInspector] public List<CharacterInfo> _sortCharacterInfos;
    [HideInInspector] public TextMeshProUGUI SortingText;
   
    private List<CharacterData> _ownedCharacters;
    private List<CharacterData> _unOwnedCharacters;
    private List<int> _sortList;
    private CharacterSortUI _characterSortUI;
    
    private SortType _curSortType;

    public SortType CurSortType
    {
        get => _curSortType;
        set => _curSortType = value;
    }

    private bool _isSorting;

    public bool IsSorting
    {
        get => _isSorting;
        set
        {
            _isSorting = value;
        }
    }

    private bool _isStart;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        SubscribeEvent();
    }

    private void Init()
    {
        _characterSortUI = GetComponent<CharacterSortUI>();
    }

    private void SubscribeEvent()
    {
        _characterSortUI.LevelSortButton.onClick.AddListener(() => SortEventFunc(SortType.LEVEL));
        _characterSortUI.PowerLevelSortButton.onClick.AddListener(() => SortEventFunc(SortType.POWERLEVEL));
        _characterSortUI.EnhanceLevelSortButton.onClick.AddListener(() => SortEventFunc(SortType.ENHANCELEVEL));
        _characterSortUI.AttackPowerSortButton.onClick.AddListener(() => SortEventFunc(SortType.OFFENSIVEPOWER));
        _characterSortUI.DefensePowerSortButton.onClick.AddListener(()=> SortEventFunc(SortType.DEFENSEIVEPOWER));
        _characterSortUI.HealthPowerSortButton.onClick.AddListener(() => SortEventFunc(SortType.HEALTH)); 
    }

    /// <summary>
    /// 정렬 함수 호출 및 현재 타입 받아오기
    /// </summary>
    /// <param name="type"></param>
    private void SortEventFunc(SortType type)
    {
        _curSortType = type; 
        CharacterListSort();
    }

    public void SortingLayerEvent()
    {
        _isSorting = !_isSorting;
        PlayerPrefs.SetInt("IsSorting", _isSorting ? 1 : 0);
        CharacterListSort();
    }
    
    /// <summary>
    /// 캐릭터 리스트 정렬 기능
    /// </summary>
    public void CharacterListSort()
    {
        
       SortingText.text = _isSorting ? "↓" : "↑";
       
       if (!_isStart)
       {
           _isStart = true;
           StartSort();
       }
  
       for (int i = 0; i < _sortCharacterInfos.Count; i++)
       {
           if (!GameManager.UserData.HasCharacter(_sortCharacterInfos[i]._CharacterData.Id))
           {
               for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
               {
                   if (GameManager.UserData.HasCharacter(_sortCharacterInfos[j]._CharacterData.Id))
                   {
                       CharacterData missingData = _sortCharacterInfos[i]._CharacterData;
                       CharacterData ownedData = _sortCharacterInfos[j]._CharacterData;
                        
                       _sortCharacterInfos[i]._CharacterData = ownedData;
                       _sortCharacterInfos[j]._CharacterData = missingData;
                       
                       _sortCharacterInfos[i].SetListNameText(ownedData.Name);
                       _sortCharacterInfos[j].SetListNameText(missingData.Name);
                       
                       _sortCharacterInfos[i].PowerLevel = (int)ownedData.PowerLevel;
                       _sortCharacterInfos[j].PowerLevel = (int)missingData.PowerLevel;
                       
                       _sortCharacterInfos[i].SetListTypeText((ElementType)ownedData.StatusTable.type);
                       _sortCharacterInfos[j].SetListTypeText((ElementType)missingData.StatusTable.type);
                       
                       _sortCharacterInfos[i].SetListImage(ownedData.FaceIconSprite);
                       _sortCharacterInfos[j].SetListImage(missingData.FaceIconSprite);
                       break;
                   }
               }
           } 
       }
          
       
        _ownedCharacters = _sortCharacterInfos.Where(x=> GameManager.UserData.HasCharacter(x._CharacterData.Id)).Select(x => x._CharacterData).ToList();
        _unOwnedCharacters = _sortCharacterInfos.Where(x => !GameManager.UserData.HasCharacter(x._CharacterData.Id)).Select(x => x._CharacterData).ToList();
        
        int ownedCount = _ownedCharacters.Count;
        int unOwnedCount = _unOwnedCharacters.Count;
    
        if (_isSorting)
        {
            _ownedCharacters.Sort((CharacterData a, CharacterData b) => {return GetSortValue(b).CompareTo(GetSortValue(a));});
            _unOwnedCharacters.Sort((CharacterData a, CharacterData b) => {return GetSortValue(b).CompareTo(GetSortValue(a));});
        }
        else
        {
            _ownedCharacters.Sort((CharacterData a, CharacterData b) => {return GetSortValue(a).CompareTo(GetSortValue(b));});
            _unOwnedCharacters.Sort((CharacterData a, CharacterData b) => {return GetSortValue(a).CompareTo(GetSortValue(b));});
        }
        
        for (int i = 0; i < ownedCount; i++)
        {
            _sortCharacterInfos[i]._CharacterData = _ownedCharacters[i];
            _sortCharacterInfos[i].SetListNameText(_ownedCharacters[i].Name);
            _sortCharacterInfos[i].PowerLevel = (int)_ownedCharacters[i].PowerLevel;
            _sortCharacterInfos[i].SetListTypeText((ElementType)_ownedCharacters[i].StatusTable.type);
            _sortCharacterInfos[i].SetListImage(_ownedCharacters[i].FaceIconSprite);
            _sortCharacterInfos[i].OwnedObject.SetActive(false);
        }
    
        for (int i = 0; i < unOwnedCount; i++)
        {
            _sortCharacterInfos[i + ownedCount]._CharacterData = _unOwnedCharacters[i];
            _sortCharacterInfos[i + ownedCount].SetListNameText(_unOwnedCharacters[i].Name);
            _sortCharacterInfos[i + ownedCount].PowerLevel = (int)_unOwnedCharacters[i].PowerLevel;
            _sortCharacterInfos[i + ownedCount].SetListTypeText((ElementType)_unOwnedCharacters[i].StatusTable.type);
            _sortCharacterInfos[i + ownedCount].SetListImage(_unOwnedCharacters[i].FaceIconSprite);
            _sortCharacterInfos[i + ownedCount].OwnedObject.SetActive(true);
        }
        
        ChangeSortButtonText();
        PlayerPrefs.SetInt("SortType", (int)_curSortType);
    }

    
    /// <summary>
    /// 현재 정렬 타입으로 버튼명 변경
    /// </summary>
    /// <exception cref="AggregateException"></exception>
    private void ChangeSortButtonText()
    {
        CharacterInfoController.SortButtonText.text = _curSortType switch
        {
            SortType.LEVEL => "레벨",
            SortType.POWERLEVEL => "전투력",
            SortType.ENHANCELEVEL => "강화",
            SortType.OFFENSIVEPOWER => "공격력",
            SortType.DEFENSEIVEPOWER => "방어력",
            SortType.HEALTH => "체력",
            _ => throw new AggregateException("잘못된 타입")
        };
    }
    
    
    /// <summary>
    /// 게임 시작 시 캐릭터 리스트 UI 설정
    /// </summary>
    private void StartSort()
    {
        for (int i = 0; i < _sortCharacterInfos.Count; i++)
        {
            _sortCharacterInfos[i].StartSetCharacterUI();
        }
    }
 
    /// <summary>
    /// 정렬할 타입에 따라 정렬 값 분류
    /// </summary>
    /// <param name="characterInfo">캐릭터 데이터</param>
    /// <returns>정렬할 값</returns>
    private int GetSortValue(CharacterData characterInfo)
    {
        return _curSortType switch
        {
            SortType.LEVEL => characterInfo.Level.Value,
            SortType.POWERLEVEL => (int)characterInfo.PowerLevel,
            SortType.ENHANCELEVEL => characterInfo.Enhancement.Value,
            SortType.OFFENSIVEPOWER => (int)characterInfo.AttackPointLeveled,
            SortType.DEFENSEIVEPOWER => (int)characterInfo.DefensePointLeveled,
            SortType.HEALTH => (int)characterInfo.HpPointLeveled,
            _ => throw new AggregateException("잘못된 타입 들어옴") 
        };
    }
}