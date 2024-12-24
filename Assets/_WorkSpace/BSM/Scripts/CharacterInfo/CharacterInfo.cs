using System;
using System.Collections;
using System.Collections.Generic; 
using TMPro; 
using UnityEngine; 
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class CharacterInfo : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CharacterData _characterData;
    [HideInInspector] public bool IsSubscribe;

    private CharacterInfoController _characterInfoController;

    private TextMeshProUGUI _characterNameText;

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
        _characterNameText = GetComponentInChildren<TextMeshProUGUI>();
        _characterInfoController = GetComponentInParent<CharacterInfoController>();

        _characterNameText.text = _characterData.Name;
        SubscribeEvent();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetInfoPopup();
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
        //TODO: 정리 필요 
        _characterInfoController._infoUI._nameText.text = _characterData.Name;
        _characterInfoController._infoUI._characterImage.sprite = _characterData.FaceIconSprite;
        _characterInfoController._infoUI._levelText.text = _characterData.Level.Value.ToString();
        _characterInfoController._infoUI._atkText.text = "공격력" + Random.Range(2, 100).ToString();
        _characterInfoController._infoUI._hpText.text = "체력" + Random.Range(2, 100).ToString();

        LevelUpCheck();
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
        
        TestMyGold -= characterLevelUpCost;
        characterLevelUpCost = 100 * _characterData.Level.Value;
        _characterInfoController._infoUI._coinText.text = characterLevelUpCost.ToString(); 
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

        float enhanceProbability = GetProbability(Random.Range(0.01f, 1f));
        
        //내 캐릭터 강화 공식이 필요
        float chance = _characterData.Level.Value * 0.1f;

        chance = Mathf.Clamp(chance, 0.01f, 1f);
         
        if (chance > enhanceProbability)
        {
            Debug.Log($"{gameObject.name} 강화 성공 : 캐릭터 확률 {chance} / 성공 확률 {enhanceProbability}");
        }
        else
        {
            Debug.Log($"강화 실패 : 캐릭터 확률 {chance} / 성공 확률 {enhanceProbability}");
        } 
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
        _characterInfoController._infoUI._enhanceButton.interactable = 
            testMyGold >= characterLevelUpCost && testMyGold != 0;
    }
    
    /// <summary>
    /// 캐릭터 리스트 이름 설정
    /// </summary>
    /// <param name="name"></param>
    public void SetListNameText(string name)
    {
        _characterNameText.text = name;
    }
}