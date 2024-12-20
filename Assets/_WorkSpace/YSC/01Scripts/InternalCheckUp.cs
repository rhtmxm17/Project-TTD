using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalCheckUp : UI_Manager
{
    public string GetUserID()
    {
        // 기기안에 저장 / 로컬기기
        if(string.IsNullOrEmpty(UserID))
        {
            UserID = PlayerPrefs.GetString("Firebase_UID", string.Empty);
        }
        // 기기내부 저장된 값과 데이터베이스/Auth 의 값을 비교


        return UserID;
    }
    private void SaveLocalUID(string uid)
    {
        PlayerPrefs.SetString("Firebase_UID", uid);
        PlayerPrefs.Save();
    }
    private void DeleteLocalUID()
    {
        PlayerPrefs.DeleteKey("Firebase_UID");
        PlayerPrefs.Save();
        UserID = string.Empty;
    }

    public void Logout()
    {
        BackendManager.Auth.SignOut();
    }



}
