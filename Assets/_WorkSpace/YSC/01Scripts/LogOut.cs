using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;

public class LogOut : MonoBehaviour
{
    [SerializeField] Button logOutButton;

    private void Start()
    {
        logOutButton = gameObject.GetComponent<Button>();
        logOutButton.onClick.AddListener(SignOut);
    }

    // Update is called once per frame
    public void SignOut()
    {
        Debug.Log("LogOut버튼 테스트");
       // BackendManager.Auth.SignOut();
    }

 //   public void OpenDoubleCheck()
 //   {
 //       WarningPopup popupInstance = GameManager.OverlayUIManager.WarningPopup();
 //       WarningPopup.Title.text = "로그아웃 합니다?";
 //   }
    
}
