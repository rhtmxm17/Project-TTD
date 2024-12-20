using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInfoManager : BaseUI
{
    [HideInInspector] public List<CharacterInfo> _characters;
    [HideInInspector] public CharacterInfoUI _infoUI;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
 
    private void OnEnable() => UpdateCharacterList();

    private void Init()
    {
        _infoUI = GetUI<CharacterInfoUI>("InfoPopUp");
    }
    private void UpdateCharacterList()
    {
        _characters = GetComponentsInChildren<CharacterInfo>().ToList();
    }
}