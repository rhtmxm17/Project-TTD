using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReqFriendBlock : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    Button btn;

    DatabaseReference rootRef;

    [SerializeField]
    string myUid;
    [SerializeField]
    string myNickname;

    string destUid;

    private void OnDisable()
    {
        btn.onClick.RemoveAllListeners();
    }

    public void InitData(in string nickname, in string uid, bool isReqable)
    {
        text.text = nickname;
        destUid = uid;
        rootRef = GameManager.Database.RootReference;
        btn.interactable = isReqable;

    }




}
