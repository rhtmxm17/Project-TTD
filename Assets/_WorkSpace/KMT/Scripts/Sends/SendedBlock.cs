using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendedBlock : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Button cancelBtn;

    [SerializeField]
    string myUid;

    DatabaseReference dataRef;

    string uid;
    string nickname;

    public void InitData(in string uid, in string nickname)
    {
        this.uid = uid;
        this.nickname = nickname;

        text.text = nickname;

        cancelBtn.onClick.AddListener(Cancel);

        dataRef = GameManager.Database.RootReference.Child("Users");

    }


    void Cancel()
    {
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{myUid}/friends/보낸 친구 요청/{uid}", null },
            { $"{uid}/friends/받은 친구 요청/{myUid}", null}
        };

        dataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("수정실패{친구신청취소}");
                return;
            }

            GetComponentInParent<sendList>().RefreshList();


        });
    }
}
