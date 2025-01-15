using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 팝업에 관련된 기능들
/// </summary>
public class DoubleCheckPopup : BaseUI
{
    public TMP_Text Mesage => popupText; 
  
    [SerializeField] PlayerInput input;
    [SerializeField] GameObject popup;
    [SerializeField]  TMP_Text popupText;


    
    private void Start()
    {
        input = GameManager.Input;
       
    }

    private void OnEnable()
    {
        popupText = gameObject.GetComponentInChildren<TMP_Text>();

        GetUI<Button>("LogOutButton").onClick.AddListener(SignOut);
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
    public void SignOut()
    {
        Debug.Log("LogOut 하였습니다.");
        BackendManager.Auth.SignOut();
        //TODO: DB관련???
    }

}