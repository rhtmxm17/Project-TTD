using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFilter : MonoBehaviour
{
    [HideInInspector] public List<CharacterInfo> _filterCharacterInfos;
    [HideInInspector] public List<int> _filterInfosIndex = new List<int>();
    [HideInInspector] public List<ElementType> _elementFilterTypes = new List<ElementType>();
    [HideInInspector] public List<RoleType> _roleFilterTypes = new List<RoleType>();
    [HideInInspector] public List<DragonVeinType> _dragonVeinFilterTypes = new List<DragonVeinType>();
    [HideInInspector] public List<Image> _buttonColors = new List<Image>();
    [HideInInspector] public CharacterInfoController CharacterController;
    private List<Enum> _filterTypes = new List<Enum>();

    private CharacterFilterUI _characterFilterUI; 
   
    
    private int _elementCount = 0;
    private int _roleCount = 0;
    private int _dragonVeinCount = 0;

    private void Awake()
    {
        _characterFilterUI = GetComponent<CharacterFilterUI>();
    }

    private void OnEnable() => AllFilterClear();

    private void Start()
    {
        SubscribeFilterEvent();
    }

    private void SubscribeFilterEvent()
    {
        _characterFilterUI._fireFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.FIRE, _characterFilterUI._fireFilterButton));
        _characterFilterUI._waterFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.WATER, _characterFilterUI._waterFilterButton));
        _characterFilterUI._grassFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.WOOD, _characterFilterUI._grassFilterButton));
        _characterFilterUI._groundFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.EARTH, _characterFilterUI._groundFilterButton));
        _characterFilterUI._electricFilterButton.onClick.AddListener(() => FilterEventFunc(ElementType.METAL, _characterFilterUI._electricFilterButton));

        _characterFilterUI._deffenseFilterButton.onClick.AddListener(() => FilterEventFunc(RoleType.DEFENDER, _characterFilterUI._deffenseFilterButton));
        _characterFilterUI._attackFilterButton.onClick.AddListener(() => FilterEventFunc(RoleType.ATTACKER, _characterFilterUI._attackFilterButton));
        _characterFilterUI._supportFilterButton.onClick.AddListener(() => FilterEventFunc(RoleType.SUPPORTER, _characterFilterUI._supportFilterButton));

        _characterFilterUI._singleFilterButton.onClick.AddListener(() => FilterEventFunc(DragonVeinType.SINGLE, _characterFilterUI._singleFilterButton));
        _characterFilterUI._multiFilterButton.onClick.AddListener(() => FilterEventFunc(DragonVeinType.MULTI, _characterFilterUI._multiFilterButton));

        _characterFilterUI._elementAllFilterButton.onClick.AddListener(() => AllFilterClear(ElementType.NONE));
        _characterFilterUI._roleAllFilterButton.onClick.AddListener(() => AllFilterClear(RoleType.NONE));
        _characterFilterUI._dragonAllFilterButton.onClick.AddListener(() => AllFilterClear(DragonVeinType.NONE));
        
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
    /// 속성 개수 1 이상, 역할군 개수 0, 용맥 개수 0
    /// </summary>
    private bool IsFilterConditionUEE() => _elementFilterTypes.Count > 0 && _roleFilterTypes.Count == 0 && _dragonVeinFilterTypes.Count == 0;
    
    /// <summary>
    /// 속성 개수 0 , 역할군 개수 1 이상, 용맥 개수 0
    /// </summary>
    private bool IsFilterConditionEUE() => _elementFilterTypes.Count == 0 && _roleFilterTypes.Count > 0 && _dragonVeinFilterTypes.Count == 0;
    
    /// <summary>
    /// 속성 개수 0 , 역할군 개수 0, 용맥 개수 1 이상
    /// </summary>
    private bool IsFilterConditionEEU() => _elementFilterTypes.Count == 0 && _roleFilterTypes.Count == 0 && _dragonVeinFilterTypes.Count > 0;
   
    /// <summary>
    /// 속성 개수 1 이상, 역할군 개수 1 이상, 용맥 개수 0
    /// </summary>
    private bool IsFilterConditionUUE() => _elementFilterTypes.Count > 0 && _roleFilterTypes.Count > 0 && _dragonVeinFilterTypes.Count == 0;
   
    /// <summary>
    /// 속성 개수 1 이상, 역할군 개수 0, 용맥 개수 1이상
    /// </summary>
    private bool IsFilterConditionUEU() => _elementFilterTypes.Count > 0 && _roleFilterTypes.Count == 0 && _dragonVeinFilterTypes.Count > 0;
    
    /// <summary>
    /// 속성 개수 0, 역할군 개수 1 이상, 용맥 개수 1이상
    /// </summary>
    private bool IsFilterConditionEUU() => _elementFilterTypes.Count == 0 && _roleFilterTypes.Count > 0 && _dragonVeinFilterTypes.Count > 0;
    
    /// <summary>
    /// 속성 개수 1 이상, 역할군 개수 1 이상, 용맥 개수 1이상
    /// </summary>
    private bool IsFilterConditionUUU() => _elementFilterTypes.Count > 0 && _roleFilterTypes.Count > 0 && _dragonVeinFilterTypes.Count > 0;

    /// <summary>
    /// 단일 조건의 캐릭터 대리자를 받아와서 필터링 On/Off 처리
    /// </summary>
    /// <param name="characterInfo"></param>
    private void FilterInfosConditionProcess(Func<CharacterInfo, bool> characterInfo, bool active)
    {
        for (int i = 0; i < _filterInfosIndex.Count; i++)
        {
            if (characterInfo(_filterCharacterInfos[_filterInfosIndex[i]]))
            {
                _filterCharacterInfos[_filterInfosIndex[i]].gameObject.SetActive(active);
            } 
        } 
    }
    
    /// <summary>
    /// 복수 조건의 캐릭터 대리자 받아와서 필터링 On/Off 처리
    /// </summary>
    /// <param name="characterInfo"></param>
    /// <param name="active"></param>
    private void FilterCharacterConditionProcess(Func<CharacterInfo, bool> characterInfo, bool active)
    {
        for (int i = 0; i < _filterCharacterInfos.Count; i++)
        {
            if (_filterCharacterInfos[i].gameObject.activeSelf)
            {
                if (characterInfo(_filterCharacterInfos[i]))
                {
                    _filterCharacterInfos[i].gameObject.SetActive(active);
                    _filterInfosIndex.Add(i);
                }
            }
        } 
    }

    private void FilterClearConditionProcess(Func<CharacterInfo, bool> characterInfo, bool active)
    {
        for (int i = 0; i < _filterInfosIndex.Count; i++)
        {
            if (characterInfo(_filterCharacterInfos[_filterInfosIndex[i]]))
            {
                _filterCharacterInfos[_filterInfosIndex[i]].gameObject.SetActive(active);
                _filterInfosIndex.RemoveAt(i);
                i--;
            } 
        } 
    }
    
    /// <summary>
    /// 캐릭터 필터 설정 
    /// </summary>
    /// <param name="type">해당 캐릭터의 타입</param>
    /// <typeparam name="T">Enum형으로 해당 캐릭터의 속성, 역할군, 용맥 특성</typeparam>
    private void CharacterListFilter<T>(T type) where T : Enum
    {
        // 0:화룡 / 1:수룡 / 2:정룡 / 3:토룡 / 4: 지룡

        if (type is ElementType element)
        {
            _elementFilterTypes.Add(element);
        }
        else if (type is RoleType role)
        {
            _roleFilterTypes.Add(role);
        }
        else if (type is DragonVeinType dragonVein)
        {
            _dragonVeinFilterTypes.Add(dragonVein);
        }  

        //이미 필터링이 걸려있는 상태
        if (_filterInfosIndex.Count > 0)
        {
            if (IsFilterConditionUEE())
            {
                FilterInfosConditionProcess(info => _elementFilterTypes.Contains(info._CharacterData.StatusTable.type), true);
            }

            if (IsFilterConditionEUE())
            {
                FilterInfosConditionProcess(info => _roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType),true);
            }

            if (IsFilterConditionEEU())
            {
                FilterInfosConditionProcess(x => _dragonVeinFilterTypes.Contains(x._CharacterData.StatusTable.dragonVeinType), true);
            }
            
            
            //추가 필터링 적용 
            if (IsFilterConditionUUE())
            {
                FilterInfosConditionProcess(info => (_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                                                     && _roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)),true);
                FilterCharacterConditionProcess(info => (!_elementFilterTypes.Contains(info._CharacterData.StatusTable.type) 
                                                         || !_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)), false);
            }
            else if (IsFilterConditionUEU())
            {
                FilterInfosConditionProcess(info => (_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                                                     && _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)),true);
                
                FilterCharacterConditionProcess(info => (!_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                    || !_dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)),false);
            }
            else if (IsFilterConditionEUU())
            {
                FilterInfosConditionProcess(info => (_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)
                    && _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)),true);
                
                FilterCharacterConditionProcess(info => (!_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)
                    || _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)),false);
            }
            else if (IsFilterConditionUUU())
            {
                for (int i = 0; i < _filterInfosIndex.Count; i++)
                {
                    if (_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[_filterInfosIndex[i]]._CharacterData.StatusTable.type)
                        && _roleFilterTypes.Contains((RoleType)_filterCharacterInfos[_filterInfosIndex[i]]._CharacterData.StatusTable.roleType)
                        && _dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[_filterInfosIndex[i]]._CharacterData.StatusTable.dragonVeinType))
                    {
                        _filterCharacterInfos[_filterInfosIndex[i]].gameObject.SetActive(true);
                    } 
                }
                
                for (int i = 0; i < _filterCharacterInfos.Count; i++)
                { 
                    if (_filterCharacterInfos[i].gameObject.activeSelf)
                    {
                        if (!_elementFilterTypes.Contains((ElementType)_filterCharacterInfos[i]._CharacterData.StatusTable.type)
                            || !_roleFilterTypes.Contains((RoleType)_filterCharacterInfos[i]._CharacterData.StatusTable.roleType)
                            || !_dragonVeinFilterTypes.Contains((DragonVeinType)_filterCharacterInfos[i]._CharacterData.StatusTable.dragonVeinType))
                        {
                            _filterCharacterInfos[i].gameObject.SetActive(false);
                            _filterInfosIndex.Add(i);
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

                _filterInfosIndex = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.type != elementValue)
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();
            }
            else if (type is RoleType roleType)
            {
                int roleValue = Convert.ToInt32(roleType);

                _filterInfosIndex = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.roleType != roleValue)
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();
            }
            else if (type is DragonVeinType dragonVeinType)
            {
                int dragonVeinValue = Convert.ToInt32(dragonVeinType);

                _filterInfosIndex = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.dragonVeinType != dragonVeinValue)
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();
            }

            for (int i = 0; i < _filterInfosIndex.Count; i++)
            {
                _filterCharacterInfos[_filterInfosIndex[i]].gameObject.SetActive(false);
            }
        }

        ChangeFilterText();
    }

    private void ChangeFilterText()
    {
        if (_elementFilterTypes.Count > 1)
        {
            CharacterController.ElementFilterText.text = "..."; 
        }
        else if (_elementFilterTypes.Count > 0)
        {
            CharacterController.ElementFilterText.text = _elementFilterTypes[0] switch
            {
                //0: 화룡, 1: 수룡, 2:정룡 ? 3: 토룡, 4: 진룡
                ElementType.FIRE => "화룡",
                ElementType.WATER => "수룡",
                ElementType.WOOD => "정룡",
                ElementType.EARTH => "토룡",
                ElementType.METAL => "진룡",
                _ => throw new AggregateException("잘못된 타입")
            };
        }
        else
        {
            CharacterController.ElementFilterText.text = "전체";
        }
        
        if (_roleFilterTypes.Count > 1)
        {
            CharacterController.RoleFilterText.text = "...";
        }
        else if(_roleFilterTypes.Count > 0)
        {  
            CharacterController.RoleFilterText.text = _roleFilterTypes[0] switch
            {
                RoleType.ATTACKER => "공격형",
                RoleType.DEFENDER => "방어형",
                RoleType.SUPPORTER => "지원형",
                _ => throw new AggregateException("잘못된 타입")
            };
        }
        else
        {
            CharacterController.RoleFilterText.text = "전체";
        }

        if (_dragonVeinFilterTypes.Count > 1)
        {
            CharacterController.DragonVeinFilterText.text = "...";
        }
        else if(_dragonVeinFilterTypes.Count > 0)
        { 
            CharacterController.DragonVeinFilterText.text = _dragonVeinFilterTypes[0] switch
            {
                DragonVeinType.SINGLE => "단일 공격",
                DragonVeinType.MULTI => "광역 공격",
                _ => throw new AggregateException("잘못된 타입")
            }; 
        }
        else
        {
            CharacterController.DragonVeinFilterText.text = "전체";
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
                        _filterInfosIndex.Add(tempArr[i]);
                    }
                }

                if (IsFilterConditionEUU())
                {
                    FilterClearConditionProcess(info => (_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)
                                                         && _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)),true);
                }
                else if (IsFilterConditionEUE())
                {
                    FilterClearConditionProcess(info => _roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType),true);
                }
                else if (IsFilterConditionEEU())
                {
                    FilterClearConditionProcess(info => _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType),true);
                }
                
            }
            else if (type is RoleType roleType)
            {
                _roleFilterTypes.Remove(roleType);

                int[] tempArr = _filterCharacterInfos
                    .Where(x => (int)x._CharacterData.StatusTable.roleType == Convert.ToInt32(roleType))
                    .Select(x => _filterCharacterInfos.IndexOf(x)).ToArray();

                for (int i = 0; i < tempArr.Length; i++)
                {
                    if (_roleFilterTypes.Count > 0)
                    {
                        _filterCharacterInfos[tempArr[i]].gameObject.SetActive(false);
                        _filterInfosIndex.Add(tempArr[i]);
                    }
                }

                if (IsFilterConditionUEU())
                {
                    FilterClearConditionProcess(info => (_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                        && _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)),true);
                }
                else if (IsFilterConditionUEE())
                {
                    FilterClearConditionProcess(info => _elementFilterTypes.Contains(info._CharacterData.StatusTable.type), true);
                }
                else if (IsFilterConditionEEU())
                {
                    FilterClearConditionProcess(info => _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType), true);
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
                        _filterInfosIndex.Add(tempArr[i]); 
                    }
                }

                if (IsFilterConditionUUE())
                {
                    FilterClearConditionProcess(info => (_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                        && _roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)),true);
                }
                else if (IsFilterConditionUEE())
                {
                    FilterClearConditionProcess(info => _elementFilterTypes.Contains(info._CharacterData.StatusTable.type),true);
                }
                else if (IsFilterConditionEUE())
                {
                    FilterClearConditionProcess(info => _roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType),true);
                } 
            } 
        }
        else
        {
            AllFilterClear(); 
        }

        ChangeFilterText();
    }
    
    /// <summary>
    /// 현재 필터링되어 있는 상태 해제
    /// </summary>
    private void AllFilterClear()
    {
        if (CharacterController == null) return;
        
        _buttonColors?.ForEach(x => x.color = Color.white);
        
        for (int i = 0; i < _filterInfosIndex.Count; i++)
        {
            _filterCharacterInfos[_filterInfosIndex[i]].gameObject.SetActive(true);
        } 
        
        _buttonColors?.Clear();
        _filterTypes.Clear();
        _filterInfosIndex.Clear();
        _elementFilterTypes.Clear();
        _roleFilterTypes.Clear();
        _dragonVeinFilterTypes.Clear();
        ChangeFilterText(); 
    }
    
    /// <summary>
    /// 전체 버튼으로 해당 필터 모두 해제
    /// </summary>
    /// <param name="type"></param>
    private void AllFilterClear(Enum type)
    {
        
        if (type is ElementType)
        {
            _elementFilterTypes.Clear(); 
            
            if (_roleFilterTypes.Count > 0 && _dragonVeinFilterTypes.Count > 0)
            {
                FilterClearConditionProcess(info => (_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)
                    && _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)),true);
            }
            else if (_roleFilterTypes.Count > 0 || _dragonVeinFilterTypes.Count > 0)
            {
                FilterClearConditionProcess(info => (_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)
                    || _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)),true);
            }
            else
            {
                Debug.Log("여기!");
                FilterClearConditionProcess(info => true ,true);
            }

            _filterTypes.Remove(ElementType.FIRE);
            _filterTypes.Remove(ElementType.WATER);
            _filterTypes.Remove(ElementType.WOOD);
            _filterTypes.Remove(ElementType.METAL);
            _filterTypes.Remove(ElementType.EARTH);

            Image img1 = _characterFilterUI._fireFilterButton.GetComponent<Image>();
            Image img2 = _characterFilterUI._waterFilterButton.GetComponent<Image>();
            Image img3 = _characterFilterUI._grassFilterButton.GetComponent<Image>();
            Image img4 = _characterFilterUI._electricFilterButton.GetComponent<Image>();
            Image img5 = _characterFilterUI._groundFilterButton.GetComponent<Image>();

            img1.color = Color.white;
            img2.color = Color.white;
            img3.color = Color.white;
            img4.color = Color.white;
            img5.color = Color.white;
             
            _buttonColors.Remove(img1);
            _buttonColors.Remove(img2);
            _buttonColors.Remove(img3);
            _buttonColors.Remove(img4);
            _buttonColors.Remove(img5);

        }
        else if (type is RoleType role)
        {
            _roleFilterTypes.Clear();
        
            if (_elementFilterTypes.Count > 0 && _dragonVeinFilterTypes.Count > 0)
            {
                FilterClearConditionProcess(info => (_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                    && _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)), true);
            }
            else if (_elementFilterTypes.Count > 0 || _dragonVeinFilterTypes.Count > 0)
            {
                FilterClearConditionProcess(info => (_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                    || _dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType)), true); 
            }
            else
            {
                FilterClearConditionProcess(info => true ,true); 
            }
            _filterTypes.Remove(RoleType.ATTACKER);
            _filterTypes.Remove(RoleType.DEFENDER);
            _filterTypes.Remove(RoleType.SUPPORTER);

            Image img1 = _characterFilterUI._attackFilterButton.GetComponent<Image>();
            Image img2 = _characterFilterUI._deffenseFilterButton.GetComponent<Image>();
            Image img3 = _characterFilterUI._supportFilterButton.GetComponent<Image>();
            
            img1.color = Color.white;
            img2.color = Color.white;
            img3.color = Color.white;
            
            _buttonColors.Remove(img1); 
            _buttonColors.Remove(img2); 
            _buttonColors.Remove(img3); 
        }
        else if (type is DragonVeinType dragonVein)
        {
            _dragonVeinFilterTypes.Clear();
        
            if (_elementFilterTypes.Count > 0 && _roleFilterTypes.Count > 0)
            {
                FilterClearConditionProcess(info => (_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                    && _roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)), true); 
            }
            else if (_elementFilterTypes.Count > 0 || _roleFilterTypes.Count > 0)
            {
                FilterClearConditionProcess(info => (_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
                    || _roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)), true);
            }
            else
            {
                FilterClearConditionProcess(info => true, true); 
            }
            _filterTypes.Remove(DragonVeinType.SINGLE);
            _filterTypes.Remove(DragonVeinType.MULTI);

            Image img1 = _characterFilterUI._singleFilterButton.GetComponent<Image>();
            Image img2 = _characterFilterUI._multiFilterButton.GetComponent<Image>();
            
            img1.color = Color.white; 
            img2.color = Color.white;  
            
            _buttonColors.Remove(img1);
            _buttonColors.Remove(img2); 
        } 
        
        ChangeFilterText(); 
    }

    public void CheckObject(CharacterInfo info, int index)
    {
        if (_filterTypes.Count == 0) return;
        
        
        if (IsFilterConditionUEE())
        { 
            if (!_elementFilterTypes.Contains(info._CharacterData.StatusTable.type))
            {
                FilterElementAdd(info.gameObject, index);
            } 
            else
            {
                FilterElementRemove(info.gameObject, index);
            } 
        }
        else if (IsFilterConditionEUE())
        {
            if (!_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType))
            {
                FilterElementAdd(info.gameObject, index);
            } 
            else
            {
                FilterElementRemove(info.gameObject, index);
            } 
        } 
        else if (IsFilterConditionEEU())
        {
            if (!_dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType))
            {
                FilterElementAdd(info.gameObject, index);
            } 
            else
            {
                FilterElementRemove(info.gameObject, index);
            } 
        }
        else if (IsFilterConditionUUE())
        {
            if(!_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
               || !_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType))
            {
                FilterElementAdd(info.gameObject, index);
            }
            else
            {
                FilterElementRemove(info.gameObject, index);
            }
        }
        else if (IsFilterConditionUEU())
        {
            if(!_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
               || !_dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType))
            {
                FilterElementAdd(info.gameObject, index);
            }
            else
            {
                FilterElementRemove(info.gameObject, index);
            }
        }
        else if (IsFilterConditionEUU())
        {
            if(!_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)
               || !_dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType))
            {
                FilterElementAdd(info.gameObject, index);
            }
            else
            {
                FilterElementRemove(info.gameObject, index);
            }
        }
        else if (_elementFilterTypes.Count > 0 && _roleFilterTypes.Count > 0 && _dragonVeinFilterTypes.Count > 0)
        {
            if(!_elementFilterTypes.Contains(info._CharacterData.StatusTable.type)
               || !_roleFilterTypes.Contains(info._CharacterData.StatusTable.roleType)
               || !_dragonVeinFilterTypes.Contains(info._CharacterData.StatusTable.dragonVeinType))
            {
                FilterElementAdd(info.gameObject, index);
            }
            else
            {
                FilterElementRemove(info.gameObject, index);
            }
        } 
    }

    private void FilterElementAdd(GameObject go, int index)
    {
        if (!_filterInfosIndex.Contains(index))
        {
            _filterInfosIndex.Add(index);
        } 
        
        go.SetActive(false);
    }

    private void FilterElementRemove(GameObject go, int index)
    {
        if (_filterInfosIndex.Contains(index))
        {
            _filterInfosIndex.Remove(index); 
        }  
        go.SetActive(true);
    }
    
}
