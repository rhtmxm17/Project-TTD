using System;
using System.Collections;
using UnityEngine; 

public class CharacterInfo : BaseUI
{
    [SerializeField] protected CharacterData _data;
    
    protected CharacterInfoManager _characterInfoManager;
    protected int _curIndex;

    private void Start()
    {
        Init(); 
    }
  
    private void Init()
    {
        _characterInfoManager = GetComponentInParent<CharacterInfoManager>();
        _curIndex = _characterInfoManager._characters.IndexOf(this);  
    } 
    
}
