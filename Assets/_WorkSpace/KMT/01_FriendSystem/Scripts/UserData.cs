using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserData
{
    public static string myUid => BackendManager.CurrentUserDataRef.Key;
    public static string myNickname => GameManager.UserData.Profile.Name.Value;

}
