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
    [HideInInspector] public List<CharacterInfo> _characterInfos;
    [HideInInspector] public CharacterInfo CurCharacterInfo;

    [HideInInspector] public GameObject _infoPopup;
    
    [HideInInspector] public int CurIndex = 0;
    
    private Button _prevButton;
    private Button _nextButton;

    
    
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
    
    /// <summary>
    /// 이전 캐릭터 정보로 변경
    /// </summary>
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
        CurCharacterInfo.UpdateInfo(); 
    }
    
    /// <summary>
    /// 다음 캐릭터 정보로 변경
    /// </summary>
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
        CurCharacterInfo.UpdateInfo(); 
    }
    
}