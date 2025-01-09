using System;
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

    private int _beforeEnhanceLevel;
    private int _afterEnhanceLevel;

    private int _beforeHp;
    private int _beforeAtk;
    private int _beforeDef;

    private int _afterHp;
    private int _afterAtk;
    private int _afterDef;

    private int _enhanceGoldCost;
    private int _enhanceMaterialCost;

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
        _characterInfoController._infoUI._mileageUseConfirmButton.onClick.AddListener(UseMileage);
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
        _beforeEnhanceLevel = _characterData.Enhancement.Value;
        _beforeHp = Convert.ToInt32(_characterData.HpPointLeveled);
        _beforeAtk = Convert.ToInt32(_characterData.AttackPointLeveled);
        _beforeDef = Convert.ToInt32(_characterData.DefensePointLeveled);

        SetEnhanceCost();

        _characterInfoController._infoUI._beforeUpGradeText.text = $"현재 등급 {_beforeEnhanceLevel}";
        _characterInfoController._infoUI._beforeHpText.text = $"체력 {_beforeHp}";
        _characterInfoController._infoUI._beforeAtkText.text = $"공격력 {_beforeAtk}";
        _characterInfoController._infoUI._beforeDefText.text = $"방어력 {_beforeDef}";
    }

    /// <summary>
    /// 강화 이후 정보
    /// </summary>
    private void AfterEnhance()
    {
        _afterEnhanceLevel = (_beforeEnhanceLevel + 1);
        _characterInfoController._infoUI._afterMax.SetActive(_beforeEnhanceLevel >= 10);
        _characterInfoController._infoUI._beforeMax.SetActive(_beforeEnhanceLevel < 10);

        if (_afterEnhanceLevel > 10)
        {
            //강화 Max 단계 정보
            _characterInfoController._infoUI._afterMaxTitleText.text = $"{_characterData.Name} +{_beforeEnhanceLevel}";
            _characterInfoController._infoUI._afterMaxHpText.text = $"체력 : {_characterData.HpPointLeveled}";
            _characterInfoController._infoUI._afterMaxAtkText.text = $"공격력 : {_characterData.AttackPointLeveled}";
            _characterInfoController._infoUI._afterMaxDefText.text = $"방어력 : {_characterData.DefensePointLeveled}";

            _characterInfoController._infoUI._enhanceCoinText.text = "-";
            _characterInfoController._infoUI._enhanceMaterialText.text = "-";
        }
        else
        {
            //다음 강화 스탯 정보
            _afterHp = Convert.ToInt32(_characterData.HpPointLeveled * (1f + 0.1f * _afterEnhanceLevel) / (1f + 0.1f * (_characterData.Enhancement.Value)));
            _afterAtk = Convert.ToInt32(_characterData.AttackPointLeveled * (1f + 0.1f * _afterEnhanceLevel) / (1f + 0.1f * (_characterData.Enhancement.Value)));
            _afterDef = Convert.ToInt32(_characterData.DefensePointLeveled * (1f + 0.1f * _afterEnhanceLevel) / (1f + 0.1f * (_characterData.Enhancement.Value)));

            _characterInfoController._infoUI._afterUpGradeText.text = $"강화 후 등급 {_afterEnhanceLevel}";
            _characterInfoController._infoUI._afterHpText.text = $"체력 {_afterHp}";
            _characterInfoController._infoUI._afterAtkText.text = $"공격력 {_afterAtk}";
            _characterInfoController._infoUI._afterDefText.text = $"방어력 {_afterDef}";
        }
    }

    /// <summary>
    /// 캐릭터 강화 기능
    /// </summary>
    private void Enhance()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;

        if (_characterInfoController._infoUI._enhanceResultPopup.activeSelf)
        {
            _characterInfoController._infoUI._enhanceResultPopup.SetActive(false);
            return;
        }
        
        if (_characterInfoController._infoUI._mileageUsePopup.activeSelf)
        {
            _characterInfoController._infoUI._mileageUsePopup.SetActive(false);
            return;
        }
        
        //기본 강화 확률 + 추가 재료 강화 확률 > Probability 보다 크면 성공
        //아니면 실패
        //TODO: 프로토타입 이후 확률 수정 필요 
        //현재 임시로 강화 레벨에 따라 최소 강화 확률 조정 중

        //최소 확률 > 임시 70%
        _minEnhanceProbability = (_maxEnhanceLevel - 7) * 0.1f;

        //강화 성공 확률
        _enhanceProbability = GetProbability(Random.Range(_minEnhanceProbability + ((_beforeEnhanceLevel + 0.1f) * 0.1f), 0.9f));

        //0.2 ~ 
        _chance = GetProbability(Random.Range(0.2f, _enhanceProbability + 0.05f));
        _chance = Mathf.Clamp(_chance, 0.01f, 1f);

        Debug.Log($"성공 최소 확률:{_enhanceProbability} / 나의 확률 :{_chance}");

        if (_chance >= _enhanceProbability)
        {
            GameManager.UserData.StartUpdateStream()
                .SetDBValue(_characterData.Enhancement, _characterData.Enhancement.Value + 1)
                .SetDBValue(_characterInfoController.UserGoldData, _characterInfoController.UserGoldData.Value - _enhanceGoldCost)
                .SetDBValue(_characterInfoController.UserEnhanceMaterialData, _characterInfoController.UserEnhanceMaterialData.Value - _enhanceMaterialCost)
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
            ResultPopup("강화 실패 \n 사유 : 네트워크 오류", _characterInfoController._infoUI.EnhanceResultIcons[2]);
            return;
        }

        ResultPopup($"+{_characterData.Enhancement.Value} 강화에 성공하셨습니다.", _characterInfoController._infoUI.EnhanceResultIcons[0]);
        CharacterStats();
        UpdateInfo();
        EnhanceCheck();
    }

    /// <summary>
    /// 강화 실패 후 
    /// </summary>
    private void EnhanceFail()
    {
        ResultPopup($"+{_characterData.Enhancement.Value + 1} 강화에 실패하셨습니다. \n 마일리지 적립 +10%", _characterInfoController._infoUI.EnhanceResultIcons[1]);

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
                    ResultPopup("마일리지 적립 실패 \n 사유 : 네트워크 오류", _characterInfoController._infoUI.EnhanceResultIcons[2]);
                    return;
                }
                _characterInfoController._infoUI._mileageSlider.value = _mileage;
                _characterInfoController._infoUI._mileageUseButton.interactable = _mileage >= 1f;
            });
    }

    /// <summary>
    /// 강화 결과 팝업
    /// </summary>
    /// <param name="text">안내 문구</param>
    private void ResultPopup(string text, Sprite sprite)
    {
        _characterInfoController._infoUI._enhanceResultPopup.SetActive(true);
        _characterInfoController._infoUI._enhanceResultText.text = text;
        _characterInfoController._infoUI._enhanceResultIcon.sprite = sprite;
    }

    /// <summary>
    /// 강화 실패 마일리지 변동 사항
    /// </summary>
    /// <param name="value"></param>
    private void MileageValueChange(float value)
    {
        _characterInfoController._infoUI._mileageValueText.text = $"강화 마일리지 {Mathf.Floor(value * 100f)}%";
    }

    private void UseMileage()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;

        GameManager.UserData.StartUpdateStream()
            .SetDBValue(_characterData.Enhancement, _characterData.Enhancement.Value + 1)
            .SetDBValue(_characterInfoController.UserGoldData, _characterInfoController.UserGoldData.Value - _enhanceGoldCost)
            .SetDBValue(_characterInfoController.UserEnhanceMaterialData, _characterInfoController.UserEnhanceMaterialData.Value - _enhanceMaterialCost)
            .Submit(result =>
            {
                EnhanceSuccess(result);

                if (!result) return;
                MileageUpdate(0);
                _characterInfoController._infoUI._mileageUseButton.interactable = false;
                _characterInfoController._infoUI._mileageUsePopup.SetActive(false);
            });
    }

    /// <summary>
    /// 강화 완료 후 스탯 반영
    /// </summary>
    private void CharacterStats()
    {
        _characterInfoController.CurCharacterInfo.UpdateInfo();
        BeforeEnhance();
        AfterEnhance();
    }

    /// <summary>
    /// 강화 업그레이드 성공 시 정보 업데이트
    /// </summary>
    private void UpdateInfo()
    {
        _characterInfoController.UserGold = _characterInfoController.UserGoldData.Value;
        _characterInfoController.UserEnhanceMaterial = _characterInfoController.UserEnhanceMaterialData.Value;
        _characterInfoController._infoUI._enhanceText.text = $"+{_characterData.Enhancement.Value.ToString()}";
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
    /// 강화 비용 업데이트 및 UI 반영
    /// </summary>
    private void SetEnhanceCost()
    {
        _enhanceMaterialCost = 200 * (_beforeEnhanceLevel + 1);
        _enhanceGoldCost = 100 * (_beforeEnhanceLevel + 1);
        _characterInfoController._infoUI._enhanceCoinText.text = _enhanceGoldCost.ToString();
        _characterInfoController._infoUI._enhanceMaterialText.text = _enhanceMaterialCost.ToString();
    }

    /// <summary>
    /// 강화 가능 여부 체크
    /// </summary>
    private void EnhanceCheck()
    {
        //TODO: 활성화/비활성화 조건 수정 필요 현재는 테스트로 임시 ~
        _characterInfoController._infoUI._enhanceCoinText.color = _characterInfoController.UserGold >= _enhanceGoldCost ? Color.white : Color.red;
        _characterInfoController._infoUI._enhanceMaterialText.color =
            _characterInfoController.UserEnhanceMaterial >= _enhanceMaterialCost ? Color.white : Color.red;

        _characterInfoController._infoUI._enhanceButton.interactable =
            _beforeEnhanceLevel < _maxEnhanceLevel &&
            _characterInfoController.UserGold >= _enhanceGoldCost &&
            _characterInfoController.UserEnhanceMaterial >= _enhanceMaterialCost;
    }

    /// <summary>
    /// 현재 캐릭터의 데이터를 받아옴
    /// </summary>
    /// <param name="characterData"></param>
    public void GetCharacterData(CharacterData characterData)
    {
        _characterInfoController.CurCharacterEnhance = this;
        _characterData = characterData;
        _characterInfoController._infoUI._mileageSlider.value = _mileage;
        _characterInfoController._infoUI._mileageUseButton.interactable = _mileage >= 1f;
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