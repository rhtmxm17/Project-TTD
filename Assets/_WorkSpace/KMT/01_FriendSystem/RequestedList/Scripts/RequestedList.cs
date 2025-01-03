using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestedList : MonoBehaviour
{
    [SerializeField]
    Transform contentTransform;
    [SerializeField]
    RequestedBlock requestedBlockPrefab;

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
                Debug.LogError("읽어올 수 없음, 보낸친구요청 목록");
                return;
            }

            RequestedBlock[] blocks = contentTransform.GetComponentsInChildren<RequestedBlock>();
            foreach (RequestedBlock block in blocks)
            {
                Destroy(block.gameObject);
            }

            if (!task.Result.HasChild($"{UserData.myUid}/friends/sentRequestList"))
            {
                Debug.Log("보냈던 친구요청이 없음");
                return;
            }

            DataSnapshot sentReqLists = task.Result.Child($"{UserData.myUid}/friends/sentRequestList");
            Debug.Log(sentReqLists.ChildrenCount + " : 보낸 친구 요청 갯수");


            DataSnapshot myFriends = task.Result.Child($"{UserData.myUid}/friends/friendList");
            Debug.Log("현재친구목록");
            foreach (DataSnapshot item in myFriends.Children)//디버그용 반복 코드
            {
                Debug.Log(item.Key + " : uid");
            }

            Debug.Log("===현재 친구 목록 출력 끝===");

            foreach (DataSnapshot req in sentReqLists.Children)
            {

                if (myFriends.HasChild(req.Key))
                {
                    Debug.Log(req.Key + " 가 이미 있음");
                    continue;
                }

                Instantiate(requestedBlockPrefab, contentTransform.transform)
                .InitData(req.Key, task.Result.Child($"{req.Key}/Profile/Name").Value.ToString(), this);

                Debug.Log(req.Key);

            }

        });

        /*userRef.Child(UserData.myUid).Child("friends").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            foreach (DataSnapshot item in sentReqList.Children)
            {

                if (task.Result.Child("friendList").HasChild(item.Key))
                {
                    Debug.Log(item.Key + " 가 이미 있음");
                    continue;
                }

                Instantiate(requestedBlockPrefab, contentTransform.transform)
                .InitData(item.Key, item.Value.ToString());

                Debug.Log(item.Key + " ?/'? " + item.Value);

            }

        });*/

        /*userRef.Child(UserData.myUid).Child("friends").Child("sentRequestList").GetValueAsync().ContinueWithOnMainThread(task => {

*//*            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("읽어올 수 없음, 보낸친구요청 목록");
                return;
            }

            Debug.Log(task.Result.ChildrenCount + " : 보낸 친구 요청 갯수");*/

/*            RequestedBlock[] blocks = contentTransform.GetComponentsInChildren<RequestedBlock>();

            foreach (RequestedBlock block in blocks)
            {
                Destroy(block.gameObject);
            }*//*

            userRef.Child(UserData.myUid).Child("friends").Child("friendList").GetValueAsync().ContinueWithOnMainThread(task2 => {

*//*                if (task2.IsFaulted || task2.IsCanceled)
                {
                    Debug.Log("기존친구 엄슴");
                    return;
                }*//*

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

                    Instantiate(requestedBlockPrefab, transform)
                    .InitData(item.Key, item.Value.ToString());

                    Debug.Log(item.Key + " ?/'? " + item.Value);

                }

            });

            

        });*/


    }
}
