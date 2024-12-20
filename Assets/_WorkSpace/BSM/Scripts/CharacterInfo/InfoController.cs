using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InfoController : CharacterInfo, IPointerClickHandler
{
 
    public void OnPointerClick(PointerEventData eventData)
    {
        _characterInfoManager._infoUI.gameObject.SetActive(true);
        
        _characterInfoManager._infoUI._nameText.text = _data.Name;
    }
}
