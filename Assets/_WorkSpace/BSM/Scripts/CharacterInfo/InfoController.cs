using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.EventSystems; 

public class InfoController : CharacterInfo, IPointerClickHandler
{

    private int tempLevel;
    private bool isSubscribe;
    
    protected override void Awake()
    {
        base.Awake();
        tempLevel = Random.Range(0, 50);
    }

    public void OnPointerClick(PointerEventData eventData)
    { 
        SetInfoPopup();
        SubscribeLevelUpEvent();
    }

    /// <summary>
    /// 유닛 정보 UI 셋팅
    /// </summary>
    private void SetInfoPopup()
    {
        _characterInfoManager._infoUI._characterInfo = this;
        _characterInfoManager._infoUI.gameObject.SetActive(true); 
        _characterInfoManager._infoUI._nameText.text = _data.Name;
        _characterInfoManager._infoUI._characterImage.sprite = _data.FaceIconSprite;
        _characterInfoManager._infoUI._levelText.text = tempLevel.ToString() + "Lv/200"; 
        _characterInfoManager._infoUI._atkText.text = "공격력" + Random.Range(2, 100).ToString();
        _characterInfoManager._infoUI._hpText.text = "체력" + Random.Range(2, 100).ToString();

    }
 
    /// <summary>
    /// Level Up 버튼 이벤트 등록
    /// </summary>
    private void SubscribeLevelUpEvent()
    {
        if (isSubscribe) return;
        isSubscribe = true; 
        _characterInfoManager._infoUI._levelUpButton.onClick.AddListener(LevelUp);
    }
    
    /// <summary>
    /// 캐릭터 레벨업 기능
    /// </summary>
    private void LevelUp()
    { 
        //오픈한 캐릭터 정보가 구독된 리스트중 자신과 같지 않으면 return
        if (_characterInfoManager._infoUI._characterInfo != this) return;
        
        tempLevel++; 
        _characterInfoManager._infoUI._levelText.text = tempLevel.ToString() + "Lv/200"; 
    }
    
}
