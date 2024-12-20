using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using CharacterState = BSM_Character_State.CharacterState;
using BuffSystem = _WorkSpace.BSM.Scripts.PartyOrg.BuffSystem;
using CircleChild = _WorkSpace.BSM.Scripts.PartyOrg.CircleChild;

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
 
    private Dictionary<CharacterState, Transform> _battleScenePos;

    [SerializeField] private CircleChild[] _childList;
    
    protected override void Awake()
    {
        base.Awake();
        GetBind();
    }

    private void OnEnable()
    {
        if (_checkCo == null) PartyAreaCheck();
    }

    private void Start() => PartyAreaCheck();

    private void GetBind()
    {
        _childList = GetComponentsInChildren<CircleChild>();
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
            _buffSystem.UpdateTypeBuff(_childList); 
            
            
            yield return null;
        }
    } 
    
    //각 캐릭터의 타입을 가져와서
}