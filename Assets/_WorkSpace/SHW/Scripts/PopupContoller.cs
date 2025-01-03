using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 팝업에 관련된 기능들
/// </summary>
public class PopupContoller : MonoBehaviour
{
  [SerializeField] PlayerInput input;
  [SerializeField] GameObject popup;

  private void Start()
  {
    input = GameManager.Input;
  }

  private void Update()
  {
    // 팝업의 외부를 터치할 경우 화면을 닫는 시스템
    if(input.actions["Click"].WasPressedThisFrame())
    {
      if (EventSystem.current.currentSelectedGameObject == true) 
        return;
      
      Debug.Log("화면 클릭 & 팝업 종료");
      popup.SetActive(false);
    }
  }
}
