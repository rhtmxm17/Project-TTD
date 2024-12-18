using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;

public class FindFriendScript : MonoBehaviour
{
    [SerializeField]
    string findNick;

    [SerializeField]
    FindReqBlock findReqBlockPrefab;

    DatabaseReference userNode;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        GameManager.Database.SetPersistenceEnabled(false);
        userNode = GameManager.Database.RootReference.Child("Users");
    }


    [ContextMenu("find")]
    public void Find()
    {
        DataSnapshot myData = null;


        userNode
                .OrderByChild("Profile/nickname")
                //.EqualTo(findNick)
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

                });

        userNode
                .OrderByChild("Profile/nickname")
                .StartAt(findNick)
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

                });

        userNode
                .OrderByChild("Profile/nickname")
                .StartAt(findNick)
                .EndAt(findNick)
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

                });

        /* //대상이 매우 많을경우 페이지 컷 고려
         userNode.Child(UserData.myUid).GetValueAsync().ContinueWithOnMainThread(task =>
         {

             if (task.IsFaulted || task.IsCanceled)
             {
                 Debug.Log("<color=red>읽어오기 실패</color>");
             }

             myData = task.Result;

             userNode
                 .OrderByChild("Profile/nickname")
                 .EqualTo("cjs")
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


                     FindReqBlock[] blocks = GetComponentsInChildren<FindReqBlock>();

                     foreach (FindReqBlock b in blocks)
                     {
                         Destroy(b.gameObject);
                     }


                     foreach (DataSnapshot item in task.Result.Children)
                     {
                         if (item.Key.Equals(UserData.myUid))
                             continue;

                         if (myData.HasChild("friendList") && myData.Child("friendList").HasChild(item.Key))
                         {
                             //이미 있는 친구.
                             continue;
                         }

                         //=============친구 요청 블록 생성 영역.=============//

                         bool reqSendable = true;

                         if (myData.HasChild("보낸 친구 요청") && myData.Child("보낸 친구 요청").HasChild(item.Key))
                         {
                             //이미 요청 보낸 친구.
                             reqSendable = false;
                         }

                         Debug.Log("여기들어옴");

                         FindReqBlock block = Instantiate(findReqBlockPrefab, transform);
                         block.InitData(item.Key, item.Child("Profile/nickname").Value.ToString(), reqSendable);

                         Debug.Log(item.Child("Profile/nickname").Value + "?" + item.Key);
                     }

                 });

         });*/



    }

}
