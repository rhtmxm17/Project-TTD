using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayerPorifle : MonoBehaviour
{
    [SerializeField]
    string uid;

    [SerializeField]
    string nickname;

    DatabaseReference userNode;
    private void Start()
    {
        userNode = GameManager.Database.RootReference.Child("Users");
    }

    [ContextMenu("Write")]
    public void Write()
    {

        /*Dictionary<string, object> updates = new Dictionary<string, object>
        { 
            {$"{uid}/Profile/nickname", nickname },
            { }
        };

        userNode.Child($"{uid}/Profile/nickname").UpdateChildrenAsync()*/

        userNode.Child($"{uid}/Profile/nickname").SetValueAsync(nickname)
        .ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {

                Debug.Log("<color=red>읽어오기 실패</color>");
            }



        });
    }

}
