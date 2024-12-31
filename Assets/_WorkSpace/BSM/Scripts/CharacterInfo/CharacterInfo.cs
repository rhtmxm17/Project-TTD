using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CharacterInfo : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CharacterData _characterData;

    private CharacterInfoController _characterInfoController;

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

    private int _userGold;
    public int UserGold
    {
        get => _userGold;
        set
        {
            _userGold = value;
            LevelUpCheck();
        }
    }

    private int characterLevelUpCost;

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
    /// 현재 캐릭터 정보 할당 기능
    /// </summary>
    private void SetInfoPopup()
    {  
        _characterInfoController.CurCharacterInfo = this;
        _characterInfoController.CurCharacterEnhance = _characterEnhance; 
        _characterInfoController.CurIndex = _characterInfoController._characterInfos.IndexOf(this);
        _characterInfoController._infoPopup.SetActive(true);
        UpdateInfo();
    }
    
    /// <summary>
    /// 캐릭터 레벨업 기능
    /// </summary>
    private void LevelUp()
    {
        //오픈한 캐릭터 정보가 구독된 리스트중 자신과 같지 않으면 return
        if (_characterInfoController.CurCharacterInfo != this) return;
        
        //TODO: 임시 캐릭터 레벨업 코스트
        characterLevelUpCost = 100 * _characterLevel;
        
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(_characterData.Level, _characterData.Level.Value + 1)
            .SetDBValue(_characterInfoController.UserGoldData, _characterInfoController.UserGoldData.Value - characterLevelUpCost) // 재화 사용
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
        Debug.Log($"{_characterData.name} 레벨업 성공");
        //테스트 재화 사용
        _characterInfoController.UserGold = _characterInfoController.UserGoldData.Value;
        CharacterStats();
        UpdateInfo();

        // 레벨업 UI 나올 위치
    }

    /// <summary>
    /// 캐릭터 정보 업데이트
    /// </summary>
    public void UpdateInfo()
    {
        _characterEnhance.GetCharacterData(_characterData);
        _characterEnhance.OnBeforeEnhance?.Invoke();
        _characterEnhance.OnAfterEnhance?.Invoke(); 
        LevelUpCheck();
        CharacterStats();
        
        _characterInfoController._infoUI._nameText.text = _characterData.Name;
        _characterInfoController._infoUI._characterImage.sprite = _characterData.FaceIconSprite;
        _characterInfoController._infoUI._levelText.text = _characterData.Level.Value.ToString();
        _characterInfoController._infoUI._enhanceText.text = $"+{_characterData.Enhancement.Value.ToString()}";
        _characterInfoController._infoUI._powerLevelText.text = $"전투력 {_powerLevel}";
        _characterInfoController._infoUI._atkText.text = $"공격력 {_atk}";
        _characterInfoController._infoUI._hpText.text = $"체력 {_hp}";
        _characterInfoController._infoUI._defText.text = $"방어력 {_def}";
        _characterInfoController._infoUI._coinText.text = characterLevelUpCost.ToString();

        _characterInfoController._infoUI._skillAIconImage.sprite = _characterData.NormalSkillIcon;
        _characterInfoController._infoUI._skillBIconImage.sprite = _characterData.SpecialSkillIcon;

        _characterInfoController._infoUI._skillADescText.text = _characterData.NormalSkillToolTip;
        _characterInfoController._infoUI._skillBDescText.text = _characterData.SpecialSkillToolTip;
        
        //TODO: 임시 텍스트 -> 속성 이미지로 변경 필요
        int tempType = (int)_characterData.StatusTable.type;
        _characterInfoController._infoUI._tempElemetTypeText.text = ((ElementType)tempType).ToString();
    }
  
    /// <summary>
    /// 현재 케릭터 레벨 별 스탯 반영
    /// </summary>
    private void CharacterStats()
    {
        _characterLevel = _characterData.Level.Value;

        _hp = _characterLevel *
              (int)(_characterData.StatusTable.healthPointBase + _characterData.StatusTable.healthPointGrouth);
        _atk = _characterLevel *
               (int)(_characterData.StatusTable.attackPointBase + _characterData.StatusTable.attackPointGrowth);
        _def = _characterLevel *
               (int)(_characterData.StatusTable.defensePointBase + _characterData.StatusTable.defensePointBase);
        
        _powerLevel = (_hp + _atk + _def);
    }

    public void SetCharacterData()
    {
        CharacterStats();
    }
    
    /// <summary>
    /// 레벨업 가능 여부 체크
    /// </summary>
    private void LevelUpCheck()
    {
        //TODO: 골드 변수 수정 필요
        _characterInfoController._infoUI._levelUpButton.interactable = _characterInfoController.UserGold >= characterLevelUpCost;
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

    public void SetListTypeText(string type)
    {
        _characterTypeText.text = type;
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
    }
}