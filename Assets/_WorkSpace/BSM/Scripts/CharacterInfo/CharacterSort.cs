using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


public class CharacterSort : MonoBehaviour
{
    [HideInInspector] public CharacterInfoController CharacterInfoController;
    [HideInInspector] public List<CharacterInfo> _sortCharacterInfos;

    private List<int> _sortList;

    private CharacterSortUI _characterSortUI;
    [FormerlySerializedAs("OrderText")] public TextMeshProUGUI SortingText;
    
    private int _sortValue;
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
        if (_sortList != null && _sortList.Count > 0) _sortList.Clear();

        switch (_curSortType)
        {
            case SortType.LEVEL:
                _sortList = _sortCharacterInfos.Select(x => x.CharacterLevel).ToList();
                break;
            case SortType.POWERLEVEL:
                _sortList = _sortCharacterInfos.Select(x => x.PowerLevel).ToList();
                break;
            case SortType.ENHANCELEVEL:
                _sortList = _sortCharacterInfos.Select(x => x._CharacterData.Enhancement.Value).ToList();
                break;
            case SortType.OFFENSIVEPOWER:
                _sortList = _sortCharacterInfos.Select(x => (int)x._CharacterData.AttackPointLeveled).ToList();
                break;
            case SortType.DEFENSEIVEPOWER:
                _sortList = _sortCharacterInfos.Select(x => (int)x._CharacterData.DefensePointLeveled).ToList();
                break;
            case SortType.HEALTH:
                _sortList = _sortCharacterInfos.Select(x => (int)x._CharacterData.HpPointLeveled).ToList();
                break;
        }

        //TRUE : 내림차순, FALSE : 오름차순
        if (_isSorting)
        {
            _sortList.Sort();
            _sortList.Reverse();
        }
        else
        {
            _sortList.Sort();
        }

        SortingText.text = _isSorting ? "↓" : "↑";
        
        if (!_isStart)
        {
            _isStart = true;
            StartSort();
        }

        for (int i = 0; i < _sortList.Count; i++)
        {
            for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
            {
                if (_sortList[i].Equals(GetSortValue(_sortCharacterInfos[j])))
                {
                    CharacterData newData = _sortCharacterInfos[j]._CharacterData;
                    CharacterData oldData = _sortCharacterInfos[i]._CharacterData;

                    _sortCharacterInfos[i]._CharacterData = newData;
                    _sortCharacterInfos[j]._CharacterData = oldData;

                    _sortCharacterInfos[i].SetListNameText(newData.Name);
                    _sortCharacterInfos[j].SetListNameText(oldData.Name);

                    int newType = (int)newData.StatusTable.type;
                    int oldType = (int)oldData.StatusTable.type;

                    int temp = _sortCharacterInfos[i].PowerLevel;
                    _sortCharacterInfos[i].PowerLevel = _sortCharacterInfos[j].PowerLevel;
                    _sortCharacterInfos[j].PowerLevel = temp;

                    _sortCharacterInfos[i].SetListTypeText(((ElementType)newType).ToString());
                    _sortCharacterInfos[j].SetListTypeText(((ElementType)oldType).ToString());

                    _sortCharacterInfos[i].SetListImage(newData.FaceIconSprite);
                    _sortCharacterInfos[j].SetListImage(oldData.FaceIconSprite);
                    break;
                }
            }
        }

        CharacterInfoController.SortButtonText.text = _curSortType.ToString();
        PlayerPrefs.SetInt("SortType", (int)_curSortType);
    }

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
        _sortValue = _curSortType switch
        {
            SortType.LEVEL => characterInfo.CharacterLevel,
            SortType.POWERLEVEL => characterInfo.PowerLevel,
            SortType.ENHANCELEVEL => characterInfo._CharacterData.Enhancement.Value,
            SortType.OFFENSIVEPOWER => (int)characterInfo._CharacterData.AttackPointLeveled,
            SortType.DEFENSEIVEPOWER => (int)characterInfo._CharacterData.DefensePointLeveled,
            SortType.HEALTH => (int)characterInfo._CharacterData.HpPointLeveled,
            _ => throw new AggregateException("잘못된 타입 들어옴") 
        };
        
        return _sortValue;
    }
}