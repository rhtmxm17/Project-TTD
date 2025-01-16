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
    Button visitBtn;
    [SerializeField]
    Button deleteFriendBtn;

    FriendList friendListRoot;
    string nickname;
    string uid;

    DatabaseReference userRef;

    public void InitData(in string uid, in string nickname, FriendList listRoot, Action<string> visitAction)
    {
        userRef = GameManager.Database.RootReference.Child("Users");


        this.uid = uid;    
        this.nickname = nickname;
        friendListRoot = listRoot;

        text.text = nickname;

        visitBtn.onClick.AddListener(() => { VisitFriend(visitAction); });
        deleteFriendBtn.onClick.AddListener(DeleteFriend);
    }

    void VisitFriend(Action<string> visitAction)
    {
        GameManager.OverlayUIManager.OpenDoubleInfoPopup(
            $"{nickname}님의 방으로 \n 놀러깔까요?",
            "아니요", "예",
            null, () => visitAction.Invoke(uid));
    }

    void DeleteFriend()
    {

        GameManager.OverlayUIManager.OpenDoubleInfoPopup(
            $"{nickname}님과 더이상 \n 친구가 아닌가요...?",
            "잘못눌렀어요!", "그렇게 됐네요",
            null, () => 
            {
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { $"{UserData.myUid}/friends/friendList/{uid}", null },
                    { $"{uid}/friends/friendList/{UserData.myUid}", null }
                };

                userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError("수정실패{상호 친구 제거 실패}");
                        return;
                    }

                    GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                        $"{nickname}님과 더이상 \n 친구가 아니게 되었어요...",
                        "다음에 좋은 인연으로 다시 만나요",
                        null
                    );

                    friendListRoot.RefreshList();

                });
            });

        
    }


}
