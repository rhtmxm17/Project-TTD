using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterInfoController : BaseUI
{
    [HideInInspector] public CharacterInfoUI _infoUI;
    public List<CharacterInfo> _characterInfos;
    public CharacterInfo CurCharacterInfo;

    public GameObject _infoPopup;

    private Button _prevButton;
    private Button _nextButton;

    public int CurIndex = 0;
    
    protected override void Awake()
    {
        base.Awake();
        Init();
        SubscribeEvent();
    }
 
    private void OnEnable() => UpdateCharacterList();

    private void Init()
    {
        _infoUI = GetUI<CharacterInfoUI>("InfoUI");  
        _infoPopup = GetUI("InfoPopup");

        _prevButton = GetUI<Button>("PreviousButton");
        _nextButton = GetUI<Button>("NextButton");

    }

    private void SubscribeEvent()
    {
        _prevButton.onClick.AddListener(PreviousCharacter);
        _nextButton.onClick.AddListener(NextCharacter);
    }
    
    /// <summary>
    /// 보유 캐릭터 리스트 업데이트
    /// </summary>
    private void UpdateCharacterList()
    {
        _characterInfos = GetComponentsInChildren<CharacterInfo>().ToList();
    }

    private void PreviousCharacter()
    {
        if (CurIndex == 0)
        {
            CurIndex = _characterInfos.Count - 1; 
        }
        else
        {
            CurIndex--; 
        }
        
        CurCharacterInfo = _characterInfos[CurIndex];
        Debug.Log($"{CurIndex}이전 캐릭터 이동");
    }

    private void NextCharacter()
    {
        if (CurIndex == _characterInfos.Count - 1)
        {
            CurIndex = 0;
        }
        else
        {
            CurIndex++;
        }
         
        CurCharacterInfo = _characterInfos[CurIndex];
 
        Debug.Log($"{CurIndex} 캐릭터 이동");
    }
    
}