using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestedBlock : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Button cancelBtn;

    RequestedList reqListWindow;
    DatabaseReference dataRef;

    string uid;
    string nickname;

    public void InitData(in string uid, in string nickname, RequestedList reqListWindow)
    {
        this.uid = uid;
        this.nickname = nickname;
        this.reqListWindow = reqListWindow;

        text.text = nickname;

        cancelBtn.onClick.AddListener(Cancel);

        dataRef = GameManager.Database.RootReference.Child("Users");

    }


    void Cancel()
    {
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{UserData.myUid}/friends/sentRequestList/{uid}", null },
            { $"{uid}/friends/recievedRequestList/{UserData.myUid}", null}
        };

        dataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("수정실패{친구신청취소}");
                return;
            }

            GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                $"{nickname}님에게 잘못보내신 \n 친구요청을 취소했어요!",
                "이게 맞지",
                null
            );

            reqListWindow.RefreshList();

        });
    }
}
