using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterInfo : BaseUI
{
    [SerializeField] protected CharacterData _data;
    
    protected CharacterInfoManager _characterInfoManager;
    protected int _curIndex;

    protected override void Awake()
    {
        base.Awake(); 
    }

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
