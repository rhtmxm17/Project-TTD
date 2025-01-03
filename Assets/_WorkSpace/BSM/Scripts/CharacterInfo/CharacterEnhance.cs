using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class CharacterEnhance : MonoBehaviour
{
    private CharacterInfoController _characterInfoController;
    private CharacterData _characterData;

    private bool _isSubscribe;

    private readonly int _maxEnhanceLevel = 10;
    private float _minEnhanceProbability = 0.9f;
    private float _enhanceProbability;
    private float _chance;

    private int _characterEnhanceLevel;
    private int beforeEnhanceLevel;

    private int _beforeHp;
    private int _beforeAtk;
    private int _beforeDef;
    
    private int _afterHp;
    private int _afterAtk;
    private int _afterDef;

    /// <summary>
    /// 0.0f ~ 1.0f 범위 마일리지
    /// </summary>
    private float _mileage => Mathf.Clamp(_characterData.EnhanceMileagePerMill.Value * 0.001f, 0f, 1f);
    
    public UnityAction OnBeforeEnhance;
    public UnityAction OnAfterEnhance;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        AddListenerEvent();  
    }

    private void OnEnable() => SubscribeEvent();

    private void OnDisable() => UnSubscribeEvent();

    private void Init()
    {
        _characterInfoController = GetComponentInParent<CharacterInfoController>();   
    }
 
    private void AddListenerEvent()
    {
        if (_isSubscribe) return;
        _isSubscribe = true;

        _characterInfoController._infoUI._enhanceButton.onClick.AddListener(Enhance);
        _characterInfoController._infoUI._mileageSlider.onValueChanged.AddListener(value => MileageValueChange(value));
    }

    private void SubscribeEvent()
    {
        OnBeforeEnhance += BeforeEnhance;
        OnAfterEnhance += AfterEnhance;
    }

    private void UnSubscribeEvent()
    {
        OnBeforeEnhance -= BeforeEnhance;
        OnAfterEnhance -= AfterEnhance;
    }

 
    /// <summary>
    /// 강화 이전 정보
    /// </summary>
    private void BeforeEnhance()
    {
        beforeEnhanceLevel = _characterData.Enhancement.Value;
        _characterInfoController._infoUI._beforeUpGradeText.text = $"현재 등급 {beforeEnhanceLevel}";
        _characterInfoController._infoUI._beforeHpText.text = $"체력 {_beforeHp}";
        _characterInfoController._infoUI._beforeAtkText.text = $"공겨력 {_beforeAtk}";
        _characterInfoController._infoUI._beforeDefText.text = $"방어력 {_beforeDef}"; 
    }
    
    /// <summary>
    /// 강화 이후 정보
    /// </summary>
    private void AfterEnhance()
    {
        //TODO: 현재 강화가 10일 때 처리 필요함
        _characterInfoController._infoUI._afterUpGradeText.text = $"강화 후 등급 {beforeEnhanceLevel + 1}";

        _afterHp = (beforeEnhanceLevel + 1) * _characterInfoController.CurCharacterInfo.Hp;
        _afterAtk = (beforeEnhanceLevel + 1) * _characterInfoController.CurCharacterInfo.Atk;
        _afterDef = (beforeEnhanceLevel + 1) * _characterInfoController.CurCharacterInfo.Def;

        _characterInfoController._infoUI._afterHpText.text = $"체력 {_afterHp}";
        _characterInfoController._infoUI._afterAtkText.text = $"공격력 {_afterAtk}";
        _characterInfoController._infoUI._afterDefText.text = $"방어력 {_afterDef}"; 
    }

    /// <summary>
    /// 캐릭터 강화 기능
    /// </summary>
    private void Enhance()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;

        //기본 강화 확률 + 추가 재료 강화 확률 > Probability 보다 크면 성공
        //아니면 실패
        //TODO: 프로토타입 이후 확률 수정 필요 
        //현재 임시로 강화 레벨에 따라 최소 강화 확률 조정 중
        
        //최소 확률 > 임시 80%
        _minEnhanceProbability = (_maxEnhanceLevel - 3) * 0.1f;
         
        //강화 성공 확률 : 0.3 ~ 최대 1f 확률 사이
        _enhanceProbability = GetProbability(Random.Range(_minEnhanceProbability, 1f));

        _chance = GetProbability(Random.Range((_enhanceProbability - 0.3f), (_enhanceProbability + 0.1f)));
        Debug.Log($"성공 확률 :{_enhanceProbability} >  내 기본 확률 : {_chance} / 마일리지 보정 : {_mileage}");
        _chance = Mathf.Clamp(_chance + _mileage, 0.01f, 1f);

        if (_chance >= _enhanceProbability)
        {
            GameManager.UserData.StartUpdateStream()
                .SetDBValue(_characterData.Enhancement, _characterData.Enhancement.Value + 1)
                .Submit(EnhanceSuccess);
        }
        else
        {
            EnhanceFail();
        }
    }
    
    /// <summary>
    /// 강화 성공 후
    /// </summary>
    /// <param name="result"></param>
    private void EnhanceSuccess(bool result)
    {
        if (!result)
        {
            ResultPopup("강화 실패 \n 사유 : 네트워크 오류");
            return;
        }

        MileageUpdate(0); 
        ResultPopup("강화 성공해서 마일리지 초기화 할게요~.~");
        CharacterStats();
        UpdateInfo();
    }
    
    /// <summary>
    /// 강화 실패 후 
    /// </summary>
    private void EnhanceFail()
    {
        ResultPopup("강화 실패했습니당~ \n 마일리지 추가!");
        
        //TODO: 마일리지 누적 값 수정 필요
        MileageUpdate(_characterData.EnhanceMileagePerMill.Value + 100);
    }
    
    /// <summary>
    /// 강화 결과에 따라 마일리지 업데이트
    /// </summary>
    /// <param name="perMill">마일리지 값 퍼밀(1/1000)</param>
    private void MileageUpdate(int perMill)
    {
        GameManager.UserData.StartUpdateStream() 
            .SetDBValue(_characterData.EnhanceMileagePerMill, perMill)
            .Submit(result =>
            {
                if (!result)
                {
                    ResultPopup("마일리지 업뎃 실패");
                    return;
                }
                _characterInfoController._infoUI._mileageSlider.value = _mileage;
            });
    }
    
    /// <summary>
    /// 강화 결과 팝업
    /// </summary>
    /// <param name="text">안내 문구</param>
    private void ResultPopup(string text)
    {
        _characterInfoController._infoUI._enhanceResultPopup.SetActive(true);
        _characterInfoController._infoUI._enhanceResultText.text = text;
    }
    
    /// <summary>
    /// 강화 실패 마일리지 변동 사항
    /// </summary>
    /// <param name="value"></param>
    private void MileageValueChange(float value)
    {
        _characterInfoController._infoUI._mileageValueText.text = $"강화 마일리지 {value * 100f}%";
    }

    /// <summary>
    /// 강화 완료 후 스탯 반영
    /// </summary>
    private void CharacterStats()
    {
        _characterEnhanceLevel = _characterData.Enhancement.Value;
        
        //TODO: 캐릭터 강화 수치 수정 필요
        _characterInfoController.CurCharacterInfo.Hp = _afterHp;
        _characterInfoController.CurCharacterInfo.Atk = _afterAtk;
        _characterInfoController.CurCharacterInfo.Def = _afterDef;
        _characterInfoController.CurCharacterInfo.PowerLevel = _afterHp + _afterAtk + _afterDef;

        _beforeAtk = _afterAtk;
        _beforeHp = _afterHp;
        _beforeDef = _afterDef; 
        BeforeEnhance();
        AfterEnhance();
    }

    /// <summary>
    /// 강화 업그레이드 성공 시 정보 업데이트
    /// </summary>
    private void UpdateInfo()
    {
        //TODO: 강화 탭 스탯 UI 변경 필요 
        _characterInfoController._infoUI._enhanceText.text = $"+{_characterData.Enhancement.Value.ToString()}";
        EnhanceCheck();
    }

    /// <summary>
    /// 강화 확률 반환, 소수점 3자리까지 제한
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private float GetProbability(float value)
    {
        return Mathf.Floor(value * 1000f) / 1000f;
    }

    /// <summary>
    /// 강화 가능 여부 체크
    /// </summary>
    private void EnhanceCheck()
    {
        //TODO: 활성화/비활성화 조건 수정 필요 현재는 테스트로 임시 ~
        _characterInfoController._infoUI._enhanceButton.interactable = _characterEnhanceLevel < _maxEnhanceLevel;
    }

    /// <summary>
    /// 현재 캐릭터의 데이터를 받아옴
    /// </summary>
    /// <param name="characterData"></param>
    public void GetCharacterData(CharacterData characterData)
    {
        _characterInfoController.CurCharacterEnhance = this;
        _characterData = characterData;
        _characterEnhanceLevel = characterData.Enhancement.Value;

        _beforeAtk = _characterInfoController.CurCharacterInfo.Atk;
        _beforeHp = _characterInfoController.CurCharacterInfo.Hp;
        _beforeDef = _characterInfoController.CurCharacterInfo.Def;
        _characterInfoController._infoUI._mileageSlider.value = _mileage;
         
        EnhanceCheck();
    }

    public void CheatEnhance(int enhanceLevel)
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;

        GameManager.UserData.StartUpdateStream()
            .SetDBValue(_characterData.Enhancement, enhanceLevel)
            .Submit(EnhanceSuccess);
    }
}