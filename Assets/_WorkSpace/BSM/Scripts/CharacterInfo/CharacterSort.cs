using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class CharacterSort : MonoBehaviour
{
    [HideInInspector] public CharacterInfoController CharacterInfoController;
    public List<CharacterInfo> _sortCharacterInfos;
    [HideInInspector] public TextMeshProUGUI SortingText;
   
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
    
    public List<CharacterInfo> _ownedCharacters;
    public List<CharacterInfo> _unOwnedCharacters;
    public List<int> _ownedValues;
    public List<int> _unOwnedValues;
    
    
    #region MyRegion

    
    
    /// <summary>
    /// 캐릭터 리스트 정렬 기능
    /// </summary>
    // public void CharacterListSort()
    // {
    //     if (_sortList != null && _sortList.Count > 0) _sortList.Clear();
    //     switch (_curSortType)
    //     {
    //         case SortType.LEVEL:
    //             _sortList = _sortCharacterInfos.Select(x => x.CharacterLevel).ToList();
    //             break;
    //         case SortType.POWERLEVEL:
    //             _sortList = _sortCharacterInfos.Select(x => x.PowerLevel).ToList();
    //             break;
    //         case SortType.ENHANCELEVEL:
    //             _sortList = _sortCharacterInfos.Select(x => x._CharacterData.Enhancement.Value).ToList();
    //             break;
    //         case SortType.OFFENSIVEPOWER:
    //             _sortList = _sortCharacterInfos.Select(x => (int)x._CharacterData.AttackPointLeveled).ToList();
    //             break;
    //         case SortType.DEFENSEIVEPOWER:
    //             _sortList = _sortCharacterInfos.Select(x => (int)x._CharacterData.DefensePointLeveled).ToList();
    //             break;
    //         case SortType.HEALTH:
    //             _sortList = _sortCharacterInfos.Select(x => (int)x._CharacterData.HpPointLeveled).ToList();
    //             break;
    //     }
    //
    //     //보유 캐릭터 = 전체 캐릭터 개수 - 보유하지 않은 캐릭터의 개수 까지만 반복?
    //     //보유하지 않은 캐릭터 = 보유하지 않은 캐릭터의 개수부터 정렬 진행?
    //     
    //     //TRUE : 내림차순, FALSE : 오름차순
    //     if (_isSorting)
    //     {
    //         //TODO: 정렬 구조
    //         _sortList.Sort();
    //         _sortList.Reverse(); 
    //     }
    //     else
    //     {
    //         _sortList.Sort();
    //     }
    //     
    //     //TODO: 현재 임시로 화살표 텍스트로 표시
    //     SortingText.text = _isSorting ? "↓" : "↑";
    //     
    //     //시작 시 1회 캐릭터 리스트 UI 설정
    //     if (!_isStart)
    //     {
    //         _isStart = true;
    //         StartSort();
    //     }
    //
    //     for (int i = 0; i < _sortList.Count; i++)
    //     {
    //         for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
    //         {
    //             //TODO: 고민중 -> 보유하지 않은 애들을 맨 뒤로..
    //             if (_sortList[i].Equals(GetSortValue(_sortCharacterInfos[j])))
    //             {
    //                 //if (GetSortValue(_sortCharacterInfos[i]) == GetSortValue(_sortCharacterInfos[j])) break; 
    //                  
    //                 CharacterData newData = _sortCharacterInfos[j]._CharacterData;
    //                 CharacterData oldData = _sortCharacterInfos[i]._CharacterData;
    //     
    //                 _sortCharacterInfos[i]._CharacterData = newData;
    //                 _sortCharacterInfos[j]._CharacterData = oldData;
    //     
    //                 _sortCharacterInfos[i].SetListNameText(newData.Name);
    //                 _sortCharacterInfos[j].SetListNameText(oldData.Name);
    //                 
    //                 int temp = _sortCharacterInfos[i].PowerLevel;
    //                 _sortCharacterInfos[i].PowerLevel = _sortCharacterInfos[j].PowerLevel;
    //                 _sortCharacterInfos[j].PowerLevel = temp;
    //     
    //                 _sortCharacterInfos[i].SetListTypeText((ElementType)newData.StatusTable.type);
    //                 _sortCharacterInfos[j].SetListTypeText((ElementType)oldData.StatusTable.type);
    //     
    //                 _sortCharacterInfos[i].SetListImage(newData.FaceIconSprite);
    //                 _sortCharacterInfos[j].SetListImage(oldData.FaceIconSprite);
    //                 break;
    //             }
    //         }
    //     }
    //     
    //
    //
    //     ChangeSortButtonText();
    //     PlayerPrefs.SetInt("SortType", (int)_curSortType);
    // }
#endregion
    
    public void CharacterListSort()
    {
        if (_sortList != null && _sortList.Count > 0) _sortList.Clear();
        _ownedCharacters = _sortCharacterInfos.Where(x=> GameManager.UserData.HasCharacter(x._CharacterData.Id)).ToList();
        _unOwnedCharacters = _sortCharacterInfos.Where(x => !GameManager.UserData.HasCharacter(x._CharacterData.Id)).ToList();
        
        int ownedCount = _ownedCharacters.Count;
        int unOwnedCount = _unOwnedCharacters.Count;

        if (_isSorting)
        {
            _ownedCharacters.Sort((CharacterInfo a, CharacterInfo b) => { return GetSortValue(b).CompareTo(GetSortValue(a));});
            _unOwnedCharacters.Sort((CharacterInfo a, CharacterInfo b) => { return GetSortValue(b).CompareTo(GetSortValue(a));});
        }
        else
        {
            _ownedCharacters.Sort((CharacterInfo a, CharacterInfo b) => { return GetSortValue(a).CompareTo(GetSortValue(b));});
            _unOwnedCharacters.Sort((CharacterInfo a, CharacterInfo b) => { return GetSortValue(a).CompareTo(GetSortValue(b));});
        }
        
        SortingText.text = _isSorting ? "↓" : "↑";
        
        if (!_isStart)
        {
            _isStart = true;
            StartSort();
        }
        
        Debug.Log($"보유 수:{ownedCount}");
        Debug.Log($"미보유 수:{unOwnedCount}");

        for (int i = 0; i < ownedCount; i++)
        {
            Debug.Log($"변경 전 :{_ownedCharacters[i]._CharacterData.Name}");
        }
        
        
        for (int i = 0; i < ownedCount; i++)
        { 
            CharacterData newData = _ownedCharacters[i]._CharacterData;
            _sortCharacterInfos[i]._CharacterData = newData;
            _sortCharacterInfos[i].SetListNameText(newData.Name);
            _sortCharacterInfos[i].PowerLevel = _ownedCharacters[i].PowerLevel;
            _sortCharacterInfos[i].SetListTypeText((ElementType)newData.StatusTable.type);
            _sortCharacterInfos[i].SetListImage(newData.FaceIconSprite); 
            
            for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
            {
                if (_ownedCharacters[i]._CharacterData.Id == _sortCharacterInfos[j]._CharacterData.Id)
                {
                    _sortCharacterInfos[j]._CharacterData = _ownedCharacters[i]._CharacterData;
                    _sortCharacterInfos[j].SetListNameText(_ownedCharacters[i]._CharacterData.Name);
                    _sortCharacterInfos[j].PowerLevel = _ownedCharacters[i].PowerLevel;
                    _sortCharacterInfos[j].SetListTypeText((ElementType)_ownedCharacters[i]._CharacterData.StatusTable.type);
                    _sortCharacterInfos[j].SetListImage(_ownedCharacters[i]._CharacterData.FaceIconSprite);
                    break;
                } 
            }  
        }
        
        for (int i = 0; i < ownedCount; i++)
        {
            Debug.Log($"변경 후 :{_ownedCharacters[i]._CharacterData.Name}");
        }
        
        // for (int i = 0; i < unOwnedCount; i++)
        // { 
        //     CharacterData unNewData = _unOwnedCharacters[i]._CharacterData;
        //     CharacterData unOldData = _sortCharacterInfos[i+ownedCount]._CharacterData;
        //      
        //     _sortCharacterInfos[i+ownedCount]._CharacterData = unNewData;
        //     _unOwnedCharacters[i]._CharacterData = unOldData;
        //     
        //     _sortCharacterInfos[i+ownedCount].SetListNameText(unNewData.Name);
        //     _unOwnedCharacters[i].SetListNameText(unOldData.Name);
        //
        //     int unTemp = _sortCharacterInfos[i+ownedCount].PowerLevel;
        //     _sortCharacterInfos[i+ownedCount].PowerLevel = _unOwnedCharacters[i].PowerLevel;
        //     _unOwnedCharacters[i].PowerLevel = unTemp;
        //      
        //     _sortCharacterInfos[i+ownedCount].SetListTypeText((ElementType)unNewData.StatusTable.type);
        //     _unOwnedCharacters[i].SetListTypeText((ElementType)unOldData.StatusTable.type);
        //      
        //     _sortCharacterInfos[i+ownedCount].SetListImage(unNewData.FaceIconSprite); 
        //     _unOwnedCharacters[i].SetListImage(unOldData.FaceIconSprite); 
        // }
        
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
    private int GetSortValue(CharacterInfo characterInfo)
    {
        return _curSortType switch
        {
            SortType.LEVEL => characterInfo.CharacterLevel,
            SortType.POWERLEVEL => characterInfo.PowerLevel,
            SortType.ENHANCELEVEL => characterInfo._CharacterData.Enhancement.Value,
            SortType.OFFENSIVEPOWER => (int)characterInfo._CharacterData.AttackPointLeveled,
            SortType.DEFENSEIVEPOWER => (int)characterInfo._CharacterData.DefensePointLeveled,
            SortType.HEALTH => (int)characterInfo._CharacterData.HpPointLeveled,
            _ => throw new AggregateException("잘못된 타입 들어옴") 
        };
    }
}