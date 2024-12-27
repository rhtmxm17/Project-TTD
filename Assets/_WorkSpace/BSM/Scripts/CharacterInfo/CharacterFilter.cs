using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterFilter : MonoBehaviour
{
    [HideInInspector] public List<CharacterInfo> _filterCharacterInfos;
    
    public List<int> _infosIndex = new List<int>();
    
    private CharacterFilterUI _characterFilterUI;

    private List<FilterType> _characterFilterTypes = new List<FilterType>();

    private void Awake()
    {
        _characterFilterUI = GetComponent<CharacterFilterUI>();
    }

    private void Start()
    {
        SubscribeFilterEvent();
    }

    private void SubscribeFilterEvent()
    {
        _characterFilterUI._fireFilterButton.onClick.AddListener(() => FilterEventFunc(FilterType.FIRE));
        _characterFilterUI._waterFilterButton.onClick.AddListener(() => FilterEventFunc(FilterType.WATER));
        _characterFilterUI._grassFilterButton.onClick.AddListener(() => FilterEventFunc(FilterType.GRASS));
        _characterFilterUI._groundFilterButton.onClick.AddListener(() => FilterEventFunc(FilterType.GROUND));
        _characterFilterUI._electricFilterButton.onClick.AddListener(() => FilterEventFunc(FilterType.ELECTRIC));
    }

    private void FilterEventFunc(FilterType filterType)
    {
        //0:불 / 1:물 / 2:풀 / 3:땅 / 4:번개 
        if (_characterFilterTypes.Contains(filterType))
        {
            //필터 해제
            Debug.Log($"{filterType} 필터 해제");
            CharacterListFilterClear(filterType);
            _characterFilterTypes.Remove(filterType);
        }
        else
        {
            //필터 진행
            Debug.Log($"{filterType} 필터 설정");
            _characterFilterTypes.Add(filterType);
            CharacterListFilter(filterType);
        }
    }

    /// <summary>
    /// 캐릭터 필터 설정
    /// </summary>
    /// <param name="filterType"></param>
    private void CharacterListFilter(FilterType filterType)
    { 
        //infosIndex count가 0과 같지 않다면 추가 필터는 add로 등록?
        //불 타입으로 필터가 켜져있는 상태 -> 물 타입 추가 설정
        //그러면 infosIndex에 등록되어 있는 애들 중 물타입 애들의 인덱스를 구해서 켜주고
        //빠져나온 애들의 인덱스는 infosIndex 에서 Remove를 시켜준다?
        
        _infosIndex = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type != (int)filterType)
            .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();

        for (int i = 0; i < _infosIndex.Count; i++)
        {
            _filterCharacterInfos[_infosIndex[i]].gameObject.SetActive(false);
        } 
    }

    /// <summary>
    /// 캐릭터 필터 해제
    /// </summary>
    /// <param name="filterType"></param>
    private void CharacterListFilterClear(FilterType filterType)
    {
        _infosIndex = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type != (int)filterType)
            .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();

        for (int i = 0; i < _infosIndex.Count; i++)
        {
            _filterCharacterInfos[_infosIndex[i]].gameObject.SetActive(true);
        }
        
        _infosIndex.Clear();
    }
}