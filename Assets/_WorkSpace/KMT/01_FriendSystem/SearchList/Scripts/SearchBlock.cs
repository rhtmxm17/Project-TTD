using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchBlock : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Button requestBtn;

    DatabaseReference userNode;

    string destUid;

    public void InitData(in string uid, in string nickname, bool sendable)
    {

        text.text = nickname;
        destUid = uid;

        requestBtn.onClick.AddListener(Request);
        requestBtn.interactable = sendable;

        userNode = GameManager.Database.RootReference.Child("Users");
    }

    [ContextMenu("sendreq")]
    public void Request()
    {

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{UserData.myUid}/friends/sentRequestList/{destUid}", " " },
            { $"{destUid}/friends/recievedRequestList/{UserData.myUid}", " "}
        };

        userNode.UpdateChildrenAsync(updates)
        .ContinueWithOnMainThread(task =>
        {

            if (task.IsFaulted || task.IsCanceled)
            {

                Debug.Log("<color=red>요청 전달하기.</color>");

            }

            Debug.Log("요청 성공");

            requestBtn.interactable = false;

        });

    }
}
