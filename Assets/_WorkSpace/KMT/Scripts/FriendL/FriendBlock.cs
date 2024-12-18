using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FriendBlock : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Button deleteFriendBtn;

    [SerializeField]
    string myUid;

    string nickname;
    string uid;

    DatabaseReference userRef;

    private void Start()
    {
        userRef = GameManager.Database.RootReference.Child("Users");
    }

    public void InitData(in string uid, in string nickname)
    { 
        this.uid = uid;    
        this.nickname = nickname;

        text.text = nickname;

        deleteFriendBtn.onClick.AddListener(DeleteFriend);
    }

    private void DeleteFriend()
    {
        //요청사항 제거 + 친구추가.
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{myUid}/friends/friendList/{uid}", null },
            { $"{uid}/friends/friendList/{myUid}", null }
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("수정실패{상호 친구 제거 실패}");
                return;
            }

            GetComponentInParent<FriendList>().RefreshList();


        });
    }


}
