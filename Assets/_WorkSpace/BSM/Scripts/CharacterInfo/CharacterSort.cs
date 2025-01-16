using System; 
using System.Collections.Generic;
using System.Linq;
using TMPro; 
using UnityEngine; 


public class CharacterSort : MonoBehaviour
{
    [HideInInspector] public CharacterInfoController CharacterInfoController;
    [HideInInspector] public List<CharacterInfo> _sortCharacterInfos;
    [HideInInspector] public TextMeshProUGUI SortingText;
   
    [HideInInspector] public List<CharacterData> _ownedCharacters;
    [HideInInspector] public List<CharacterData> _unOwnedCharacters;
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
        if(_ownedCharacters.Count > 0) _ownedCharacters.Clear();
        if(_unOwnedCharacters.Count > 0) _unOwnedCharacters.Clear();
        
       SortingText.text = _isSorting ? "↓" : "↑";
       
       if (!_isStart)
       {
           _isStart = true;
           StartSort();
       }
  
       for (int i = 0; i < _sortCharacterInfos.Count; i++)
       {
           if (!GameManager.UserData.HasCharacter(_sortCharacterInfos[i]._CharacterData.Id) && _sortCharacterInfos[i].gameObject.activeSelf)
           {
               for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
               {
                   if (GameManager.UserData.HasCharacter(_sortCharacterInfos[j]._CharacterData.Id)&& _sortCharacterInfos[j].gameObject.activeSelf)
                   {
                       CharacterData unOwnedData = _sortCharacterInfos[i]._CharacterData;
                       CharacterData ownedData = _sortCharacterInfos[j]._CharacterData;
                        
                       _sortCharacterInfos[i]._CharacterData = ownedData;
                       _sortCharacterInfos[j]._CharacterData = unOwnedData;
                       
                       _sortCharacterInfos[i].SetListNameText(ownedData.Name);
                       _sortCharacterInfos[j].SetListNameText(unOwnedData.Name);
                       
                       _sortCharacterInfos[i].PowerLevel = (int)ownedData.PowerLevel;
                       _sortCharacterInfos[j].PowerLevel = (int)unOwnedData.PowerLevel;
                       
                       _sortCharacterInfos[i].SetListLevelText(ownedData.Level.Value);
                       _sortCharacterInfos[j].SetListLevelText(unOwnedData.Level.Value);
                       
                       _sortCharacterInfos[i].SetListImage(ownedData.FaceIconSprite);
                       _sortCharacterInfos[j].SetListImage(unOwnedData.FaceIconSprite);
                       break;
                   }
               }
           } 
       }

        
        _ownedCharacters = _sortCharacterInfos
            .Where(x=> GameManager.UserData.HasCharacter(x._CharacterData.Id))
            .Select(x => x._CharacterData).ToList();
        _unOwnedCharacters = _sortCharacterInfos
            .Where(x => !GameManager.UserData.HasCharacter(x._CharacterData.Id))
            .Select(x => x._CharacterData).ToList();
        
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

        CharacterSortResult(_ownedCharacters, 0, ownedCount, false);
        CharacterSortResult(_unOwnedCharacters, ownedCount, unOwnedCount, true);
        
        ChangeSortButtonText();
        PlayerPrefs.SetInt("SortType", (int)_curSortType);
    }
    
    /// <summary>
    /// 정렬 후 캐릭터 UI 셋팅
    /// </summary>
    /// <param name="characterInfos">가지고 있는 캐릭터 데이터</param>
    /// <param name="ownedCount">보유, 미보유 캐릭터 개수</param>
    /// <param name="index">배열의 크기</param>
    /// <param name="active">활성화, 비활성화 여부</param>
    private void CharacterSortResult(List<CharacterData> characterInfos, int ownedCount, int index, bool active)
    {
        for (int i = 0; i < index; i++)
        {
            _sortCharacterInfos[i + ownedCount]._CharacterData = characterInfos[i];
            _sortCharacterInfos[i + ownedCount].SetListNameText(characterInfos[i].Name);
            _sortCharacterInfos[i + ownedCount].PowerLevel = (int)characterInfos[i].PowerLevel;
            _sortCharacterInfos[i + ownedCount].SetListLevelText(characterInfos[i].Level.Value);
            _sortCharacterInfos[i + ownedCount].SetListImage(characterInfos[i].FaceIconSprite);
            _sortCharacterInfos[i + ownedCount].OwnedObject.SetActive(active);
            CharacterInfoController._characterFilter.CheckObject(_sortCharacterInfos[i + ownedCount], i + ownedCount); 
        }
        
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