using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;

public class FriendList : MonoBehaviour
{
    [SerializeField]
    FriendBlock friendBlockPrefab;
    [SerializeField]
    Transform contentTransform;
    [SerializeField]
    OpenableWindow friendPanel;

    [SerializeField]
    MyroomInitializer initializer;

    DatabaseReference userRef;

    private void Awake()
    {
        GetComponent<OpenableWindow>().onOpenAction += RefreshList;
    }

    [ContextMenu("refresh")]
    public void RefreshList()
    {
        userRef = GameManager.Database.RootReference.Child("Users");

        userRef.GetValueAsync().ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("읽어올 수 없음, 친구 목록");
                return;
            }

            FriendBlock[] blocks = contentTransform.GetComponentsInChildren<FriendBlock>();

            foreach (FriendBlock block in blocks)
            {
                Destroy(block.gameObject);
            }

            if (!task.Result.HasChild($"{UserData.myUid}/friends/friendList"))
            {
                Debug.Log("친구 없음");
                return;
            }

            DataSnapshot myFriends = task.Result.Child($"{UserData.myUid}/friends/friendList");

            Debug.Log(myFriends.ChildrenCount + " : 친구수");

            foreach (DataSnapshot friendsUid in myFriends.Children)
            {

                if (!task.Result.HasChild(friendsUid.Key))
                {
                    Debug.Log("해당 친구가 존재하지 않음");
                    continue;
                }

                Debug.Log(task.Result.Child($"{friendsUid.Key}/Profile/Name").Value);

                string nickname = task.Result.Child($"{friendsUid.Key}/Profile/Name").Value.ToString();

                Instantiate(friendBlockPrefab, contentTransform)
                    .InitData(friendsUid.Key,
                              nickname,
                              this,
                              (str) => {

                                  ItemData friendTicket = DataTableManager.Instance.GetItemData(10/*친구방문 보상 수령 카운트*/);
                                  ItemData gold = DataTableManager.Instance.GetItemData(1/*골드*/);

                                  if (!task.Result.Child($"{UserData.myUid}/friends/visitedList").HasChild(friendsUid.Key)
                                     && friendTicket.Number.Value > 0)
                                  {//보상 받을수 있는 방문
                                      GameManager.UserData.StartUpdateStream()
                                        .AddDBValue(friendTicket.Number, -1)
                                        .AddDBValue(gold.Number, 100)
                                        .SetDBValue($"friends/visitedList/{friendsUid.Key}", "")
                                        .Submit((result) => {

                                            if (result)
                                            {
                                                 VisitFriend(str, $"{nickname}님의 방이에요! \n 비싼 물건을 찾아보죠!", 
                                                     () => {
                                                         GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                                                              $"100골드정도는 가져가도 괜찮겠죠! \n (경찰이 오기까지 [{friendTicket.Number.Value}/10])",
                                                              "ㄹㅇㄹㅇ",
                                                              null
                                                        );
                                                     });
                                            }
                                            else
                                            {
                                                Debug.Log("놀러가기 실패.");
                                            }

                                        });
                                  }
                                  else
                                  {
                                      VisitFriend(str, $"{nickname}님의 방이에요! \n 오늘은 이제 구경만 하도록 하죠.", null);
                                  }
                              });

            }

        });

    }

    void VisitFriend(in string uid, in string popupText, Action closeWindowCallback)
    {
        initializer.InitRoom(uid);
        friendPanel.CloseWindow();
        GameManager.OverlayUIManager.OpenSimpleInfoPopup(
              popupText,
              "....?",
              closeWindowCallback
        );
    }

}
