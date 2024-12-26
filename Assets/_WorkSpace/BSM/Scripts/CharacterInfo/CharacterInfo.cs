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


    private CharacterInfoController _characterInfoController;

    private TextMeshProUGUI _characterListNameText;
    private Image _characterListImage;
    
    private bool _isSubscribe;
    private int _characterLevel;
    
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
        
        _characterEnhance = GetComponent<CharacterEnhance>();
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
        
        Debug.Log($"{_characterData.name} : Lv.{_characterLevel} / {_characterData.Enhancement.Value}");
    }
    
    private void SubscribeEvent()
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
    /// 캐릭터 정보 업데이트
    /// </summary>
    public void UpdateInfo()
    { 
        Debug.Log($"인포 :{_characterData.name} {_characterData.Enhancement.Value}");
        _characterEnhance.GetCharacterData(_characterData);
        
        //TODO: 정리 필요 
        _characterInfoController._infoUI._nameText.text = _characterData.Name;
        _characterInfoController._infoUI._characterImage.sprite = _characterData.FaceIconSprite;
        _characterInfoController._infoUI._levelText.text = _characterData.Level.Value.ToString();
        _characterInfoController._infoUI._enhanceText.text = $"+{_characterData.Enhancement.Value.ToString()}";
        _characterInfoController._infoUI._atkText.text = "공격력" + Random.Range(2, 100).ToString();
        _characterInfoController._infoUI._hpText.text = "체력" + Random.Range(2, 100).ToString();
        _characterInfoController._infoUI._coinText.text = characterLevelUpCost.ToString(); 
        Debug.Log("인포 업뎃 완료");
        
        LevelUpCheck();
        //EnhanceCheck();
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