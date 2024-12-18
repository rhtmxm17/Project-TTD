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
    string myUid;

    DatabaseReference userRef;

    private void Start()
    {
        //userRef = GameManager.Database.RootReference.Child("Users");
    }

    [ContextMenu("refresh")]
    public void RefreshList()
    {
        userRef = GameManager.Database.RootReference.Child("Users");

        userRef.Child(myUid).Child("friends").Child("friendList").GetValueAsync().ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("읽어올 수 없음, 보낸친구요청 목록");
                return;
            }

            Debug.Log(task.Result.ChildrenCount + " : 친구수");

            FriendBlock[] blocks = GetComponentsInChildren<FriendBlock>();

            foreach (FriendBlock block in blocks)
            {
                Destroy(block.gameObject);
            }
             
            foreach (DataSnapshot item in task.Result.Children)
            {

                Debug.LogWarning(item.Key + " // " + item.Value.ToString());
                
                Instantiate(friendBlockPrefab, transform)
                    .InitData(item.Key, item.Value.ToString());

                Debug.Log(item.Key + " ?/'? " + item.Value);

            }


        });


    }

}
