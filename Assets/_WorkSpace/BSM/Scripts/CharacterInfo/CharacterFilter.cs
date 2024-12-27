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

    public List<FilterType> _characterFilterTypes = new List<FilterType>();

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

        _characterFilterUI._allFilterButton.onClick.AddListener(AllFilterClear);
    }

    /// <summary>
    /// 필터 이벤트 실행할 함수
    /// </summary>
    /// <param name="filterType"></param>
    private void FilterEventFunc(FilterType filterType)
    {
        //0:불 / 1:물 / 2:풀 / 3:땅 / 4:번개 
        if (_characterFilterTypes.Contains(filterType))
        {
            //필터 해제
            CharacterListFilterClear(filterType);
            
        }
        else
        {
            //필터 진행
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
        int temp = 0;
        
        //이미 필터링이 걸려있는 상태
        if (_infosIndex.Count != 0)
        {
            //추가 필터링 적용
            for (int i = 0; i < _infosIndex.Count; i++)
            {
                temp = (int)_filterCharacterInfos[_infosIndex[i]]._CharacterData.StatusTable.type;
                
                if (temp == (int)filterType)
                {
                    _filterCharacterInfos[_infosIndex[i]].gameObject.SetActive(true);
                    _infosIndex.RemoveAt(i);
                    i--;
                }
            }
        }
        //처음 필터링 거는 상태
        else
        { 
            //현재 타입 외 모든 타입들 필터링 진행
            _infosIndex = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type != (int)filterType)
                .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();

            for (int i = 0; i < _infosIndex.Count; i++)
            {
                _filterCharacterInfos[_infosIndex[i]].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 캐릭터 필터 해제
    /// </summary>
    /// <param name="filterType"></param>
    private void CharacterListFilterClear(FilterType filterType)
    {
        _characterFilterTypes.Remove(filterType);
        
        //필터 제거했을 때 마지막이 아닌 상태
        if (_characterFilterTypes.Count > 0)
        {
            //필터가 1개 남아있는 상태이므로 그 타입을 제외한 해제한 타입은 필터링 진행
            int[] tempArr = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type == (int)filterType)
                .Select(x => _filterCharacterInfos.IndexOf(x)).ToArray();

            for (int i = 0; i < tempArr.Length; i++)
            {
                _filterCharacterInfos[tempArr[i]].gameObject.SetActive(false);
                _infosIndex.Add(tempArr[i]);
            } 
        }
        else
        {
            //모든 필터링이 해제되는 상황이므로 필터링된 애들 모두 활성화
            for (int i = 0; i < _infosIndex.Count; i++)
            {
                _filterCharacterInfos[_infosIndex[i]].gameObject.SetActive(true);
            }

            _infosIndex.Clear();
        } 
    }
    
    /// <summary>
    /// 현재 필터링되어 있는 상태 해제
    /// </summary>
    private void AllFilterClear()
    {
        if (_infosIndex.Count == 0) return;

        for (int i = 0; i < _infosIndex.Count; i++)
        {
            _filterCharacterInfos[_infosIndex[i]].gameObject.SetActive(true);
        }

        _characterFilterTypes.Clear();
        _infosIndex.Clear();
    }
}