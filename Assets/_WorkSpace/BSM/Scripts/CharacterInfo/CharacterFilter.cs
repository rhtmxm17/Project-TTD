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
    
    [HideInInspector] public List<int> _elementInfosIndex = new List<int>();
    [HideInInspector] public List<ElementType> _elementFilterTypes = new List<ElementType>();
    
    [HideInInspector] public List<int> _roleInfosIndex = new List<int>();
    [HideInInspector] public List<RoleType> _roleFiterTypes = new List<RoleType>();
    
    [HideInInspector] public List<int> _dragonVeinInfosIndex = new List<int>();
    [HideInInspector] public List<DragonVeinType> _dragonVeinFilterTypes = new List<DragonVeinType>();
     
    [HideInInspector] public List<Image> _buttonColors = new List<Image>();

    public List<Enum> _filterTypes = new List<Enum>();
    
    
    private CharacterFilterUI _characterFilterUI;

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

    // private void FilterEventFunc(Enum type, Button button)
    // {
    //
    //     if (_filterTypes.Contains(type))
    //     {
    //         Image buttonBlock = button.GetComponent<Image>();
    //         buttonBlock.color = Color.white;
    //
    //         if (_buttonColors.Contains(buttonBlock))
    //         {
    //             _buttonColors.Remove(buttonBlock);
    //         }
    //         
    //     }
    //     else
    //     {
    //         Image buttonBlock = button.GetComponent<Image>();
    //         buttonBlock.color = Color.red;
    //
    //         if (!_buttonColors.Contains(buttonBlock))
    //         {
    //             _buttonColors.Add(buttonBlock);
    //         }
    //         
    //         _filterTypes.Add(type);
    //         CharacterListFilter(type);
    //     }
    //     
    //     
    // }
    //
    // private ElementType element;
    // private RoleType role;
    // private DragonVeinType dragonVein;
    //
    // private void CharacterListFilter(Enum type)
    // {
    //     int tempType = 0;
    //     
    //     switch (type)
    //     {
    //         case ElementType :
    //             element = (ElementType)type;
    //             break; 
    //         
    //         case RoleType :
    //             role = (RoleType)type;
    //             break;
    //         
    //         case DragonVeinType :
    //             dragonVein = (DragonVeinType)type;
    //             break;
    //     }
    //     
    //     
    //     
    //     //이미 필터링이 걸려있는 상태
    //     if (_elementInfosIndex.Count > 0)
    //     {
    //         //추가 필터링 적용
    //         for (int i = 0; i < _elementInfosIndex.Count; i++)
    //         {
    //             tempType = (int)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.type;
    //
    //             if (tempType == (int)type)
    //             {
    //                 _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
    //                 _elementInfosIndex.RemoveAt(i);
    //                 i--;
    //             }
    //         }
    //     }
    //     //처음 필터링 거는 상태
    //     else
    //     { 
    //         //현재 타입 외 모든 타입들 필터링 진행
    //         _elementInfosIndex = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type != (int)type)
    //             .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();
    //
    //         for (int i = 0; i < _elementInfosIndex.Count; i++)
    //         {
    //             _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(false);
    //         }
    //     }
    // }
    
    
    
    /// <summary>
    /// 필터 이벤트 실행할 함수
    /// </summary>
    /// <param name="elementType"></param>
     private void FilterEventFunc(ElementType elementType, Button button)
     {
         // 0:불 / 1:물 / 2:풀 / 3:땅 / 4:번개 
         if (_elementFilterTypes.Contains(elementType))
         {
             //필터 해제
             CharacterListFilterClear(elementType); 
             
             Image colorblock = button.GetComponent<Image>();
             colorblock.color = Color.white;
        
             if (_buttonColors.Contains(colorblock))
             {
                 _buttonColors.Remove(colorblock);
             }
         }
         else
         {
             //필터 진행
             Image colorblock = button.GetComponent<Image>();
             colorblock.color = Color.red;
             if (!_buttonColors.Contains(colorblock))
             {
                 _buttonColors.Add(colorblock);
             }
        
             _elementFilterTypes.Add(elementType);
             CharacterListFilter(elementType);
         }
     }

     private void FilterEventFunc(RoleType roleType, Button button)
     {
         if (_roleFiterTypes.Contains(roleType))
         { 
             Image colorBlock = button.GetComponent<Image>();
             colorBlock.color = Color.white;
    
             if (_buttonColors.Contains(colorBlock))
             {
                 _buttonColors.Remove(colorBlock);
             }
             
         }
         else
         {
             Image colorBlock = button.GetComponent<Image>();
             colorBlock.color = Color.red;
    
             if (!_buttonColors.Contains(colorBlock))
             {
                 _buttonColors.Add(colorBlock);
             } 
         } 
     }
    
     private void FilterEventFunc(DragonVeinType dargonVeinType, Button button)
     {
    
         if (_dragonVeinFilterTypes.Contains(dargonVeinType))
         {
             Image buttonBlock = button.GetComponent<Image>();
             buttonBlock.color = Color.white;
    
             if (_buttonColors.Contains(buttonBlock))
             {
                 _buttonColors.Remove(buttonBlock);
             }
             
         }
         else
         {
             
             Image buttonBlock = button.GetComponent<Image>();
             buttonBlock.color = Color.red;
    
             if (!_buttonColors.Contains(buttonBlock))
             {
                 _buttonColors.Add(buttonBlock);
             }
             
         }
         
     }
    
    //필터 걸었을 때 전부 Active False 시키고, 그 중 현재 걸려있는 필터들의 속성에 해당하는 애들은 다시 Active True?
     
    //IF elementFilter.Count > 0 || roleFilter.Count > 0 || dragonVeinFilter.Count > 0
        // 현재 Active False 인 애들 중 새로 등록된 필터의 속성과 같은 애들을 확인
            // 같은 애들이 발견되면 그 애들은 해당 필터의 리스트에 등록
                    
            
            
    // 걸어둔 필터가 2개인데 2개다 같은 애들이 있다? 이 경우에는 하나가 꺼져도 상관없기 때문에 Pass
                
    

    
    /// <summary>
    /// 캐릭터 필터 설정
    /// </summary>
    /// <param name="elementType"></param>
    private void CharacterListFilter(ElementType elementType)
    {
        int temp = 0;

        //이미 필터링이 걸려있는 상태
        if (_elementInfosIndex.Count != 0)
        {
            //추가 필터링 적용
            for (int i = 0; i < _elementInfosIndex.Count; i++)
            {
                temp = (int)_filterCharacterInfos[_elementInfosIndex[i]]._CharacterData.StatusTable.type;

                if (temp == (int)elementType)
                {
                    _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(true);
                    _elementInfosIndex.RemoveAt(i);
                    i--;
                }
            }
        }
        //처음 필터링 거는 상태
        else
        {
            //현재 타입 외 모든 타입들 필터링 진행
            _elementInfosIndex = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type != (int)elementType)
                .Select(x => _filterCharacterInfos.IndexOf(x)).ToList();

            for (int i = 0; i < _elementInfosIndex.Count; i++)
            {
                _filterCharacterInfos[_elementInfosIndex[i]].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 캐릭터 필터 해제
    /// </summary>
    /// <param name="elementType"></param>
    private void CharacterListFilterClear(ElementType elementType)
    {
        _elementFilterTypes.Remove(elementType);

        //필터 제거했을 때 마지막이 아닌 상태
        if (_elementFilterTypes.Count > 0)
        {
            //필터가 1개 남아있는 상태이므로 그 타입을 제외한 해제한 타입은 필터링 진행
            int[] tempArr = _filterCharacterInfos.Where(x => (int)x._CharacterData.StatusTable.type == (int)elementType)
                .Select(x => _filterCharacterInfos.IndexOf(x)).ToArray();

            for (int i = 0; i < tempArr.Length; i++)
            {
                _filterCharacterInfos[tempArr[i]].gameObject.SetActive(false);
                _elementInfosIndex.Add(tempArr[i]);
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
        _elementFilterTypes.Clear();
        _elementInfosIndex.Clear();
    }
}