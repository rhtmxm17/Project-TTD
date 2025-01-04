using Firebase.Database;
using Firebase.Extensions;
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
                                  initializer.InitRoom(str);
                                  friendPanel.CloseWindow();
                                  GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                                        $"{nickname}님의 방이에요! \n 비싼 물건을 찾아보죠!",
                                        "....?",
                                        null
                                    );
                              });

            }

        });

    }

}
