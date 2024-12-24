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
    }

    private void Start()
    {
        _characterSortUI.NameSortButton.onClick.AddListener(NameSort);
        _characterSortUI.LevelSortButton.onClick.AddListener(LevelSort);
    }

    /// <summary>
    /// 캐릭터 레벨 오름차순 정렬
    /// </summary>
    public void LevelSort()
    {
        if (_sortList != null && _sortList.Count > 0) _sortList.Clear();

        _sortList = _sortCharacterInfos.Select(x => (object)x.CharacterLevel).ToList();
        _sortList.Sort();

        for (int i = 0; i < _sortList.Count; i++)
        {
            for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
            {
                if (_sortList[i].Equals(_sortCharacterInfos[j].CharacterLevel))
                {
                    CharacterData oldData = _sortCharacterInfos[i]._CharacterData;
                    CharacterData newData = _sortCharacterInfos[j]._CharacterData;

                    _sortCharacterInfos[i]._CharacterData = newData;
                    _sortCharacterInfos[j]._CharacterData = oldData;

                    _sortCharacterInfos[i].SetListNameText(_sortCharacterInfos[i].CharacterName);
                    _sortCharacterInfos[j].SetListNameText(_sortCharacterInfos[j].CharacterName);
                    break;
                }
            }
        }

        _curSortType = SortType.LEVEL;
        PlayerPrefs.SetInt("SortType", (int)_curSortType); 
    }
 
    /// <summary>
    /// 이름 오름차순 정렬 기능
    /// </summary>
    public void NameSort()
    {
        if (_sortList != null && _sortList.Count > 0) _sortList.Clear();

        _sortList = _sortCharacterInfos.Select(x => (object)x.CharacterName).ToList();
        _sortList.Sort();

        for (int i = 0; i < _sortList.Count; i++)
        {
            for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
            {
                if (_sortList[i].Equals(_sortCharacterInfos[j].CharacterName))
                {
                    CharacterData newData = _sortCharacterInfos[j]._CharacterData;
                    CharacterData oldData = _sortCharacterInfos[i]._CharacterData;

                    _sortCharacterInfos[i]._CharacterData = newData;
                    _sortCharacterInfos[j]._CharacterData = oldData;

                    _sortCharacterInfos[i].SetListNameText(_sortCharacterInfos[i].CharacterName);
                    _sortCharacterInfos[j].SetListNameText(_sortCharacterInfos[j].CharacterName);
                    break;
                }
            }
        }

        _curSortType = SortType.NAME; 
        PlayerPrefs.SetInt("SortType", (int)_curSortType);
    }
}