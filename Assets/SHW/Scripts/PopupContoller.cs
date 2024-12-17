using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PopupContoller : MonoBehaviour
{
  [SerializeField] PlayerInput input;
  [SerializeField] GameObject popup;

  private void Update()
  {
    if(input.actions["Click"].WasPressedThisFrame())
    {
      if (EventSystem.current.currentSelectedGameObject == true) 
        return;
      
      Debug.Log("화면 클릭 & 팝업 종료");
      popup.SetActive(false);
    }
  }
}
