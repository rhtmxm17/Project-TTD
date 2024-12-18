using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Writer : MonoBehaviour
{
    [SerializeField]
    string findNick;

    [SerializeField]
    Transform scrollview;

    [SerializeField]
    ReqFriendBlock friendReqBlockPrefab;

    DatabaseReference userNode;


    private void Start()
    {
        //GameManager.Database.SetPersistenceEnabled(false);
        //userNode = GameManager.Database.RootReference.Child("Users");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Find();

        }
    }

    [ContextMenu("readTest")]
    public void sendFirend()
    {
        userNode = GameManager.Database.RootReference.Child("Users");

        userNode
            .OrderByChild("Profile/nickname")
            //.EqualTo(findNick)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {

                if (task.IsFaulted || task.IsCanceled)
                {

                    Debug.Log("<color=red>읽어오기 실패</color>");
                }

                Debug.Log(task.Result.ChildrenCount);

                foreach (DataSnapshot item in task.Result.Children)
                {
                    Debug.Log(item.Key + " / " + item.Child("Profile/nickname").Value);

                }

            });

    }
    [ContextMenu("find")]
    public void Find()
    {
        userNode = GameManager.Database.RootReference.Child("Users");

        DataSnapshot myData = null;

        userNode
            .OrderByChild("Profile/nickname")
            .EqualTo(findNick)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {

                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("<color=red>읽어오기 실패</color>");
                }

                Debug.Log(task.Result.ChildrenCount + "/////1");

                foreach (DataSnapshot item in task.Result.Children)
                {
                    Debug.Log(item.Child("Profile/nickname").Value + "?" + item.Key);
                }

            });

        //대상이 매우 많을경우 페이지 컷 고려
        userNode.Child(myUid).GetValueAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("<color=red>읽어오기 실패</color>");
            }

            myData = task.Result;

            userNode
                .OrderByChild("Profile/nickname")
                .EqualTo(findNick)
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {

                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log("<color=red>읽어오기 실패</color>");
                    }

                    Debug.Log(task.Result.ChildrenCount);

                    foreach (DataSnapshot item in task.Result.Children)
                    {
                        Debug.Log(item.Child("Profile/nickname").Value + "?" + item.Key);
                    }

                    foreach (DataSnapshot item in task.Result.Children)
                    {
                        if (item.Key.Equals(myUid))
                            continue;

                        if (myData.HasChild("friendList") && myData.Child("friendList").HasChild(item.Key))
                        {
                            //이미 있는 친구.
                            continue;
                        } 

                        //친구 요청 블록 생성 영역.//

                        bool reqSendable = true;

                        if (myData.HasChild("보낸 친구 요청") && myData.Child("보낸 친구 요청").HasChild(item.Key))
                        {
                            //이미 요청 보낸 친구.
                            reqSendable = false;
                        }
                        
                        Debug.Log("여기들어옴");

                        ReqFriendBlock block = Instantiate(friendReqBlockPrefab, transform);
                        block.InitData(item.Child("Profile/nickname").Value.ToString(), item.Key, reqSendable);

                        Debug.Log(item.Child("Profile/nickname").Value + "?" + item.Key);
                    }

                });

        });


    }

    [ContextMenu("Write")]
    public void Write()
    {
        userNode = GameManager.Database.RootReference.Child("Users");

        userNode.Child("Dummy0/Profile/nickname").SetValueAsync(findNick)
        .ContinueWithOnMainThread(task =>
        {

            if (task.IsFaulted || task.IsCanceled)
            {

                Debug.Log("<color=red>읽어오기 실패</color>");


            }



        });
    }


    [SerializeField]
    string myUid;
    [SerializeField]
    string myNickname;
    [SerializeField]
    [Description("nickname?uid")]
    string destfriendCode;

    [ContextMenu("sendreq")]
    public void SendReq()
    {
        userNode = GameManager.Database.RootReference.Child("Users");

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { $"{UserData.myUid}/friends/보낸 친구 요청/{UserData.otherUid}", " " },
            { $"{UserData.otherUid}/friends/받은 친구 요청/{UserData.myUid}", " "}
        };

        userNode.UpdateChildrenAsync(updates)
        .ContinueWithOnMainThread(task =>
        {

            if (task.IsFaulted || task.IsCanceled)
            {

                Debug.Log("<color=red>요청 전달하기.</color>");

            }

            Debug.Log("요청 성공");

        });

    }

}
