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
    // 알림창팝업
    [SerializeField] DoubleCheckPopup warningPopup;

    private void Start()
    {
        logOutButton = gameObject.GetComponent<Button>();
        logOutButton.onClick.AddListener(OpenDoubleCheck);
        logOutButton.onClick.AddListener(SignOut);
    }

    // Update is called once per frame
    public void SignOut()
    {
        Debug.Log("LogOut버튼 테스트");
        // BackendManager.Auth.SignOut();
    }
    
    // TODO: 로그아웃경고 팝업 띄우기

    public void OpenDoubleCheck()
    {
        DoubleCheckPopup popupInstance = Instantiate(warningPopup, transform.parent.transform);
        popupInstance.Mesage.text = "로그아웃 하시겠습니까?";
    }
    
}
