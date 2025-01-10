using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterInfo : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public List<CharacterModel> CharacterModels;
    [HideInInspector] public GameObject OwnedObject;
    [SerializeField] private CharacterData _characterData;
 
    private CharacterInfoController _characterInfoController;

    private CharacterModel levelOne;
    private CharacterModel levelTwo;
    private CharacterModel levelThree;
    private Vector3 modelPos = new Vector3(-30f, 40f, 1f);
    private Vector3 levelOneScale = new Vector3(400f, 500f, 1f);
    private Vector3 levelTwoScale = new Vector3(350f, 400f, 1f);
    private Vector3 levelThreeScale = new Vector3(250f, 250f, 1f);
    
    
    private TextMeshProUGUI _characterTypeText;
    private TextMeshProUGUI _characterListNameText;
    private Image _characterListImage;
    
    
    private bool _isSubscribe;
    private int _characterLevel = 1;

    private int _hp;

    public int Hp
    {
        get => _hp;
        set { _hp = value; }
    }

    private int _atk;

    public int Atk
    {
        get => _atk;
        set { _atk = value; }
    }

    private int _def;

    public int Def
    {
        get => _def;
        set { _def = value; }
    }

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

    private CharacterEnhance _characterEnhance;
    
    private int _characterLevelUpGoldCost;
    private int _characterLevelUpMaterialCost;
    
    private void Awake()
    {
        _characterEnhance = GetComponent<CharacterEnhance>(); 
        _characterInfoController = GetComponentInParent<CharacterInfoController>();

    }
   
    private void Start()
    {
        ButtonOnClickEvent();
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

        GameManager.UserData.StartUpdateStream()
            .SetDBValue(_characterData.Level, _characterData.Level.Value + 1)
            .SetDBValue(_characterInfoController.UserGoldData, _characterInfoController.UserGoldData.Value - _characterLevelUpGoldCost) // 재화 사용
            .SetDBValue(_characterInfoController.UserLevelMaterialData, _characterInfoController.UserGoldData.Value - _characterLevelUpMaterialCost)
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

        _characterInfoController.UserGold = _characterInfoController.UserGoldData.Value;
        _characterInfoController.UserLevelMaterial = _characterInfoController.UserLevelMaterialData.Value;
        UpdateInfo(); 
        //TODO: 레벨업 UI 나올 위치
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
 
        CharacterModel();
        UpdateInfo();
    }
 
    /// <summary>
    /// 각 레벨 이미지 호출
    /// </summary>
    private void CharacterModel()
    {
        if (_characterInfoController.CurCharacterInfo == this)
        {
            if (_characterData.ModelPrefab != null && CharacterModels.Count == 0)
            {
                levelOne = CreateModelPrefab(_characterData.ModelPrefab, modelPos, levelOneScale, true);

                if (levelOne.NextEvolveModel != null)
                {
                    levelTwo = CreateModelPrefab(levelOne.NextEvolveModel, modelPos, levelTwoScale, false);
                }

                if (levelTwo != null && levelTwo.NextEvolveModel != null)
                {
                    levelThree = CreateModelPrefab(levelTwo.NextEvolveModel, modelPos, levelThreeScale, false);
                }
            }
            else if (CharacterModels != null && !CharacterModels[0].gameObject.activeSelf)
            {
                CharacterModels[0].gameObject.SetActive(true);
            }
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
    private CharacterModel CreateModelPrefab(CharacterModel model, Vector3 pos, Vector3 scale, bool active)
    {
        model = Instantiate(model, pos, Quaternion.identity);
        model.transform.localScale = scale;
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
        CharacterStats();
        
        if (GameManager.UserData.HasCharacter(_characterData.Id))
        {
            SetCurrentCost();
            LevelUpCheck();
            
            _characterInfoController._infoUI._levelUpCoinText.text = _characterLevelUpGoldCost.ToString();
            _characterInfoController._infoUI._levelUpMaterialText.text = _characterLevelUpMaterialCost.ToString();
            _characterInfoController._infoUI._materialGroup.SetActive(true);
            _characterInfoController._infoUI._levelUpButton.gameObject.SetActive(true);
            _characterInfoController._infoUI._enhanceTabButton.interactable = true;
        }
        else
        {
            _characterInfoController._infoUI._materialGroup.SetActive(false);
            _characterInfoController._infoUI._levelUpButton.gameObject.SetActive(false);
            _characterInfoController._infoUI._enhanceTabButton.interactable = false;
        }
        
        _characterInfoController._infoUI._beforeNameText.text = _characterData.Name;
        _characterInfoController._infoUI._beforeEnhanceLevelText.text =$"+{_characterData.Enhancement.Value.ToString()}";
        
        _characterInfoController._infoUI._nameText.text = _characterData.Name;
        _characterInfoController._infoUI._characterImage.sprite = _characterData.FaceIconSprite;
        _characterInfoController._infoUI._levelText.text = _characterData.Level.Value.ToString();
        _characterInfoController._infoUI._enhanceText.text = $"+{_characterData.Enhancement.Value.ToString()}";
        _characterInfoController._infoUI._powerLevelText.text = $"전투력 {_powerLevel}";
        _characterInfoController._infoUI._atkText.text = $"공격력 {_atk}";
        _characterInfoController._infoUI._hpText.text = $"체력 {_hp}";
        _characterInfoController._infoUI._defText.text = $"방어력 {_def}";
 
        _characterInfoController._infoUI._skillAIconImage.sprite = _characterData.NormalSkillIcon;
        _characterInfoController._infoUI._skillBIconImage.sprite = _characterData.SpecialSkillIcon;

        _characterInfoController._infoUI._skillADescText.text = _characterData.NormalSkillToolTip;
        _characterInfoController._infoUI._skillBDescText.text = _characterData.SpecialSkillToolTip;
        
        //TODO: 임시 텍스트 -> 속성 이미지로 변경 필요
        ElementType tempElementType = (ElementType)_characterData.StatusTable.type;
        RoleType tempRoleType = (RoleType)_characterData.StatusTable.roleType;
        DragonVeinType tempDragonVeinType = (DragonVeinType)_characterData.StatusTable.dragonVeinType;

        _characterInfoController._infoUI._tempElemetTypeText.text = tempElementType switch
        {
            ElementType.FIRE => "화룡",
            ElementType.WATER => "수룡",
            ElementType.WOOD => "정룡",
            ElementType.EARTH => "토룡",
            ElementType.METAL => "진룡",
            _ => throw new AggregateException("잘못됨")
        };

        _characterInfoController._infoUI._tempRoleTypeText.text = tempRoleType switch
        {
            RoleType.ATTACKER => "공격형",
            RoleType.DEFENDER => "방어형",
            RoleType.SUPPORTER => "지원형",
            _ => throw new AggregateException("잘못")
        };

        _characterInfoController._infoUI._tempDragonVeinTypeText.text = tempDragonVeinType switch
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
        _characterLevelUpGoldCost = 100 * _characterData.Level.Value;
        _characterLevelUpMaterialCost = 250 * _characterData.Level.Value; 
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

        _characterInfoController._infoUI._levelUpMaterialText.color =
            _characterInfoController.UserLevelMaterial >=  _characterLevelUpMaterialCost? Color.white : Color.red;
        
        _characterInfoController._infoUI._levelUpButton.interactable = _characterInfoController.UserGold >= _characterLevelUpGoldCost && _characterInfoController.UserLevelMaterial >= _characterLevelUpMaterialCost;
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
            ElementType.WOOD => "정룡",
            ElementType.EARTH => "토룡",
            ElementType.METAL => "진룡",
            _ => throw new AggregateException("잘못된 타입")
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
        _characterListNameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _characterTypeText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        OwnedObject = transform.GetChild(3).gameObject;
    }
}