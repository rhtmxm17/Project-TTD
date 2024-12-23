using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterInfo : MonoBehaviour, IPointerClickHandler
{
    public CharacterData _CharacterData;
    private CharacterInfoController _characterInfoController;
    
    public bool IsSubscribe;
    
    public int tempLevel;


    private void Awake()
    {
        tempLevel = Random.Range(0, 50); 
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _characterInfoController = GetComponentInParent<CharacterInfoController>(); 
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
    }

    private void SetInfoPopup()
    {
        _characterInfoController.CurCharacterInfo = this;
        _characterInfoController.CurIndex = _characterInfoController._characterInfos.IndexOf(this);
        _characterInfoController._infoPopup.SetActive(true);
        
        //TODO: 정리 필요
        _characterInfoController._infoUI._nameText.text = _CharacterData.Name;
        _characterInfoController._infoUI._characterImage.sprite = _CharacterData.FaceIconSprite;
        _characterInfoController._infoUI._levelText.text = tempLevel.ToString() + "Lv/200";
        _characterInfoController._infoUI._atkText.text = "공격력" + Random.Range(2, 100).ToString();
        _characterInfoController._infoUI._hpText.text = "체력" + Random.Range(2, 100).ToString();
    }

    private void LevelUp()
    {
        //오픈한 캐릭터 정보가 구독된 리스트중 자신과 같지 않으면 return
        if (_characterInfoController.CurCharacterInfo != this) return;

        tempLevel++;
        LevelUpStats();
    }


    private void LevelUpStats()
    {
        //TODO: 레벨업에 따른 각종 수치 증가 필요
        _characterInfoController._infoUI._levelText.text = tempLevel.ToString() + "Lv/200";
    }


}