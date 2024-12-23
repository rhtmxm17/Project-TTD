using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterInfo : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool IsSubscribe;
    
    [SerializeField] private CharacterData _characterData;
    private CharacterInfoController _characterInfoController;

    private TextMeshProUGUI _characterName;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _characterName = GetComponentInChildren<TextMeshProUGUI>();
        _characterInfoController = GetComponentInParent<CharacterInfoController>(); 
        
        _characterName.text = _characterData.Name;
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
    /// 캐릭터 강화 기능
    /// </summary>
    private void Enhance()
    {
        if (_characterInfoController.CurCharacterInfo != this) return;

        Debug.Log($"{gameObject.name} 강화 성공");
    }

    public string GetCharacterName()
    {
        return _characterData.Name;
    }
    
}