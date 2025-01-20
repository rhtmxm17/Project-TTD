using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = System.Random;

public class CharacterInfo : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public List<CharacterModel> CharacterModels;
    [HideInInspector] public GameObject OwnedObject;
    [SerializeField] private CharacterData _characterData;
 
    private CharacterInfoController _characterInfoController;

    private CharacterModel levelOne;
    private CharacterModel levelTwo;
    private CharacterModel levelThree;

    private TextMeshProUGUI _characterTypeText;
    private TextMeshProUGUI _characterListNameText;
    private Image _characterListImage;
    
    
    private bool _isSubscribe;
    private int _characterLevel = 1;

    private int _hp;
    private int _atk;
    private int _def;
    private int _powerLevel;

    public int PowerLevel
    {
        get => _powerLevel;
        set
        {
            _powerLevel = value;
        }
    }

    public int CharacterLevel
    {
        //현재 캐릭터 레벨 반환
        get => _characterData.Level.Value;
    }

    public string CharacterName
    {
        //현재 캐릭터 이름 반환
        get => _characterData.Name;
    }

    public CharacterData _CharacterData
    {
        //현재 캐릭터 데이터 반환 및 데이터 변경
        get => _characterData;
        set { _characterData = value; }
    }

    private Coroutine _levelUpNormalEffectCo;
    private Coroutine _levelUpSpecialEffectCo;
    private CharacterEnhance _characterEnhance;
    
    private int _characterLevelUpGoldCost;
    private int _characterLevelUpYongGwaCost;
    
    private void Awake()
    {
        _characterEnhance = GetComponent<CharacterEnhance>(); 
        _characterInfoController = GetComponentInParent<CharacterInfoController>();

    }
   
    private void Start()
    {
        ButtonOnClickEvent();
        CharacterModel();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetInfoPopup();
    }

    private void ButtonOnClickEvent()
    {
        if (_isSubscribe) return;
        _isSubscribe = true;
        
        _characterInfoController._infoUI._levelUpButton.onClick.AddListener(LevelUp);
    }
 
    /// <summary>
    /// 캐릭터 레벨업 기능
    /// </summary>
    private void LevelUp()
    {
        //오픈한 캐릭터 정보가 구독된 리스트중 자신과 같지 않으면 return
        if (_characterInfoController.CurCharacterInfo != this) return;
        
        if (_characterInfoController._infoUI._bonusPopup.activeSelf)
        {
            _characterInfoController._infoUI._bonusPopup.SetActive(false);
            return;
            ;
        }
        
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(_characterData.Level, _characterData.Level.Value + 1)
            .SetDBValue(_characterInfoController.UserGoldData, _characterInfoController.UserGoldData.Value - _characterLevelUpGoldCost) // 재화 사용
            .SetDBValue(_characterInfoController.UserYongGwaData, _characterInfoController.UserYongGwaData.Value - _characterLevelUpYongGwaCost)
            .Submit(LevelUpSuccess);
    }
    
    /// <summary>
    /// 레벨업 결과
    /// </summary>
    /// <param name="result"></param>
    private void LevelUpSuccess(bool result)
    {
        if (false == result)
        {
            Debug.LogWarning("접속 실패");
            return;
        }

        if (_levelUpNormalEffectCo != null)
        {
            _characterInfoController._infoUI._levelUpNormalEffect.color =new Color(1, 1, 1, 0f);
            StopCoroutine(_levelUpNormalEffectCo);
            _levelUpNormalEffectCo = null;
        }

        if (_levelUpSpecialEffectCo != null)
        {
            _characterInfoController._infoUI._levelUpSpecialEffect.color = new Color(1, 1, 1, 0f);
            StopCoroutine(_levelUpSpecialEffectCo);
            _levelUpSpecialEffectCo = null;
        }
        
        _characterInfoController.UserGold = _characterInfoController.UserGoldData.Value;
        _characterInfoController.UserYongGwa = _characterInfoController.UserYongGwaData.Value;

        UpdateInfo(); 

        //레벨업 시 이펙트 재생 및 추가 스탯 UI
        if (_characterLevel % 10 != 0)
        {
            if (_levelUpNormalEffectCo == null)
            {
                _levelUpNormalEffectCo = StartCoroutine(LevelUpNormalEffectRoutine(_characterInfoController._infoUI._levelUpNormalEffect, 0.25f));
            } 
        }
        else
        {
            if (_levelUpSpecialEffectCo == null)
            {
                _levelUpSpecialEffectCo = StartCoroutine(LevelUpSpecialffectRoutine(_characterInfoController._infoUI._levelUpSpecialEffect, 0.5f));
                _characterInfoController._infoUI._bonusPopup.SetActive(true);
                _characterInfoController._infoUI._bonusLevelText.text = $"{_characterLevel}레벨 달성!";

                // 레벨당 성장치
                float EnhancementGrouth = 1f + 0.05f * _characterData.Enhancement.Value; // 강화 증폭 배율
                float atkBonused = (int)(_characterData.StatusTable.attackPointGrowth * EnhancementGrouth);
                float defBonused = (int)(_characterData.StatusTable.defensePointGrouth * EnhancementGrouth);
                float hpBonused = (int)(_characterData.StatusTable.healthPointGrouth * EnhancementGrouth);

                //레벨업 전 정보 (6레벨 분량 성장치 차감)
                _characterInfoController._infoUI._beforeBonusAtkText.text =
                    $"공격력 {(int)(_characterData.AttackPointLeveled - (1f + (1f + CharacterData.BonusGrouthLevel)) * atkBonused)}";
                _characterInfoController._infoUI._beforeBonusDefText.text =
                    $"방어력 {(int)(_characterData.DefensePointLeveled - (1f + CharacterData.BonusGrouthLevel) * defBonused)}";
                _characterInfoController._infoUI._beforeBonusHpText.text =
                    $"체력 {(int)(_characterData.HpPointLeveled - (1f + CharacterData.BonusGrouthLevel) * hpBonused)}";
                
                //레벨업 후 정보 (5레벨 분량 성장치 차감 + 5레벨 분량 성장치)
                _characterInfoController._infoUI._afterBonusAtkText.text =
                    $"공격력 {(int)(_characterData.AttackPointLeveled - CharacterData.BonusGrouthLevel * atkBonused)} + {(int)(CharacterData.BonusGrouthLevel * atkBonused)}";

                _characterInfoController._infoUI._afterBonusDefText.text =
                    $"방어력 {(int)(_characterData.DefensePointLeveled - CharacterData.BonusGrouthLevel * defBonused)} + {(int)(CharacterData.BonusGrouthLevel * defBonused)}";
                
                _characterInfoController._infoUI._afterBonusHpText.text =
                    $"체력 {(int)(_characterData.HpPointLeveled - CharacterData.BonusGrouthLevel * hpBonused)} + {(int)(CharacterData.BonusGrouthLevel * hpBonused)}";

            } 
        }
    }
     
    /// <summary>
    /// 레벨업 이펙트 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator LevelUpNormalEffectRoutine(Image effectImage ,float value)
    {
        float elapsedTime = 0f;
        float endTime = value;

        while (elapsedTime <= endTime)
        {
            float r = UnityEngine.Random.Range(0, 1f);
            float g = UnityEngine.Random.Range(0, 1f);
            float b = UnityEngine.Random.Range(0, 1f);
            
            elapsedTime += Time.deltaTime;
            effectImage.color = new Color(r, g, b, elapsedTime + 0.35f);
            yield return null;
        }

        while (elapsedTime > 0)
        {
            float r = UnityEngine.Random.Range(0, 1f);
            float g = UnityEngine.Random.Range(0, 1f);
            float b = UnityEngine.Random.Range(0, 1f);
            
            elapsedTime -= Time.deltaTime;
            effectImage.color = new Color(r, g, b, elapsedTime);
            yield return null;
        } 
    }
    
    private IEnumerator LevelUpSpecialffectRoutine(Image effectImage ,float value)
    {
        float elapsedTime = 0f;
        float endTime = value;

        while (elapsedTime <= endTime)
        {
            float r = UnityEngine.Random.Range(0, 1f);
            float g = UnityEngine.Random.Range(0, 1f);
            float b = UnityEngine.Random.Range(0, 1f);
            
            elapsedTime += Time.deltaTime;
            effectImage.color = new Color(r, g, b, elapsedTime + 0.35f);
            yield return null;
        }

        while (elapsedTime >= 0)
        {
            float r = UnityEngine.Random.Range(0, 1f);
            float g = UnityEngine.Random.Range(0, 1f);
            float b = UnityEngine.Random.Range(0, 1f);
            
            elapsedTime -= Time.deltaTime;
            effectImage.color = new Color(r, g, b, elapsedTime);
            yield return null;
        } 
    }
    
    
    /// <summary>
    /// 현재 캐릭터 정보 할당 기능
    /// </summary>
    private void SetInfoPopup()
    { 
        _characterInfoController.CurCharacterInfo = this;
        _characterInfoController.CurCharacterEnhance = _characterEnhance; 
        _characterInfoController.CurIndex = _characterInfoController._characterInfos.IndexOf(this);
        _characterInfoController._infoPopup.SetActive(true);
        
        _characterInfoController.UserGold = _characterInfoController.UserGoldData.Value;
        _characterInfoController.UserYongGwa = _characterInfoController.UserYongGwaData.Value;
        UpdateInfo();
    }
 
    /// <summary>
    /// 각 레벨 이미지 호출
    /// </summary>
    private void CharacterModel()
    {
        if (_characterData.ModelPrefab != null && CharacterModels.Count == 0)
        {
            levelOne = CreateModelPrefab(_characterData.ModelPrefab, false);

            if (levelOne.NextEvolveModel != null)
            {
                levelTwo = CreateModelPrefab(levelOne.NextEvolveModel, false);
            }

            if (levelTwo != null && levelTwo.NextEvolveModel != null)
            {
                levelThree = CreateModelPrefab(levelTwo.NextEvolveModel, false);
            }
        }
        else if (CharacterModels.Count != 0 && !CharacterModels[0].gameObject.activeSelf)
        {
            CharacterModels[0].gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// 레벨 별 이미지 생성
    /// </summary>
    /// <param name="model">생성할 캐릭터 모델</param>
    /// <param name="pos">생성될 위치</param>
    /// <param name="scale">생성될 크기</param>
    /// <param name="active">활성화 여부</param>
    /// <returns></returns>
    private CharacterModel CreateModelPrefab(CharacterModel model, bool active)
    {
        model = Instantiate(model, _characterInfoController.ModelParent.transform);
        model.gameObject.SetActive(active);
        CharacterModels.Add(model);
        return model;
    }
    
    /// <summary>
    /// 캐릭터 정보 업데이트
    /// </summary>
    public void UpdateInfo()
    {
        _characterEnhance.GetCharacterData(_characterData);
        _characterEnhance.OnBeforeEnhance?.Invoke();
        _characterEnhance.OnAfterEnhance?.Invoke();
        _characterInfoController.UserYongGwa = _characterInfoController.UserYongGwaData.Value;
        CharacterStats();
        
        _characterInfoController._infoUI._evolutionTabButton.interactable = CharacterModels.Count != 0;

        if (GameManager.UserData.HasCharacter(_characterData.Id))
        {
            SetCurrentCost();
            LevelUpCheck();

            _characterInfoController._infoUI._levelUpCoinText.text = CurrencyFormat.Trans(_characterLevelUpGoldCost);
            _characterInfoController._infoUI._levelUpYongGwaText.text = CurrencyFormat.Trans(_characterLevelUpYongGwaCost);
            _characterInfoController._infoUI._materialGroup.SetActive(true);
            _characterInfoController._infoUI._levelUpButton.gameObject.SetActive(true);
            _characterInfoController._infoUI._amountGroup.SetActive(true);
            _characterInfoController._infoUI._enhanceTabButton.interactable = true;
        }
        else
        {
            _characterInfoController._infoUI._amountGroup.SetActive(false);
            _characterInfoController._infoUI._materialGroup.SetActive(false);
            _characterInfoController._infoUI._levelUpButton.gameObject.SetActive(false);
            _characterInfoController._infoUI._enhanceTabButton.interactable = false;
        }
        
        _characterInfoController._infoUI._nameText.text = _characterData.Name;
        _characterInfoController._infoUI._characterImage.sprite = _characterData.FaceIconSprite;
        _characterInfoController._infoUI._levelText.text = $"Lv. {_characterData.Level.Value}";
        _characterInfoController._infoUI._enhanceText.text = $"+{_characterData.Enhancement.Value.ToString()}";
        _characterInfoController._infoUI._powerLevelText.text = $"전투력 {_powerLevel}";
        _characterInfoController._infoUI._atkText.text = $"공격력 {_atk}";
        _characterInfoController._infoUI._hpText.text = $"체력 {_hp}";
        _characterInfoController._infoUI._defText.text = $"방어력 {_def}";
 
        _characterInfoController._infoUI._skillAIconImage.sprite = _characterData.NormalSkillIcon;
        _characterInfoController._infoUI._skillBIconImage.sprite = _characterData.SpecialSkillIcon;
        
        _characterInfoController._infoUI._skillNormalTitleText.text = _characterData.NormalSkillName;
        _characterInfoController._infoUI._skillSpecialTitleText.text = _characterData.SpecialSkillName; 
        
        _characterInfoController._infoUI._skillNormalDescText.text = _characterData.NormalSkillToolTip;
        _characterInfoController._infoUI._skillSpecialDescText.text = _characterData.SpecialSkillToolTip;
        
        //TODO: 역할군, 용맥 아이콘도 추가 필요
        ElementType elementType = _characterData.StatusTable.type;
        RoleType roleType = _characterData.StatusTable.roleType;
        DragonVeinType dragonVeinType = _characterData.StatusTable.dragonVeinType;

        _characterInfoController._infoUI._elementFrameImage.sprite =  _characterInfoController._infoUI.ElementFrames[(int)elementType - 1];
        _characterInfoController._infoUI._elementIconImage.sprite = _characterInfoController._infoUI._elementIcons[(int)elementType - 1];
        _characterInfoController._infoUI._roleIconImage.sprite =  _characterInfoController._infoUI._roleIcons[(int)roleType - 1];
        _characterInfoController._infoUI._dragonVeinIconImage.sprite =  _characterInfoController._infoUI._dragonVeinIcons[(int)dragonVeinType - 1];
        
        _characterInfoController._infoUI._ElemetTypeText.text = elementType switch
        {
            ElementType.FIRE => "화룡",
            ElementType.WATER => "수룡",
            ElementType.WIND => "정룡",
            ElementType.EARTH => "토룡",
            ElementType.METAL => "진룡",
            _ => throw new AggregateException("잘못됨")
        };

        _characterInfoController._infoUI._roleTypeText.text = roleType switch
        {
            RoleType.ATTACKER => "공격형",
            RoleType.DEFENDER => "방어형",
            RoleType.SUPPORTER => "지원형",
            _ => throw new AggregateException("잘못")
        };

        _characterInfoController._infoUI._dragonVeinTypeText.text = dragonVeinType switch
        {
            DragonVeinType.SINGLE => "단일",
            DragonVeinType.MULTI => "범위",
            _ => throw new AggregateException("잘못")
        };

        
        
    }
  
    /// <summary>
    /// 현재 케릭터 레벨 별 스탯 반영
    /// </summary>
    private void CharacterStats()
    {
        _characterLevel = _characterData.Level.Value; 
        _hp = Convert.ToInt32(_characterData.HpPointLeveled);
        _atk = Convert.ToInt32(_characterData.AttackPointLeveled);
        _def = Convert.ToInt32(_characterData.DefensePointLeveled); 
        _powerLevel = Convert.ToInt32(_characterData.PowerLevel);
    }
    
    /// <summary>
    /// 캐릭터 레벨업 비용
    /// </summary>
    private void SetCurrentCost()
    {
        //TODO: 임시 캐릭터 레벨업 코스트
        _characterLevelUpGoldCost = 1000 * _characterData.Level.Value;
        _characterLevelUpYongGwaCost = 50 * _characterData.Level.Value; 
    }
    
    /// <summary>
    /// 외부에서 사용하기 위한 현재 케릭터 정보 셋팅
    /// </summary>
    public void SetCharacterData()
    {
        CharacterStats();
    }
    
    /// <summary>
    /// 레벨업 가능 여부 체크
    /// </summary>
    private void LevelUpCheck()
    {
        _characterInfoController._infoUI._levelUpCoinText.color =
            _characterInfoController.UserGold >= _characterLevelUpGoldCost ? Color.white : Color.red;

        _characterInfoController._infoUI._levelUpYongGwaText.color =
            _characterInfoController.UserYongGwa >=  _characterLevelUpYongGwaCost? Color.white : Color.red;
        
        _characterInfoController._infoUI._levelUpButton.interactable = _characterInfoController.UserGold >= _characterLevelUpGoldCost && _characterInfoController.UserYongGwa >= _characterLevelUpYongGwaCost;
    }

    /// <summary>
    /// 캐릭터 리스트 이름 설정
    /// </summary>
    /// <param name="name"></param>
    public void SetListNameText(string name)
    {
        _characterListNameText.text = name;
    }

    /// <summary>
    /// 캐릭터 리스트 이미지 설정
    /// </summary>
    /// <param name="sprite"></param>
    public void SetListImage(Sprite sprite)
    {
        _characterListImage.sprite = sprite;
    }
    
    
    /// <summary>
    /// 캐릭터 속성 설정
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="AggregateException"></exception>
    public void SetListTypeText(ElementType type)
    {
        _characterTypeText.text = type switch
        {
            ElementType.FIRE => "화룡",
            ElementType.WATER => "수룡",
            ElementType.WIND => "정룡",
            ElementType.EARTH => "토룡",
            ElementType.METAL => "진룡",
            _ => "무속성"
        };
    }
    
    /// <summary>
    /// 캐릭터 레벨업 치트 기능
    /// </summary>
    /// <param name="level"></param>
    public void LevelUpCheat(int level)
    {
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(_characterData.Level, level)
            .Submit(LevelUpSuccess);
    }
 
    /// <summary>
    /// 캐릭터 리스트 오픈 시 CharacterSort 에서 UI 설정
    /// </summary>
    public void StartSetCharacterUI()
    { 
        _characterListImage = transform.GetChild(0).GetComponent<Image>();
        _characterListNameText = transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        _characterTypeText = transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
        OwnedObject = transform.GetChild(3).gameObject;
    }
}