using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterSort : MonoBehaviour
{
    [HideInInspector] public CharacterInfoController CharacterInfoController;
    [HideInInspector] public List<CharacterInfo> _sortCharacterInfos;

    private List<object> _sortList;

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
        set { _isSorting = value; }
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
        _characterSortUI.NameSortButton.onClick.AddListener(() => SortEventFunc(SortType.NAME));
        _characterSortUI.LevelSortButton.onClick.AddListener(() => SortEventFunc(SortType.LEVEL));
        _characterSortUI.PowerLevelSortButton.onClick.AddListener(() => SortEventFunc(SortType.POWERLEVEL));
    }

    /// <summary>
    /// 정렬 함수 호출 및 현재 타입 받아오기
    /// </summary>
    /// <param name="type"></param>
    private void SortEventFunc(SortType type)
    {
        if (_curSortType.Equals(type))
        {
            _isSorting = !_isSorting;
        }
        else
        {
            _isSorting = true;
        }

        _curSortType = type;

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
            default:
                _sortList = _sortCharacterInfos.Select(x => (object)x.CharacterLevel).ToList();
                break;
            case SortType.NAME:
                _sortList = _sortCharacterInfos.Select(x => (object)x.CharacterName).ToList();
                break;
            case SortType.POWERLEVEL:
                _sortList = _sortCharacterInfos.Select(x => (object)x.PowerLevel).ToList();
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
    private object GetSortValue(CharacterInfo characterInfo)
    {
        if (_curSortType.Equals(SortType.NAME))
        {
            return characterInfo.CharacterName;
        }
        else if (_curSortType.Equals(SortType.LEVEL))
        {
            return characterInfo.CharacterLevel;
        }
        else if (_curSortType.Equals(SortType.POWERLEVEL))
        {
            return characterInfo.PowerLevel;
        }

        return null;
    }
}