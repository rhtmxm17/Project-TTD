using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterSort : MonoBehaviour
{
    [HideInInspector] public List<CharacterInfo> _sortCharacterInfos;

    private List<object> _sortList;

    private CharacterSortUI _characterSortUI;
    private SortType _curSortType;

    private void Awake()
    {
        _characterSortUI = GetComponent<CharacterSortUI>(); 
        _curSortType = (SortType)PlayerPrefs.GetInt("SortType");
    }

    private void Start()
    {
        _characterSortUI.NameSortButton.onClick.AddListener(() => SortEventFunc(SortType.NAME));
        _characterSortUI.LevelSortButton.onClick.AddListener(() => SortEventFunc(SortType.LEVEL));
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

    public void CharacterListFilter()
    {
        //TODO: 캐릭터 리스트 필터 기능 추가
        //현재 선택한 속성 외 캐릭터 리스트는 Active = false or List에서 Remove하면 될듯?
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
                _sortList = _sortCharacterInfos.Select(x => (object)x.CharacterLevel).ToList();
                break;
            case SortType.NAME:
                _sortList = _sortCharacterInfos.Select(x => (object)x.CharacterName).ToList();
                break;
        }

        _sortList.Sort();
        
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

                    _sortCharacterInfos[i].SetListImage(newData.FaceIconSprite);
                    _sortCharacterInfos[j].SetListImage(oldData.FaceIconSprite);
                    break;
                }
            }
        }

        PlayerPrefs.SetInt("SortType", (int)_curSortType);
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

        return null;
    }
}