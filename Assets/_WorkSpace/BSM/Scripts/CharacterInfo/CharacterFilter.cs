using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterFilter : MonoBehaviour
{
    [HideInInspector] public List<CharacterInfo> _filterCharacterInfos;

    public List<int> _infosIndex = new List<int>();

    private CharacterFilterUI _characterFilterUI;

    public List<ElementType> _characterFilterTypes = new List<ElementType>();
    private List<Image> _buttonColors = new List<Image>();
    
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
        _characterFilterUI._fireFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.FIRE, _characterFilterUI._fireFilterButton));
        _characterFilterUI._waterFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.WATER,_characterFilterUI._waterFilterButton));
        _characterFilterUI._grassFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.GRASS,_characterFilterUI._grassFilterButton));
        _characterFilterUI._groundFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.GROUND, _characterFilterUI._groundFilterButton));
        _characterFilterUI._electricFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.ELECTRIC, _characterFilterUI._electricFilterButton));
        
        //TODO: 역할군에 대한 버튼들 이벤트 등록 필요
        //
        
        _characterFilterUI._allFilterButton.onClick.AddListener(AllFilterClear);
    }

    /// <summary>
    /// 필터 이벤트 실행할 함수
    /// </summary>
    /// <param name="elementType"></param>
    private void FilterEventFunc(ElementType elementType,Button button)
    {
        //0:불 / 1:물 / 2:풀 / 3:땅 / 4:번개 
        if (_characterFilterTypes.Contains(elementType))
        {
            //필터 해제
            CharacterListFilterClear(elementType);

            Image colorblock = button.GetComponent<Image>();
            colorblock.color = Color.white;
            
        }
        else
        {
            //필터 진행
            Image colorblock = button.GetComponent<Image>();
            colorblock.color = Color.red;
            _buttonColors.Add(colorblock);
            
            _characterFilterTypes.Add(elementType);
            CharacterListFilter(elementType);
        }
    }

    /// <summary>
    /// 캐릭터 필터 설정
    /// </summary>
    /// <param name="elementType"></param>
    private void CharacterListFilter(ElementType elementType)
    {
        int temp = 0;
        
        //이미 필터링이 걸려있는 상태
        if (_infosIndex.Count != 0)
        {
            //추가 필터링 적용
            for (int i = 0; i < _infosIndex.Count; i++)
            {
                temp = (int)_filterCharacterInfos[_infosIndex[i]]._CharacterData.StatusTable.type;
                
                if (temp == (int)elementType)
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
            _infosIndex = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type != (int)elementType)
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
    /// <param name="elementType"></param>
    private void CharacterListFilterClear(ElementType elementType)
    {
        _characterFilterTypes.Remove(elementType);
        
        //필터 제거했을 때 마지막이 아닌 상태
        if (_characterFilterTypes.Count > 0)
        {
            //필터가 1개 남아있는 상태이므로 그 타입을 제외한 해제한 타입은 필터링 진행
            int[] tempArr = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type == (int)elementType)
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
        if (_buttonColors.Count != 0)
        {
            _buttonColors.ForEach(x=> x.color = Color.white);
        }            
        
        for (int i = 0; i < _infosIndex.Count; i++)
        {
            _filterCharacterInfos[_infosIndex[i]].gameObject.SetActive(true);
        }
        
        _buttonColors.Clear();
        _characterFilterTypes.Clear();
        _infosIndex.Clear();
    }
}