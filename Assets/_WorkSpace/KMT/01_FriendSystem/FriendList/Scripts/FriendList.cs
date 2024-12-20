using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendList : MonoBehaviour
{
    [SerializeField]
    FriendBlock friendBlockPrefab;
    [SerializeField]
    Transform contentTransform;

    DatabaseReference userRef;


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

                Debug.Log(task.Result.Child($"{friendsUid.Key}/Profile/nickname").Value);

                Instantiate(friendBlockPrefab, contentTransform)
                    .InitData(friendsUid.Key, 
                              task.Result.Child($"{friendsUid.Key}/Profile/nickname").Value.ToString(),
                              this);

            }

        });

    }

}
