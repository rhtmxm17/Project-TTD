using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterSort : MonoBehaviour
{
    public List<CharacterInfo> _sortCharacterInfos;

    private List<object> _sortList;

    private CharacterSortUI _characterSortUI;

    private void Awake()
    {
        _characterSortUI = GetComponent<CharacterSortUI>();
    }

    private void Start()
    {
        _characterSortUI.NameSortButton.onClick.AddListener(NameSort);
        _characterSortUI.LevelSortButton.onClick.AddListener(LevelSort);
    }

    
    //TODO: Sort 로직 하나로 수정하기.
    
    /// <summary>
    /// 캐릭터 레벨 오름차순 정렬
    /// </summary>
    private void LevelSort()
    {
        Debug.Log("레벨순 정렬 시작");
        if (_sortList != null && _sortList.Count > 0) _sortList.Clear();

        _sortList = _sortCharacterInfos.Select(x => (object)x.Level).ToList();
        _sortList.Sort();

        for (int i = 0; i < _sortList.Count; i++)
        {
            for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
            {
                if (_sortList[i].Equals(_sortCharacterInfos[j].Level))
                {
                    CharacterData oldData = _sortCharacterInfos[i].GetCharacterData();
                    CharacterData newData = _sortCharacterInfos[j].GetCharacterData();

                    _sortCharacterInfos[i].ChangeData(newData);
                    _sortCharacterInfos[j].ChangeData(oldData);

                    _sortCharacterInfos[i].SetListNameText(_sortCharacterInfos[i].GetCharacterName());
                    _sortCharacterInfos[j].SetListNameText(_sortCharacterInfos[j].GetCharacterName());
                }
            }
        }
    }


    /// <summary>
    /// 이름 오름차순 정렬 기능
    /// </summary>
    private void NameSort()
    {
        if (_sortList.Count != 0) _sortList.Clear();

        _sortList = _sortCharacterInfos.Select(x => (object)x.GetCharacterName()).ToList();
        _sortList.Sort();

        //린지 > 메피 > 케피 > 코로몬
        CharacterData tempData;

        for (int i = 0; i < _sortList.Count; i++)
        {
            for (int j = i + 1; j < _sortCharacterInfos.Count; j++)
            {
                if (_sortList[i].Equals(_sortCharacterInfos[j].GetCharacterName()))
                {
                    CharacterData searchData = _sortCharacterInfos[j].GetCharacterData();

                    //J 번째를 i번째로 옮기고, i 번째의 데이터를 J번째로
                    tempData = _sortCharacterInfos[i].GetCharacterData();
                    _sortCharacterInfos[i].ChangeData(searchData);
                    _sortCharacterInfos[j].ChangeData(tempData);

                    _sortCharacterInfos[i].SetListNameText(_sortList[i].ToString());
                    _sortCharacterInfos[j].SetListNameText(_sortCharacterInfos[j].GetCharacterName());
                    break;
                }
            }
        }
    }
}