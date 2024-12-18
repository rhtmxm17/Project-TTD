using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sendList : MonoBehaviour
{
    [SerializeField]
    SendedBlock sendFriendBlockPrefab;

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

        userRef.Child(myUid).Child("friends").Child("보낸 친구 요청").GetValueAsync().ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("읽어올 수 없음, 보낸친구요청 목록");
                return;
            }

            Debug.Log(task.Result.ChildrenCount + " : 보낸 친구 요청 갯수");

            SendedBlock[] blocks = GetComponentsInChildren<SendedBlock>();

            foreach (SendedBlock block in blocks)
            {
                Destroy(block.gameObject);
            }

            userRef.Child(myUid).Child("friends").Child("friendList").GetValueAsync().ContinueWithOnMainThread(task2 => {

                if (task2.IsFaulted || task2.IsCanceled)
                {
                    Debug.Log("기존친구 엄슴");
                    return;
                }

                Debug.Log("현재친구목록");



                foreach (DataSnapshot item in task2.Result.Children)
                {

                    Debug.Log(item.Key + " ?/'? " + item.Value);

                }


                foreach (DataSnapshot item in task.Result.Children)
                {

                    if (task2.Result.HasChild(item.Key))
                    {
                        Debug.Log(item.Key + " 가 이미 있음");
                        continue;
                    }

                    Instantiate(sendFriendBlockPrefab, transform)
                    .InitData(item.Key, item.Value.ToString());

                    Debug.Log(item.Key + " ?/'? " + item.Value);

                }

            });

            

        });


    }
}
