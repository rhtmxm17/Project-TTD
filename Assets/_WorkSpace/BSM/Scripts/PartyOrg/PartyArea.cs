using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterState = BSM_Character_State.CharacterState;

public class PartyArea : BaseUI
{

    private GameObject _frontLine;
    private GameObject _middleLine;
    private GameObject _backLine;

    private Coroutine _checkCo;
 
    private BuffSystem _buffSystem;
    private CharacterState[] _frontCharacters;
    private CharacterState[] _middleCharacters;
    private CharacterState[] _backCharacters;
    
    
    private Dictionary<UnitType, int> _unitTypeDict = new Dictionary<UnitType, int>();
    
    protected override void Awake()
    {
        base.Awake();
        GetBind();
    }

    private void OnEnable()
    { 
        if(_checkCo == null) PartyAreaCheck();
    }

    private void Start() => PartyAreaCheck();
    
    private void GetBind()
    {
        _buffSystem = GetComponent<BuffSystem>();
        
        _frontLine = GetUI("FrontLine");
        _middleLine = GetUI("MiddleLine");
        _backLine = GetUI("BackLine");
    }

    private void PartyAreaCheck()
    {
        _checkCo = StartCoroutine(PartyAreaCoroutine());
    }
 
    private IEnumerator PartyAreaCoroutine()
    {
        //전투 시작 전까지 조건
        while (true)
        {
            _frontCharacters = _frontLine.GetComponentsInChildren<CharacterState>();
            _middleCharacters = _middleLine.GetComponentsInChildren<CharacterState>();
            _backCharacters = _backLine.GetComponentsInChildren<CharacterState>();
            
            _buffSystem.FrontCount = _frontCharacters.Length;
            _buffSystem.MiddleCount = _middleCharacters.Length;
            _buffSystem.BackCount = _backCharacters.Length;
            
            _buffSystem.CurrentBuff();
            yield return null;
        } 
    } 
    
    //캐릭터 속성 개수 종합
    private void TotalUnitType()
    {
          _frontCharacters[0].CurUnitType;

    }
    
}
