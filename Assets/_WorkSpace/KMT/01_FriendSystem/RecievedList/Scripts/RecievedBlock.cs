using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;

public class RecievedBlock : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Button acceptBtn;
    [SerializeField]
    Button denyBtn;

    DatabaseReference dataRef;
    RecievedList recievedList;

    string uid;
    string nickname;

    public void InitData(in string uid, in string nickname, RecievedList recievedList)
    { 
        this.uid = uid;
        this.nickname = nickname;
        this.recievedList = recievedList;

        text.text = nickname;

        acceptBtn.onClick.AddListener(Accept);
        denyBtn.onClick.AddListener(Deny);

        dataRef = GameManager.Database.RootReference.Child("Users");
    }

    void Accept()
    {

        //요청사항 제거 + 친구추가.
        //TODO : 디버그용이므로 나중에 myuid로 바꾸는 작업 필요!!!{my,other 스왑 작업}
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{UserData.otherUid}/friends/recievedRequestList/{uid}", null },
            { $"{UserData.otherUid}/friends/friendList/{uid}", "" },
            { $"{uid}/friends/sentRequestList/{UserData.otherUid}", null},
            { $"{uid}/friends/friendList/{UserData.otherUid}", "" }
        };

        dataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("수정실패{상호 친구 등록 실패}");
                return;
            }

            recievedList.RefreshList();


        });
    }

    void Deny()
    {

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{UserData.otherUid}/friends/recievedRequestList/{uid}", null },
            { $"{uid}/friends/sentRequestList/{UserData.otherUid}", null}
        };

        dataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("수정실패{친구신청거부}");
                return;
            }

            recievedList.RefreshList();

        });

    }

}
