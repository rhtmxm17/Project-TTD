using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoPopup : MonoBehaviour
{
    private CharacterInfoController _characterInfoController;
    
    private void Awake()
    {
        _characterInfoController = GetComponentInParent<CharacterInfoController>();
    }

    private void OnDisable()
    {
        InfoPopupClose();
    }

    /// <summary>
    /// 상세 팝업 종료 후 탭 타입 변경
    /// </summary>
    private void InfoPopupClose()
    {
        gameObject.SetActive(false);
        _characterInfoController.StartListSort();
        _characterInfoController._infoUI._detailTabColor.color = Color.cyan;
        _characterInfoController._infoUI._enhanceTabColor.color = Color.white;
        _characterInfoController._infoUI._evolutionTabColor.color = Color.white;
        _characterInfoController.CurInfoTabType = InfoTabType.DETAIL;
        _characterInfoController.CurCharacterInfo.CharacterModel.gameObject.SetActive(false);
        
    }
}
