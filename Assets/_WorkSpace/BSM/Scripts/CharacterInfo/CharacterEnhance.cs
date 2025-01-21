using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterEnhance : MonoBehaviour
{
    private CharacterInfoController _characterInfoController;
    private CharacterData _characterData;

    private bool _isSubscribe;
    private readonly int _maxEnhanceLevel = 10;
    private float _minEnhanceProbability = 0.9f;
    private float _enhanceProbability;
    private float _successProbability;
    private int _beforeEnhanceLevel;
    private int _afterEnhanceLevel;
    private int _beforeHp;
    private int _beforeAtk;
    private int _beforeDef;
    private int _afterHp;
    private int _afterAtk;
    private int _afterDef;
    private int _enhanceGoldCost;
    private int _enhanceDragonCandyCost;
    private int _curCharacterToken;
    private int _selectedCommonMaterial;
    private int _selectedCharacterMaterial;

    private EnhanceType _curEnhanceType;
    private EnhanceTokenType _curTokenType;
    private Color color = new Color(0.9882353f, 1f, 0.6196079f);
    private float[] _decreaseRate = { 0, 0.05f, 0.05f, 0.1f, 0.05f, 0.05f, 0.05f, 0.15f, 0.1f, 0.1f };
    private float _cumulativeProbability;

    private int[] _increaseMileage = { 1, 1, 2, 2, 4, 6, 7, 8, 12, 14 };
    
    /// <summary>
    /// 0.0f ~ 1.0f 범위 마일리지
    /// </summary>
    private float _mileage => Mathf.Clamp(_characterData.EnhanceMileagePerMill.Value * 0.01f, 0f, 1f);

    private float _sliderValue;
    private float SliderValue
    {
        get => _sliderValue;
        set
        {
            _sliderValue = value;
        
            //TODO: Slider 컬러 값 변경 필요
            if (_sliderValue >= 1f)
            {
                _characterInfoController._infoUI._sliderFillImage.sprite = _characterInfoController._infoUI.SliderFillSprites[1];
            }
            else
            {
                _characterInfoController._infoUI._sliderFillImage.sprite = _characterInfoController._infoUI.SliderFillSprites[0];
            } 
        }
    }
    
    public UnityAction OnBeforeEnhance;
    public UnityAction OnAfterEnhance;
    private Coroutine _effectCo;
    private Coroutine _faceEffectCo;
    
    private string[] _failTalk = { "실", "수", "해", "서 ", "미", "안", "해", "요", ".", "." };
    private string[] _successTalk = { "성", "공!", "\n", "어", "때", "요?", " 저", " 잘", "했", "죠?" };
    
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

        _characterInfoController._infoUI._enhanceButton.onClick.AddListener(()=>
        {
            _curEnhanceType = EnhanceType.ENHANCE;
            Enhance();
        });
        _characterInfoController._infoUI._mileageSlider.onValueChanged.AddListener(value => MileageValueChange(value));
        _characterInfoController._infoUI._mileageUseConfirmButton.onClick.AddListener(() =>
        {
            _curEnhanceType = EnhanceType.MILEAGE;
            UseMileage();
        });
        _characterInfoController._infoUI._characterTokenButton.onClick.AddListener(()=> EnhanceTokenSetUp(EnhanceTokenType.CHARACTER_TOKEN));
        _characterInfoController._infoUI._commonTokenButton.onClick.AddListener(()=> EnhanceTokenSetUp(EnhanceTokenType.COMMON_TOKEN));
         
        _characterInfoController._infoUI._tokenIncreaseButton.onClick.AddListener(IncreaseToken);
        _characterInfoController._infoUI._tokenDecreaseButton.onClick.AddListener(DecreaseToken); 
        _characterInfoController._infoUI._tokenInputField.onValueChanged.AddListener(x => InputToken(x));
        _characterInfoController._infoUI._tokenConfirmButton.onClick.AddListener(TokenPopupConfirm);
        _characterInfoController._infoUI._tokenCancelButton.onClick.AddListener(SelectTokenReset);
        _characterInfoController._infoUI._autoTokenButton.onClick.AddListener(AutoSelectToken);
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
    /// 토큰 자동 선택 기능 > 캐릭터 전용 토큰 우선 사용
    /// </summary>
    private void AutoSelectToken()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;
        _curTokenType = EnhanceTokenType.AUTO;
        
        _curCharacterToken = GameManager.TableData
            .GetItemData(_characterData.EnhanceItemID).Number.Value;

        if (_curCharacterToken > _enhanceDragonCandyCost)
        {
            _selectedCharacterMaterial = _enhanceDragonCandyCost;
        }
        else
        {
            _selectedCharacterMaterial = _curCharacterToken; 
            
            if (_characterInfoController.UserDragonCandy > (_enhanceDragonCandyCost - _selectedCharacterMaterial))
            {
                _selectedCommonMaterial = (_enhanceDragonCandyCost - _selectedCharacterMaterial);  
            }
            else
            {
                _selectedCommonMaterial = _characterInfoController.UserDragonCandy; 
            }
        }
 
        TokenCountTextUpdate();
        EnhanceCheck();
    }
    
    /// <summary>
    /// 마지막으로 입력 후 Confirm했던 값으로 초기화
    /// </summary>
    private void SelectTokenReset()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;

        if (_curTokenType == EnhanceTokenType.COMMON_TOKEN)
        {
            _selectedCommonMaterial = int.Parse(_characterInfoController._infoUI._commonTokenCountText.text);
            SetTextCommonTokenCount();
        }
        else if (_curTokenType == EnhanceTokenType.CHARACTER_TOKEN)
        {
            _selectedCharacterMaterial = int.Parse(_characterInfoController._infoUI._characterTokenCountText.text);
            SetTextCharacterTokenCount();
        } 
    }
    
    /// <summary>
    /// 강화 토큰 등록 팝업창 셋팅
    /// </summary>
    /// <param name="tokenType"></param>
    private void EnhanceTokenSetUp(EnhanceTokenType tokenType)
    {
        if(_characterInfoController.CurCharacterEnhance != this) return;
        
        _characterInfoController._infoUI._enhanceTokenPopup.SetActive(true);
        _curTokenType = tokenType;
        _curCharacterToken = GameManager.TableData
            .GetItemData(_characterData.EnhanceItemID).Number.Value;
        
        if (_curTokenType == EnhanceTokenType.CHARACTER_TOKEN)
        {
            _selectedCharacterMaterial = 0;
            SetTextInputField(_selectedCharacterMaterial);
            _characterInfoController._infoUI._tokenPopupTitleText.text = "용사탕 등록";
            _characterInfoController._infoUI._enhanceTokenIcon.sprite = _characterInfoController.TokenIcons[0];
        }
        else if (_curTokenType == EnhanceTokenType.COMMON_TOKEN)
        {
            _selectedCommonMaterial = 0;
            SetTextInputField(_selectedCommonMaterial);
            _characterInfoController._infoUI._tokenPopupTitleText.text = "용젤리 등록";
            _characterInfoController._infoUI._enhanceTokenIcon.sprite = _characterInfoController.TokenIcons[1];
        } 
    }
    
    /// <summary>
    /// 증가 버튼 클릭 기능 동작
    /// </summary>
    private void IncreaseToken()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;

        if (_curTokenType == EnhanceTokenType.CHARACTER_TOKEN)
        { 
            if (_selectedCharacterMaterial + _selectedCommonMaterial < _enhanceDragonCandyCost
                && _curCharacterToken > _selectedCharacterMaterial)
            {
                _selectedCharacterMaterial++;
                SetTextInputField(_selectedCharacterMaterial);
            } 
        }
        else if (_curTokenType == EnhanceTokenType.COMMON_TOKEN)
        {
            if (_selectedCharacterMaterial + _selectedCommonMaterial < _enhanceDragonCandyCost
                && _characterInfoController.UserDragonCandy > _selectedCommonMaterial)
            {
                _selectedCommonMaterial++;
                SetTextInputField(_selectedCommonMaterial);
            } 
        }
    }
    
    /// <summary>
    /// 감소 버튼 클릭 기능 동작
    /// </summary>
    private void DecreaseToken()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;
        
        if (_curTokenType == EnhanceTokenType.CHARACTER_TOKEN)
        {
            if (_selectedCharacterMaterial > 0)
            {
                _selectedCharacterMaterial--;
                SetTextInputField(_selectedCharacterMaterial);
            }
        }
        else if (_curTokenType == EnhanceTokenType.COMMON_TOKEN)
        {
            if (_selectedCommonMaterial > 0)
            {
                _selectedCommonMaterial--;
                SetTextInputField(_selectedCommonMaterial);
            } 
        }

    }
    
    /// <summary>
    /// 텍스트 인풋 필드 입력 기능 동작
    /// </summary>
    /// <param name="value"></param>
    private void InputToken(string value)
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;
        
        if (_curTokenType == EnhanceTokenType.COMMON_TOKEN)
        {
            bool check = int.TryParse(value, out _selectedCommonMaterial);
            
            if ((_enhanceDragonCandyCost - _selectedCharacterMaterial) < _selectedCommonMaterial)
            {
                if (_characterInfoController.UserDragonCandy < (_enhanceDragonCandyCost - _selectedCharacterMaterial))
                {
                    _selectedCommonMaterial = _characterInfoController.UserDragonCandy;
                    SetTextInputField(_selectedCommonMaterial);
                }
                else
                {
                    _selectedCommonMaterial = (_enhanceDragonCandyCost - _selectedCharacterMaterial);
                    SetTextInputField(_selectedCommonMaterial);
                } 
            }
            else
            {
                if (_characterInfoController.UserDragonCandy < _selectedCommonMaterial)
                {
                    _selectedCommonMaterial = _characterInfoController.UserDragonCandy;
                    SetTextInputField(_selectedCommonMaterial);
                }
            }
        }
        else if (_curTokenType == EnhanceTokenType.CHARACTER_TOKEN)
        {
            bool check2 = int.TryParse(value, out _selectedCharacterMaterial);

            if ((_enhanceDragonCandyCost - _selectedCommonMaterial) < _selectedCharacterMaterial)
            {
                if (_curCharacterToken < (_enhanceDragonCandyCost - _selectedCommonMaterial))
                {
                    _selectedCharacterMaterial = _curCharacterToken;
                    SetTextInputField(_selectedCharacterMaterial);
                }
                else
                {
                    _selectedCharacterMaterial = (_enhanceDragonCandyCost - _selectedCommonMaterial);
                    SetTextInputField(_selectedCharacterMaterial);
                }
            }
            else
            {
                if (_curCharacterToken < _selectedCharacterMaterial)
                {
                    _selectedCharacterMaterial = _curCharacterToken;
                    SetTextInputField(_selectedCharacterMaterial);
                } 
            }
        } 
    }
    
    /// <summary>
    /// 강화 토큰 등록 팝업창 종료 동작
    /// </summary>
    private void TokenPopupConfirm()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;
        
        TokenCountTextUpdate();
        EnhanceCheck(); 
        _characterInfoController._infoUI._enhanceTokenPopup.SetActive(false);
    }
    
    /// <summary>
    /// 현재 선택 토큰 > 캐릭터 강화창 토큰 개수 설정
    /// </summary>
    private void TokenCountTextUpdate()
    {
        switch (_curTokenType)
        {
            case EnhanceTokenType.NONE:
                _selectedCharacterMaterial = 0;
                _selectedCommonMaterial = 0;
                SetTextInputField(0);
                SetTextCommonTokenCount();
                SetTextCharacterTokenCount(); 
                break;
            
            case EnhanceTokenType.COMMON_TOKEN:
                SetTextCommonTokenCount();
                break;
            
            case EnhanceTokenType.CHARACTER_TOKEN: 
                SetTextCharacterTokenCount();
                break;
            
            case EnhanceTokenType.AUTO:
                SetTextCommonTokenCount();
                SetTextCharacterTokenCount();
                break;
        } 
    }

    private void SetTextInputField(int value)
    {
        _characterInfoController._infoUI._tokenInputField.text = value.ToString();
    }
    
    private void SetTextCommonTokenCount()
    {
        _characterInfoController._infoUI._commonTokenCountText.text = _selectedCommonMaterial.ToString();
    }

    private void SetTextCharacterTokenCount()
    {
        _characterInfoController._infoUI._characterTokenCountText.text = _selectedCharacterMaterial.ToString();
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

        //_characterInfoController._infoUI._beforeUpGradeText.text = $"+{_beforeEnhanceLevel} 현재 능력치";
        _characterInfoController._infoUI._beforeAtkText.text = $"공격력 {_beforeAtk}";
        _characterInfoController._infoUI._beforeDefText.text = $"방어력 {_beforeDef}";
        _characterInfoController._infoUI._beforeHpText.text = $"체력 {_beforeHp}";
    }

    /// <summary>
    /// 강화 이후 정보
    /// </summary>
    private void AfterEnhance()
    {
        _afterEnhanceLevel = (_beforeEnhanceLevel + 1);
        _characterInfoController._infoUI._afterMax.SetActive(_characterData.Enhancement.Value >= 10 && _characterInfoController.CurInfoTabType == InfoTabType.ENHANCE);
        _characterInfoController._infoUI._beforeMax.SetActive(_beforeEnhanceLevel < 10 && _characterInfoController.CurInfoTabType == InfoTabType.ENHANCE);

        if (_afterEnhanceLevel > 10)
        {
            //강화 Max 단계 정보
            _characterInfoController._infoUI._afterMaxHpText.text = $"체력 : {_characterData.HpPointLeveled}";
            _characterInfoController._infoUI._afterMaxAtkText.text = $"공격력 : {_characterData.AttackPointLeveled}";
            _characterInfoController._infoUI._afterMaxDefText.text = $"방어력 : {_characterData.DefensePointLeveled}";

            _characterInfoController._infoUI._enhanceCoinText.text = "-";
            _characterInfoController._infoUI._enhanceMaterialText.text = "-";
            
            _characterInfoController._infoUI._mileageUseButton.interactable = false;
            SliderValue = 1f;
        }
        else
        {
            //다음 강화 스탯 정보
            _afterHp = Convert.ToInt32(_characterData.HpPointLeveled * (1f + 0.1f * _afterEnhanceLevel) / (1f + 0.1f * (_characterData.Enhancement.Value)));
            _afterAtk = Convert.ToInt32(_characterData.AttackPointLeveled * (1f + 0.1f * _afterEnhanceLevel) / (1f + 0.1f * (_characterData.Enhancement.Value)));
            _afterDef = Convert.ToInt32(_characterData.DefensePointLeveled * (1f + 0.1f * _afterEnhanceLevel) / (1f + 0.1f * (_characterData.Enhancement.Value)));

            //_characterInfoController._infoUI._afterUpGradeText.text = $"+{_afterEnhanceLevel} 강화 후 능력치";
            _characterInfoController._infoUI._afterHpText.text = _afterHp.ToString();
            _characterInfoController._infoUI._afterAtkText.text = _afterAtk.ToString();
            _characterInfoController._infoUI._afterDefText.text = _afterDef.ToString();
        }
    }

    /// <summary>
    /// 캐릭터 강화 기능
    /// </summary>
    private void Enhance()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;
         
        //누적 확률
        _cumulativeProbability = 0f;
        
        //최소 확률 > 90%
        _minEnhanceProbability = 0.1f;

        for (int i = 0; i <= _characterData.Enhancement.Value; i++)
        {
            _cumulativeProbability += _decreaseRate[i];
        }
        
        //강화 성공 확률
        _enhanceProbability = _minEnhanceProbability + GetProbability(_cumulativeProbability);

        //0 ~ 1f 사이
        _successProbability = GetProbability(Random.Range(0f, 1f)); 

        //Debug.Log($"누적확률:{_cumulativeProbability} / 성공 확률:{_enhanceProbability} / 나의 확률 :{_successProbability}");
        
        if (_successProbability > _enhanceProbability)
        {  
            DragonCandyDataUpdate(1, true);
        }
        else
        {  
            DragonCandyDataUpdate(0, false); 
        }
    }
    
    /// <summary>
    /// 강화 시도 시 재화 상태 업데이트
    /// </summary>
    private void DragonCandyDataUpdate(int enhanceValue,bool result)
    {
        UserDataInt _characterToken = GameManager.TableData
            .GetItemData(_characterInfoController.CurCharacterInfo._CharacterData.EnhanceItemID).Number;

        GameManager.UserData.StartUpdateStream()
            .SetDBValue(_characterData.Enhancement, _characterData.Enhancement.Value + enhanceValue)
            .SetDBValue(_characterInfoController.UserGoldData, _characterInfoController.UserGoldData.Value - _enhanceGoldCost)
            .SetDBValue(_characterInfoController.UserDragonCandyData, _characterInfoController.UserDragonCandyData.Value - _selectedCommonMaterial)
            .SetDBValue(_characterToken, _characterToken.Value - _selectedCharacterMaterial)
            .Submit(result ? EnhanceSuccess : EnhanceFail);  
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

        _curTokenType = EnhanceTokenType.NONE;
        _characterInfoController.UserCharacterToken = GameManager.TableData.GetItemData(_characterInfoController.CurCharacterInfo._CharacterData.EnhanceItemID).Number.Value;
        

        if (_curEnhanceType == EnhanceType.ENHANCE)
        {
            _characterInfoController._enhanceNormalEffect.Play();
            SoundManager.Instance.PlaySFX(_characterInfoController.CharacterEnhanceSounds[0]);
        }
        else if (_curEnhanceType == EnhanceType.MILEAGE)
        {
            _characterInfoController._enhanceSpecialEffect.Play();
            SoundManager.Instance.PlaySFX(_characterInfoController.CharacterEnhanceSounds[1]);
            _effectCo = StartCoroutine(EnhanceEffectTimer());
        }
        
        //TODO: FaceIcon 노출
        _characterInfoController._infoUI._faceIconImage.sprite = _characterInfoController._infoUI.FaceIcons[0];
        
        _faceEffectCo = StartCoroutine(FaceIconRoutine(_characterInfoController._infoUI._faceIconImage, _characterInfoController._infoUI._speechBubbleImage, true));
        
        TokenCountTextUpdate();
        ResultPopup($"+{_characterData.Enhancement.Value} 강화에 성공하셨습니다.");
        CharacterStats();
        UpdateInfo();
        EnhanceCheck();
    }

    private IEnumerator FaceIconRoutine(Image faceIcon, Image bubble, bool isResult)
    {
        float elapsedTime = 0f;
        int index = 0;
        
        Color color = faceIcon.color;
        
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;

            color = new Color(color.r, color.g, color.b, elapsedTime * 5f);
            faceIcon.color = color;
            bubble.color = color;
               
            yield return null;
        }

        Color textColor = _characterInfoController._infoUI._speechText.color;
        
        _characterInfoController._infoUI._speechText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
        _characterInfoController._infoUI._speechText.text = "";
        
        while (true)
        {
            if (isResult)
            {
                _characterInfoController._infoUI._speechText.text += _successTalk[index++];
                yield return new WaitForSeconds(0.1f);

                if (index >= _successTalk.Length) break;
            }
            else
            {
                _characterInfoController._infoUI._speechText.text += _failTalk[index++];
                yield return new WaitForSeconds(0.1f);

                if (index >= _failTalk.Length) break;
            }
            
        } 
    }
    
    private IEnumerator EnhanceEffectTimer()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _characterInfoController._enhanceSpecialEffect.Stop(); 
    }
    
    /// <summary>
    /// 강화 실패 후 
    /// </summary>
    private void EnhanceFail(bool result)
    {
        if (!result)
        {
            ResultPopup("강화 실패 \n 사유 : 네트워크 오류");
            return;
        }

        _curTokenType = EnhanceTokenType.NONE;
        _characterInfoController.UserCharacterToken = GameManager.TableData.GetItemData(_characterInfoController.CurCharacterInfo._CharacterData.EnhanceItemID).Number.Value;
        
        _characterInfoController._infoUI._faceIconImage.sprite = _characterInfoController._infoUI.FaceIcons[1];
        
        _faceEffectCo = StartCoroutine(FaceIconRoutine(_characterInfoController._infoUI._faceIconImage, _characterInfoController._infoUI._speechBubbleImage, false));
        SoundManager.Instance.PlaySFX(_characterInfoController.CharacterEnhanceSounds[2]);
        
        TokenCountTextUpdate();
        ResultPopup($"+{_characterData.Enhancement.Value + 1} 강화에 실패하셨습니다. \n 마일리지 적립 +{_increaseMileage[_characterData.Enhancement.Value]}%");
        CharacterStats();
        UpdateInfo();
        EnhanceCheck();
        MileageUpdate(_characterData.EnhanceMileagePerMill.Value + _increaseMileage[_characterData.Enhancement.Value]);
        _characterInfoController._infoUI._sliderFillImage.pixelsPerUnitMultiplier += _characterData.EnhanceMileagePerMill.Value * 0.01f;
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
                    ResultPopup("마일리지 적립 실패 \n 사유 : 네트워크 오류");
                    return;
                }
                 
                if (_characterData.Enhancement.Value >= 10)
                {
                    SliderValue = 1f;
                    _characterInfoController._infoUI._sliderFillImage.pixelsPerUnitMultiplier = 15;
                    _characterInfoController._infoUI._mileageValueText.text = "-"; 
                }
                else
                {
                    _characterInfoController._infoUI._mileageSlider.value = _mileage;
                    _characterInfoController._infoUI._mileageUseButton.interactable = _mileage >= 1f;
                    SliderValue = _mileage; 
                } 
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
        _characterInfoController._infoUI._mileageValueText.text = $"강화 마일리지 {Mathf.Floor(value * 100f)}%";
    }
    
    /// <summary>
    /// 마일리지 사용
    /// </summary>
    private void UseMileage()
    {
        if (_characterInfoController.CurCharacterEnhance != this) return;
        
        if (_effectCo != null)
        {
            StopCoroutine(_effectCo);
            _effectCo = null;
        }
        
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(_characterData.Enhancement, _characterData.Enhancement.Value + 1)
            .Submit(result =>
            {
                EnhanceSuccess(result);

                if (!result) return;
                MileageUpdate(0);
                _characterInfoController._infoUI._mileageUseButton.interactable = false;
                _characterInfoController._infoUI._mileageUsePopup.SetActive(false);
                _characterInfoController._infoUI._sliderFillImage.pixelsPerUnitMultiplier = 1;
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
        _characterInfoController.UserDragonCandy = _characterInfoController.UserDragonCandyData.Value;
        _curCharacterToken = GameManager.TableData.GetItemData(_characterData.EnhanceItemID).Number.Value;
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
        //TODO: 강화 코스트 수정 필요
        _enhanceDragonCandyCost = 100 * (_beforeEnhanceLevel + 1);
        _enhanceGoldCost = 1000 * (_beforeEnhanceLevel + 1);
        _characterInfoController._infoUI._enhanceCoinText.text = CurrencyFormat.Trans(_enhanceGoldCost);
        _characterInfoController._infoUI._enhanceMaterialText.text = CurrencyFormat.Trans(_enhanceDragonCandyCost);
    }

    /// <summary>
    /// 강화 가능 여부 체크
    /// </summary>
    private void EnhanceCheck()
    {
        //TODO: 활성화/비활성화 조건 수정 필요 현재는 테스트로 임시 ~

        if (_characterData.Enhancement.Value >= 10)
        {
            _characterInfoController._infoUI._enhanceCoinText.color = Color.white;
            _characterInfoController._infoUI._enhanceMaterialText.color = Color.white;
        }
        else
        { 
            _characterInfoController._infoUI._enhanceCoinText.color = _characterInfoController.UserGold >= _enhanceGoldCost ? color : Color.red;
            _characterInfoController._infoUI._enhanceMaterialText.color = _characterInfoController.UserDragonCandy + _curCharacterToken >= _enhanceDragonCandyCost ? color : Color.red;
        }
        
        _characterInfoController._infoUI._enhanceButton.interactable =
            _enhanceDragonCandyCost == (_selectedCharacterMaterial + _selectedCommonMaterial) &&
            _beforeEnhanceLevel < _maxEnhanceLevel &&
            _characterInfoController.UserGold >= _enhanceGoldCost &&
            _characterInfoController.UserDragonCandy + _curCharacterToken >= _enhanceDragonCandyCost;
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
        _characterInfoController._infoUI._mileageUseButton.interactable = _mileage >= 1f && characterData.Enhancement.Value < 10;
        SliderValue = _mileage;
        _characterInfoController._infoUI._sliderFillImage.pixelsPerUnitMultiplier = _mileage * 10f;
        
        _characterInfoController._infoUI._autoTokenButton.interactable = characterData.Enhancement.Value < 10;
        _characterInfoController._infoUI._characterTokenButton.interactable = characterData.Enhancement.Value < 10;
        _characterInfoController._infoUI._commonTokenButton.interactable = characterData.Enhancement.Value < 10;

        if (characterData.Enhancement.Value >= 10)
        {
            _characterInfoController._infoUI._mileageSlider.value = 1f;
            _characterInfoController._infoUI._mileageValueText.text = "-";
            _characterInfoController._infoUI._sliderFillImage.pixelsPerUnitMultiplier = 15;
        }
        
        SetEnhanceCost();
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