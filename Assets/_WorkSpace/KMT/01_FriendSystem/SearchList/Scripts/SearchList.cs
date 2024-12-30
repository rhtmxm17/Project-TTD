using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public class SearchList : MonoBehaviour
{

    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    Transform contentTransform;
    [SerializeField]
    SearchBlock searchBlockPrefab;

    DatabaseReference userNode;


    [ContextMenu("find")]
    public void Find()
    {
        userNode = GameManager.Database.RootReference.Child("Users");

        DataSnapshot myData = null;

        //대상이 매우 많을경우 페이지 컷 고려
        userNode.Child(UserData.myUid).GetValueAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("<color=red>읽어오기 실패</color>");
            }

            myData = task.Result;

            userNode
                .OrderByChild("Profile/Name")

                //.EqualTo(findNick)
                .StartAt(inputField.text).LimitToFirst(5)//대체코드

                .GetValueAsync()
                .ContinueWithOnMainThread(task2 =>
                {

                    if (task2.IsFaulted || task2.IsCanceled)
                    {
                        Debug.Log("<color=red>읽어오기 실패</color>");
                    }

                    Debug.Log(task2.Result.ChildrenCount);


                    SearchBlock[] blocks = contentTransform.GetComponentsInChildren<SearchBlock>();

                    foreach (SearchBlock b in blocks)
                    {
                        Destroy(b.gameObject);
                    }


                    foreach (DataSnapshot item in task2.Result.Children)
                    {
                        if (item.Key.Equals(UserData.myUid))
                            continue;

                        Debug.Log(item.Key + "/keys");

                        if (myData.HasChild($"friends/friendList/{item.Key}"))
                        {
                            Debug.Log("중복친구왔습니다. " + item.Key);
                            //이미 있는 친구.
                            continue;
                        }

                        //=============친구 요청 블록 생성 영역.=============//

                        bool reqSendable = true;

                        if (myData.HasChild($"friends/sentRequestList/{item.Key}"))
                        {

                            Debug.Log("중복요청?");
                            //이미 요청 보낸 친구.
                            reqSendable = false;
                        }

                        Debug.Log("여기들어옴");

                        SearchBlock block = Instantiate(searchBlockPrefab, contentTransform.transform);
                        block.InitData(item.Key, item.Child("Profile/Name").Value.ToString(), reqSendable);

                        Debug.Log(item.Child("Profile/Name").Value + "?" + item.Key);
                    }

                    inputField.text = "";

                });

        });



    }

}
