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
        GameManager.OverlayUIManager.OpenDoubleInfoPopup(
            $"{text.text}님에게 \n 친구비를 내실건가요?",
            "그정도는 아닌듯", "친구가 되어줘...!",
            null, () => {

                userNode.GetValueAsync().ContinueWithOnMainThread(t1 =>
                {

                    if (t1.IsFaulted || t1.IsCanceled)
                    {
                        Debug.LogError("정보 읽기 실패");
                        return;
                    }

                    if (t1.Result.Child(UserData.myUid).Child("friends/friendList").ChildrenCount >= FriendList.MAX_FRIEND_CNT)
                    {
                        GameManager.OverlayUIManager.OpenSimpleInfoPopup($"친구가 너무 많아요! \n 친구좀 줄이고 오시죠", "앗 아아..", null);
                        return;
                    }

                    if (t1.Result.Child(destUid).Child("friends/friendList").ChildrenCount >= FriendList.MAX_FRIEND_CNT)
                    {
                        GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                            $"{t1.Result.Child(destUid).Child("Profile/Name").Value.ToString()}님은 인싸에요! \n 더 받아줄수 없다네요...", "비겁한 인싸녀석들...", null);
                        return;
                    }

                    //친구요청을 보내는 영역

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

                        GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                            $"{text.text}님에게 친구신청과 \n약간의 [성의]를 보냈습니다.",
                            "헉...!",
                            null
                        );

                        requestBtn.interactable = false;

                    });

                });

            }
        );
    }
}
