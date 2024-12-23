using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterSort : MonoBehaviour
{
    public List<CharacterInfo> _sortCharacterInfos;
    
    public List<object> _sortList; 
    
    private CharacterSortUI _characterSortUI;
 
    private void Awake()
    {
        _characterSortUI = GetComponent<CharacterSortUI>();
        
       
    }

    private void Start()
    {
        _characterSortUI.NameSortButton.onClick.AddListener(NameSort);
 
    }
 

    private void LevelSort()
    {
        
    }
    
    
    private void NameSort()
    { 
        _sortList = _sortCharacterInfos.Select(x => (object)x.GetCharacterName()).ToList();
        _sortList.Sort();
    }
}



