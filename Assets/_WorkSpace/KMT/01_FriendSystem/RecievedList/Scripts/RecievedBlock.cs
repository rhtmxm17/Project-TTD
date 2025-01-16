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

    bool isTransactionSuccess;

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

    TransactionResult Transactions(MutableData data)
    {

        if (data.Child(UserData.myUid).HasChild("friends/friendList") &&
            data.Child(UserData.myUid).Child("friends/friendList").ChildrenCount >= FriendList.MAX_FRIEND_CNT)
        {
            GameManager.OverlayUIManager.OpenSimpleInfoPopup($"친구가 너무 많아요! \n 친구좀 줄이고 오시죠", "창닫기", null);
            isTransactionSuccess = false;
            return TransactionResult.Abort();
        }

        if (data.Child(uid).HasChild("friends/friendList") && 
            data.Child(uid).Child("friends/friendList").ChildrenCount >= FriendList.MAX_FRIEND_CNT)
        {
            GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                $"{data.Child(uid).Child("Profile/Name").Value.ToString()}님은 인싸에요! \n 더 받아줄수 없다네요...", "창닫기", null);
            isTransactionSuccess = false;
            return TransactionResult.Abort();
        }

        data.Child($"{UserData.myUid}/friends/recievedRequestList/{uid}").Value = null;
        data.Child($"{UserData.myUid}/friends/friendList/{uid}").Value = "";
        data.Child($"{uid}/friends/sentRequestList/{UserData.myUid}").Value = null;
        data.Child($"{uid}/friends/friendList/{UserData.myUid}").Value = "";

        isTransactionSuccess = true;
        return TransactionResult.Success(data);

    }

    void Accept()
    {
        GameManager.OverlayUIManager.OpenDoubleInfoPopup(
            $"{nickname}님과 친구가 \n 되시는건가요?",
            "아니요", "네! 친구에요!",
            null, () => {

                isTransactionSuccess = false;

                dataRef.RunTransaction(Transactions).ContinueWithOnMainThread(t1 => {

                    if (t1.IsFaulted || t1.IsCanceled)
                    {
                        Debug.Log("트렌젝션 abort 또는 트랜젝션 실패");
                        return;
                    }

                    if (isTransactionSuccess)
                    {
                        GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                        $"{nickname}님과 친구가 되었어요! \n 얏호~!",
                        "와아~! 와아~~!",
                        null
                        );

                        recievedList.RefreshList();
                    }




                });

            }
        );

    }

    void Deny()
    {

        GameManager.OverlayUIManager.OpenDoubleInfoPopup(
            $"{nickname}님의 친구요청을 \n 거절할까요?",
            "실수 실수~~", "거절해주세요",
            null, () =>
            {
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { $"{UserData.myUid}/friends/recievedRequestList/{uid}", null },
                    { $"{uid}/friends/sentRequestList/{UserData.myUid}", null}
                };

                dataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError("수정실패{친구신청거부}");
                        return;
                    }

                    GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                        $"{nickname}님의 \n 친구요청을 거절했어요!",
                        "창닫기",
                        null
                    );

                    recievedList.RefreshList();

                });
            }
        );

    }

}
