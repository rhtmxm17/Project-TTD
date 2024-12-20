using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : BaseUI
{
    public CharacterData _data;
    
    private CharacterList _characterList;
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
        _characterList = GetComponentInParent<CharacterList>();
        _curIndex = _characterList._characters.IndexOf(this);
    }
    
}
