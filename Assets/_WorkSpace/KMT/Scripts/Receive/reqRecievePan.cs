using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

public class reqRecievePan : MonoBehaviour
{
    [SerializeField]
    RecieveBlock reqFriendBlockPrefab;

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

        userRef.Child(myUid).Child("friends").Child("받은 친구 요청").GetValueAsync().ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("읽어올 수 없음, 받은친구요청 목록");
                return;
            }

            Debug.Log(task.Result.ChildrenCount + " : 친구 요청 갯수");

            RecieveBlock[] blocks = GetComponentsInChildren<RecieveBlock>();

            foreach (RecieveBlock block in blocks)
            {
                Destroy(block.gameObject);
            }

            foreach (DataSnapshot item in task.Result.Children) {

                Instantiate(reqFriendBlockPrefab, transform)
                .InitData(item.Key, item.Value.ToString());

                Debug.Log(item.Key + " ?/'? " + item.Value);

            }
        
        });


    }

}
