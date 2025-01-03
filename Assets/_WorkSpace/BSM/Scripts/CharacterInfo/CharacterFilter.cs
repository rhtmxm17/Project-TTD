using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterFilter : MonoBehaviour
{
    [HideInInspector] public List<CharacterInfo> _filterCharacterInfos;
    [HideInInspector] public List<int> _elementInfosIndex = new List<int>();
    [HideInInspector] public List<ElementType> _elementFilterTypes = new List<ElementType>();
    [HideInInspector] public List<RoleType> _roleFiterTypes = new List<RoleType>();
    [HideInInspector] public List<DragonVeinType> _dragonVeinFilterTypes = new List<DragonVeinType>();
    [HideInInspector] public List<Image> _buttonColors = new List<Image>();

    private List<Enum> _filterTypes = new List<Enum>();

    private CharacterFilterUI _characterFilterUI;

    private int _elementCount = 0;
    private int _roleCount = 0;
    private int _dragonVeinCount = 0;
    
    private void Awake()
    {
        _characterFilterUI = GetComponent<CharacterFilterUI>();
    }

    private void OnEnable() => FilterReset();

    private void Start()
    {
        SubscribeFilterEvent();
    }

    private void SubscribeFilterEvent()
    {
        _characterFilterUI._fireFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.FIRE, _characterFilterUI._fireFilterButton));
        _characterFilterUI._waterFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.WATER, _characterFilterUI._waterFilterButton));
        _characterFilterUI._grassFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.GRASS, _characterFilterUI._grassFilterButton));
        _characterFilterUI._groundFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.GROUND, _characterFilterUI._groundFilterButton));
        _characterFilterUI._electricFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.ELECTRIC, _characterFilterUI._electricFilterButton));

        _characterFilterUI._deffenseFilterButton.onClick.AddListener(() => FilterEventFunc(RoleType.DEFENDER, _characterFilterUI._deffenseFilterButton));
        _characterFilterUI._attackFilterButton.onClick.AddListener(() => FilterEventFunc(RoleType.ATTACKER, _characterFilterUI._attackFilterButton));
        _characterFilterUI._supportFilterButton.onClick.AddListener(() => FilterEventFunc(RoleType.SUPPORTER, _characterFilterUI._supportFilterButton));

        _characterFilterUI._singleFilterButton.onClick.AddListener(() => FilterEventFunc(DragonVeinType.SINGLE, _characterFilterUI._singleFilterButton));
        _characterFilterUI._multiFilterButton.onClick.AddListener(() => FilterEventFunc(DragonVeinType.MULTI, _characterFilterUI._multiFilterButton));

        _characterFilterUI._allFilterButton.onClick.AddListener(AllFilterClear);
    }

    private void FilterEventFunc(Enum type, Button button)
    {
        if (_filterTypes.Contains(type))
        {
            Image buttonBlock = button.GetComponent<Image>();
            buttonBlock.color = Color.white;

            if (_buttonColors.Contains(buttonBlock))
            {
                _buttonColors.Remove(buttonBlock);
            }

            CharacterListFilterClear(type);
        }
        else
        {
            Image buttonBlock = button.GetComponent<Image>();
            buttonBlock.color = Color.red;

            if (!_buttonColors.Contains(buttonBlock))
            {
                _buttonColors.Add(buttonBlock);
            }

            _filterTypes.Add(type);
            CharacterListFilter(type);
        }
    }

    /// <summary>
    /// 캐릭터 필터 설정 
    /// </summary>
    /// <param name="type">해당 캐릭터의 타입</param>
    /// <typeparam name="T">Enum형으로 해당 캐릭터의 속성, 역할군, 용맥 특성</typeparam>
    private void CharacterListFilter<T>(T type) where T : Enum
    {
        int compareType = 0;
        // 0:불 / 1:물 / 2:풀 / 3:땅 / 4:번개 

        if (type is ElementType element)
        {
            _elementFilterTypes.Add(element);
        }
        else if (type is RoleType role)
        {
            _roleFiterTypes.Add(role);
        }
        else if (type is DragonVeinType dragonVein)
        {
            _dragonVeinFilterTypes.Add(dragonVein);
        }

        _elementCount = _elementFilterTypes.Count;
        _roleCount = _roleFiterTypes.Count;
        _dragonVeinCount = _dragonVeinFilterTypes.Count;
        //1 1 0
        //1 0 1
        //0 1 1
        //1 1 1 

        //이미 필터링이 걸려있는 상태
        if (_elementInfosIndex.Count > 0)
        {
            if (_elementCount > 0 && _roleCount == 0 && _dragonVeinCount == 0)
            {
                for (int i = 0; i < _elementInfosIndex.Count; i++)
                { 
                    if (_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.type))
                    {
                        _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
                    } 
                } 
            }

            if (_elementCount == 0 && _roleCount > 0 && _dragonVeinCount == 0)
            {
                for (int i = 0; i < _elementInfosIndex.Count; i++)
                {
                    if (_roleFiterTypes.Contains((RoleType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.roleType))
                    {
                        _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
                    }
                } 
            }

            if (_elementCount == 0 && _roleCount == 0 && _dragonVeinCount > 0)
            {
                for (int i = 0; i < _elementInfosIndex.Count; i++)
                {
                    if (_dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.dragonVeinType))
                    {
                        _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
                    }
                } 
            }
            
            
            //추가 필터링 적용 
            if (_elementCount > 0 && _roleCount > 0 && _dragonVeinCount == 0)
            {
                for (int i = 0; i < _elementInfosIndex.Count; i++)
                {
                    if (_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.type)
                        && _roleFiterTypes.Contains((RoleType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable .roleType))
                    {
                        _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
                    }
                }

                for (int i = 0; i < _filterCharacterInfos.Count; i++)
                { 
                    if (_filterCharacterInfos[i].gameObject.activeSelf)
                    {
                        if (!_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[i]._CharacterData.StatusTable.type)
                             || !_roleFiterTypes.Contains((RoleType)_filterCharacterInfos[i]._CharacterData.StatusTable.roleType))
                        {
                            _filterCharacterInfos[i].gameObject.SetActive(false);
                            _elementInfosIndex.Add(i);
                        }
                    } 
                }
                
            }
            else if (_elementCount > 0 && _roleCount == 0 && _dragonVeinCount > 0)
            {
                for (int i = 0; i < _elementInfosIndex.Count; i++)
                {
                    if (_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.type)
                        && _dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.dragonVeinType))
                    {
                        _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
                    } 
                }
                
                for (int i = 0; i < _filterCharacterInfos.Count; i++)
                { 
                    if (_filterCharacterInfos[i].gameObject.activeSelf)
                    {
                        if (!_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[i]._CharacterData.StatusTable.type)
                            || !_dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[i]._CharacterData.StatusTable.dragonVeinType))
                        {
                            _filterCharacterInfos[i].gameObject.SetActive(false);
                            _elementInfosIndex.Add(i);
                        }
                    } 
                }
                
            }
            else if (_elementCount == 0 && _roleCount > 0 && _dragonVeinCount > 0)
            {
                for (int i = 0; i < _elementInfosIndex.Count; i++)
                {
                    if (_roleFiterTypes.Contains((RoleType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.roleType)
                        && _dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.dragonVeinType))
                    {
                        _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
                    }
                }
                
                for (int i = 0; i < _filterCharacterInfos.Count; i++)
                { 
                    if (_filterCharacterInfos[i].gameObject.activeSelf)
                    {
                        if (!_roleFiterTypes.Contains((RoleType)_filterCharacterInfos[i]._CharacterData.StatusTable.roleType)
                            || !_dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[i]._CharacterData.StatusTable.dragonVeinType))
                        {
                            _filterCharacterInfos[i].gameObject.SetActive(false);
                            _elementInfosIndex.Add(i);
                        }
                    } 
                } 
            }
            else if (_elementCount > 0 && _roleCount > 0 && _dragonVeinCount > 0)
            {
                for (int i = 0; i < _elementInfosIndex.Count; i++)
                {
                    if (_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.type)
                        && _roleFiterTypes.Contains((RoleType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.roleType)
                        && _dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.dragonVeinType))
                    {
                        _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
                    } 
                }
                
                for (int i = 0; i < _filterCharacterInfos.Count; i++)
                { 
                    if (_filterCharacterInfos[i].gameObject.activeSelf)
                    {
                        if (!_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[i]._CharacterData.StatusTable.type)
                            || !_roleFiterTypes.Contains((RoleType)_filterCharacterInfos[i]._CharacterData.StatusTable.roleType)
                            || !_dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[i]._CharacterData.StatusTable.dragonVeinType))
                        {
                            _filterCharacterInfos[i].gameObject.SetActive(false);
                            _elementInfosIndex.Add(i);
                        }
                    } 
                }
            }
        }
        //처음 필터링 거는 상태
        else
        {
            if (type is ElementType elementType)
            {
                int elementValue = Convert.ToInt32(elementType);

                _elementInfosIndex = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.type != elementValue)
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();
            }
            else if (type is RoleType roleType)
            {
                int roleValue = Convert.ToInt32(roleType);

                _elementInfosIndex = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.roleType != roleValue)
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();
            }
            else if (type is DragonVeinType dragonVeinType)
            {
                int dragonVeinValue = Convert.ToInt32(dragonVeinType);

                _elementInfosIndex = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.dragonVeinType != dragonVeinValue)
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();
            }

            for (int i = 0; i < _elementInfosIndex.Count; i++)
            {
                _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 필터 해제
    /// </summary>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    private void CharacterListFilterClear<T>(T type) where T : Enum
    {
        _filterTypes.Remove(type);

        //필터 제거했을 때 마지막이 아닌 상태
        if (_filterTypes.Count > 0)
        {
            //필터가 1개 남아있는 상태이므로 그 타입을 제외한 해제한 타입은 필터링 진행

            if (type is ElementType elementType)
            {
                _elementFilterTypes.Remove(elementType);

                int[] tempArr = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.type == Convert.ToInt32(elementType))
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToArray();
               
                for (int i = 0; i < tempArr.Length; i++)
                {
                    if (_elementFilterTypes.Count > 0)
                    {
                        _filterCharacterInfos[tempArr[i]].gameObject.SetActive(false);
                        _elementInfosIndex.Add(tempArr[i]);
                    }
                }

                for (int j = 0; j < _elementInfosIndex.Count; j++)
                {
                    if (_elementFilterTypes.Count == 0)
                    {
                        if (_roleFiterTypes.Contains((RoleType)_filterCharacterInfos[_elementInfosIndex[j]]._CharacterData.StatusTable.roleType)
                            || _dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[_elementInfosIndex[j]]._CharacterData.StatusTable.dragonVeinType))
                        {  
                            _filterCharacterInfos[_elementInfosIndex[j]].gameObject.SetActive(true);
                            _elementInfosIndex.RemoveAt(j);
                            j--; 
                        }
                    } 
                }

            }
            else if (type is RoleType roleType)
            {
                _roleFiterTypes.Remove(roleType);

                int[] tempArr = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.roleType == Convert.ToInt32(roleType))
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToArray();

                for (int i = 0; i < tempArr.Length; i++)
                {
                    if (_roleFiterTypes.Count > 0)
                    {
                        _filterCharacterInfos[tempArr[i]].gameObject.SetActive(false);
                        _elementInfosIndex.Add(tempArr[i]);
                    }
                }

                for(int j = 0; j < _elementInfosIndex.Count; j++)
                {
                    if (_roleFiterTypes.Count == 0)
                    {
                        if (_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[_elementInfosIndex[j]]._CharacterData.StatusTable.type) 
                            || _dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[_elementInfosIndex[j]]._CharacterData.StatusTable.dragonVeinType))
                        {
                            _filterCharacterInfos[_elementInfosIndex[j]].gameObject.SetActive(true);
                            _elementInfosIndex.RemoveAt(j);
                            j--;
                        }
                    } 
                } 
            }
            else if (type is DragonVeinType dragonVeinType)
            {
                _dragonVeinFilterTypes.Remove(dragonVeinType);
                
                int[] tempArr = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.dragonVeinType == Convert.ToInt32(dragonVeinType))
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToArray();
                
                for (int i = 0; i < tempArr.Length; i++)
                {
                    if (_dragonVeinFilterTypes.Count > 0)
                    {
                        _filterCharacterInfos[tempArr[i]].gameObject.SetActive(false);
                        _elementInfosIndex.Add(tempArr[i]); 
                    }
                }

                for (int j = 0; j < _elementInfosIndex.Count; j++)
                {
                    if (_dragonVeinFilterTypes.Count == 0)
                    {
                        if (_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[_elementInfosIndex[j]]._CharacterData.StatusTable.type)
                            || _roleFiterTypes.Contains((RoleType)_filterCharacterInfos[_elementInfosIndex[j]]._CharacterData.StatusTable.roleType))
                        {
                            _filterCharacterInfos[_elementInfosIndex[j]].gameObject.SetActive(true);
                            _elementInfosIndex.RemoveAt(j);
                            j--;
                        }
                    } 
                } 
            } 
        }
        else
        {
            //모든 필터링이 해제되는 상황이므로 필터링된 애들 모두 활성화
            for (int i = 0; i < _elementInfosIndex.Count; i++)
            {
                _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
            }
            
            _elementInfosIndex.Clear();
            _elementFilterTypes.Clear();
            _roleFiterTypes.Clear();
            _dragonVeinFilterTypes.Clear();
        }
    }
    
    /// <summary>
    /// 현재 필터링되어 있는 상태 해제
    /// </summary>
    private void AllFilterClear()
    {
        if (_buttonColors.Count == 0) return;

        _buttonColors.ForEach(x => x.color = Color.white);

        for (int i = 0; i < _elementInfosIndex.Count; i++)
        {
            _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
        }

        _buttonColors.Clear();
        _filterTypes.Clear();
        _elementInfosIndex.Clear();
        _elementFilterTypes.Clear();
        _roleFiterTypes.Clear();
        _dragonVeinFilterTypes.Clear();
    }
    
    
    /// <summary>
    /// 캐릭터 탭 진입 시 필터 초기화
    /// </summary>
    private void FilterReset()
    {
        if (_buttonColors.Count == 0) return; 
        _buttonColors.ForEach(x => x.color = Color.white);

        for (int i = 0; i < _filterCharacterInfos.Count; i++)
        {
            _filterCharacterInfos[i].gameObject.SetActive(true);
        }
        
        _buttonColors.Clear();
        _filterTypes.Clear();
        _elementInfosIndex.Clear();
        _elementFilterTypes.Clear();
        _roleFiterTypes.Clear();
        _dragonVeinFilterTypes.Clear();
    }
    
}