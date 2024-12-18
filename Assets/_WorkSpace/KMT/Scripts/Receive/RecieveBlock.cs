using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;

public class RecieveBlock : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Button acceptBtn;
    [SerializeField]
    Button denyBtn;

    [SerializeField]
    string myUid;
    [SerializeField]
    string myNickname;

    DatabaseReference dataRef;

    string uid;
    string nickname;

    public void InitData(in string uid, in string nickname)
    { 
        this.uid = uid;
        this.nickname = nickname;

        text.text = nickname;

        acceptBtn.onClick.AddListener(Accept);
        denyBtn.onClick.AddListener(Deny);

        dataRef = GameManager.Database.RootReference.Child("Users");
    }

    void Accept()
    {

        //요청사항 제거 + 친구추가.
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{myUid}/friends/받은 친구 요청/{uid}", null },
            { $"{myUid}/friends/friendList/{uid}", nickname },
            { $"{uid}/friends/보낸 친구 요청/{myUid}", null},
            { $"{uid}/friends/friendList/{myUid}", myNickname }
        };

        dataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("수정실패{상호 친구 등록 실패}");
                return;
            }

            GetComponentInParent<reqRecievePan>().RefreshList();


        });
    }

    void Deny()
    {

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{myUid}/friends/받은 친구 요청/{uid}", null },
            { $"{uid}/friends/보낸 친구 요청/{myUid}", null}
        };

        dataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("수정실패{친구신청거부}");
                return;
            }

            GetComponentInParent<reqRecievePan>().RefreshList();


        });

    }

}
