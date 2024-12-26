using System;
using System.Collections;
using System.Collections.Generic; 
using TMPro; 
using UnityEngine; 
using UnityEngine.UI; 
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class CharacterInfo : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CharacterData _characterData;
    [HideInInspector] public bool IsSubscribe;

    private CharacterInfoController _characterInfoController;

    private TextMeshProUGUI _characterListNameText;
    private Image _characterListImage;
    
    private float _minEnhanceProbability = 0.8f;
    private int _maxEnhanceLevel = 10;

    private int _characterLevel;
    private int _characterEnhanceLevel;
    
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

    #region 테스트코드

    [SerializeField] private int testMyGold;
     
    public int TestMyGold
    {
        get => testMyGold;
        set
        {
            testMyGold = value;
            LevelUpCheck();
        }
    }

    #endregion


    [SerializeField] private int characterLevelUpCost;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _characterListNameText = GetComponentInChildren<TextMeshProUGUI>();
        _characterListImage = transform.GetChild(0).GetComponent<Image>();
        _characterInfoController = GetComponentInParent<CharacterInfoController>();
        
        SetListNameText(_characterData.Name);
        SetListImage(_characterData.FaceIconSprite);
        SubscribeEvent();
        GetCharacterDBValue();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetInfoPopup();
    }

    private void GetCharacterDBValue()
    {
        _characterLevel = _characterData.Level.Value;
        characterLevelUpCost = 100 * _characterData.Level.Value; 
        _characterEnhanceLevel = _characterData.Enhancement.Value;  
    }
    
    private void SubscribeEvent()
    {
        if (IsSubscribe) return;
        IsSubscribe = true;

        _characterInfoController._infoUI._levelUpButton.onClick.AddListener(LevelUp);
        _characterInfoController._infoUI._enhanceButton.onClick.AddListener(Enhance);
    }
 
    /// <summary>
    /// 현재 캐릭터 정보 할당 기능
    /// </summary>
    private void SetInfoPopup()
    {
        _characterInfoController.CurCharacterInfo = this;
        _characterInfoController.CurIndex = _characterInfoController._characterInfos.IndexOf(this);
        _characterInfoController._infoPopup.SetActive(true);
        UpdateInfo();
    }

    /// <summary>
    /// 캐릭터 정보 업데이트
    /// </summary>
    public void UpdateInfo()
    {
        Debug.Log($"{gameObject.name} : {_characterData.name}");
        
        //TODO: 정리 필요 
        _characterInfoController._infoUI._nameText.text = _characterData.Name;
        _characterInfoController._infoUI._characterImage.sprite = _characterData.FaceIconSprite;
        _characterInfoController._infoUI._levelText.text = _characterData.Level.Value.ToString();
        _characterInfoController._infoUI._enhanceText.text = $"+{_characterData.Enhancement.Value.ToString()}";
        _characterInfoController._infoUI._atkText.text = "공격력" + Random.Range(2, 100).ToString();
        _characterInfoController._infoUI._hpText.text = "체력" + Random.Range(2, 100).ToString();
        _characterInfoController._infoUI._coinText.text = characterLevelUpCost.ToString(); 
        
        LevelUpCheck();
        EnhanceCheck();
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
            // .SetDBValue(_characterData.Level, _characterData.Level.Value + 1) // 재화 사용
            .Submit(LevelUpResult); 
    }
    
    /// <summary>
    /// 레벨업 결과
    /// </summary>
    /// <param name="result"></param>
    private void LevelUpResult(bool result)
    {
        if (false == result)
        {
            Debug.LogWarning("접속 실패");
            return;
        }
        //테스트 재화 사용
        TestMyGold -= characterLevelUpCost;
        characterLevelUpCost = 100 * _characterData.Level.Value; 
        UpdateInfo();

        // 레벨업 UI 나올 위치
    }

    /// <summary>
    /// 레벨업 가능 여부 체크
    /// </summary>
    private void LevelUpCheck()
    {
        //TODO: 골드 변수 수정 필요
        _characterInfoController._infoUI._levelUpButton.interactable =
            testMyGold >= characterLevelUpCost && testMyGold != 0;
    }
    
    /// <summary>
    /// 캐릭터 강화 기능
    /// </summary>
    private void Enhance()
    {
        if (_characterInfoController.CurCharacterInfo != this) return; 
         
        //기본 강화 확률 + 추가 재료 강화 확률 > Probability 보다 크면 성공
        //아니면 실패
        //현재 강화 레벨에 따라 최소 강화 확률 조정 필요
        _minEnhanceProbability = _characterEnhanceLevel == 1 ? 1f : (_maxEnhanceLevel - _characterEnhanceLevel) * 0.1f;
        
        Debug.Log($"현재 최소 확률 : {_minEnhanceProbability}");
         
        float enhanceProbability = GetProbability(Random.Range(_minEnhanceProbability, 1f));
        
        //내 캐릭터 강화 공식이 필요
        float chance = _characterEnhanceLevel == 1 ? 1f : GetProbability(Random.Range((enhanceProbability - 0.2f), (enhanceProbability + 0.2f)));
        chance = Mathf.Clamp(chance, 0.01f, 1f);
         
        if (chance >= enhanceProbability)
        {
            GameManager.UserData.StartUpdateStream()
                .SetDBValue(_characterData.Enhancement, _characterData.Enhancement.Value + 1)
                .Submit(EnhanceResult);
             
            Debug.Log($"{gameObject.name} :{_characterEnhanceLevel} 강화 성공 : 캐릭터 확률 {chance} / 성공 확률 {enhanceProbability}");
        }
        else
        {
            Debug.Log($"강화 실패 : 캐릭터 확률 {chance} / 성공 확률 {enhanceProbability}");
        } 
    }

    private void EnhanceResult(bool result)
    {
        if (!result)
        {
            Debug.Log("네트워크 오류");
            return;
        }

        _characterEnhanceLevel = _characterData.Enhancement.Value;
        
        UpdateInfo();
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
        Debug.Log(_characterEnhanceLevel);
        //TODO: 활성화/비활성화 조건 수정 필요
        //테스트 강화 비활성화 조건
        _characterInfoController._infoUI._enhanceButton.interactable = _characterEnhanceLevel < _maxEnhanceLevel;
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
    
}