using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

public class RecievedList : MonoBehaviour
{
    //테스트를 위해서 myuid를 받는사람 uid로 임의로 지정. 추후 수정 필요.
    [SerializeField]
    Transform contentParent;
    [SerializeField]
    RecievedBlock reqFriendBlockPrefab;

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
                Debug.LogError("읽어올 수 없음, 받은친구요청 목록");
                return;
            }

            RecievedBlock[] blocks = contentParent.GetComponentsInChildren<RecievedBlock>();

            foreach (RecievedBlock block in blocks)
            {
                Destroy(block.gameObject);
            }

            if (!task.Result.HasChild($"{UserData.myUid}/friends/recievedRequestList"))
            {
                Debug.Log("받은친구 요청 없음");
                return;
            }

            DataSnapshot requestedList = task.Result.Child($"{UserData.myUid}/friends/recievedRequestList");
            Debug.Log(requestedList.ChildrenCount + " : 친구 요청 갯수");

            
            DataSnapshot myFriends = task.Result.Child($"{UserData.myUid}/friends/friendList");
            Debug.Log("현재친구목록");
            foreach (DataSnapshot item in myFriends.Children)//디버그용 반복 코드
            {
                Debug.Log(item.Key + " : uid");
            }

            Debug.Log("===현재 친구 목록 출력 끝===");


            //받은 친구 요청 출력
            foreach (DataSnapshot rec in requestedList.Children)
            {

                Instantiate(reqFriendBlockPrefab, contentParent)
                .InitData(rec.Key, task.Result.Child($"{rec.Key}/Profile/Name").Value.ToString(), this);

                Debug.Log(rec.Key + "블록 생성");

            }


        });


    }

}
