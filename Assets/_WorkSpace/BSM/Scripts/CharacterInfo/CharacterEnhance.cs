using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterEnhance : MonoBehaviour
{
    private CharacterInfoController _characterInfoController;
    private CharacterData _characterData;

    private bool _isSubscribe;

    private readonly int _maxEnhanceLevel = 10;
    private float _minEnhanceProbability = 0.9f;
    private int _characterEnhanceLevel;

    //Info에서 DB를 전달받을 필요가 있을듯

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        SubscribeEvent();
    }

    private void Init()
    {
        _characterInfoController = GetComponentInParent<CharacterInfoController>();
    }

    private void SubscribeEvent()
    {
        if (_isSubscribe) return;
        _isSubscribe = true;

        _characterInfoController._infoUI._enhanceButton.onClick.AddListener(Enhance);
    }

    /// <summary>
    /// 캐릭터 강화 기능
    /// </summary>
    private void Enhance()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;

        //기본 강화 확률 + 추가 재료 강화 확률 > Probability 보다 크면 성공
        //아니면 실패
        //현재 강화 레벨에 따라 최소 강화 확률 조정 필요
        _minEnhanceProbability = (_maxEnhanceLevel - _characterEnhanceLevel) * 0.1f;
        float enhanceProbability = GetProbability(Random.Range(_minEnhanceProbability, 1f));

        //내 캐릭터 강화 공식이 필요
        float chance = _characterEnhanceLevel == 1
            ? 1f
            : GetProbability(Random.Range((enhanceProbability - 0.2f), (enhanceProbability + 0.2f)));
        chance = Mathf.Clamp(chance, 0.01f, 1f);

        if (chance >= enhanceProbability)
        {
            GameManager.UserData.StartUpdateStream()
                .SetDBValue(_characterData.Enhancement, _characterData.Enhancement.Value + 1)
                .Submit(EnhanceSuccess);

            Debug.Log($"강화 :{_characterData.name} :{_characterEnhanceLevel} 강화 성공");
        }
        else
        {
            Debug.Log($"강화 실패");
            EnhanceFail();
        }
    }

    private void EnhanceSuccess(bool result)
    {
        if (!result)
        {
            Debug.Log("네트워크 오류");
            return;
        }

        _characterEnhanceLevel = _characterData.Enhancement.Value;
        CharacterStats();
        UpdateInfo();
    }

    private void EnhanceFail()
    {
        
    }
    
    /// <summary>
    /// 강화 완료 후 스탯 반영
    /// </summary>
    private void CharacterStats()
    {
        _characterInfoController.CurCharacterInfo.Hp = _characterData.Level.Value *
                                                       (int)(_characterData.StatusTable.healthPointBase +
                                                             _characterData.StatusTable.healthPointGrouth);
        
        _characterInfoController.CurCharacterInfo.Atk = _characterData.Level.Value *
                                                        (int)(_characterData.StatusTable.attackPointBase +
                                                              _characterData.StatusTable.attackPointGrowth);
        
        _characterInfoController.CurCharacterInfo.Def = _characterData.Level.Value *
                                                        (int)(_characterData.StatusTable.defensePointBase +
                                                              _characterData.StatusTable.defensePointGrouth);

        _characterInfoController.CurCharacterInfo.PowerLevel = _characterInfoController.CurCharacterInfo.Hp +
                                                               _characterInfoController.CurCharacterInfo.Atk +
                                                               _characterInfoController.CurCharacterInfo.Def;
    }

    /// <summary>
    /// 강화 업그레이드 성공 시 정보 업데이트
    /// </summary>
    private void UpdateInfo()
    {
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
        //TODO: 활성화/비활성화 조건 수정 필요
        //테스트 강화 비활성화 조건
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