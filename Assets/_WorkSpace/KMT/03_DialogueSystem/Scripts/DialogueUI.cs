using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : BaseUI
{
    [SerializeField]
    string curUid;

    [SerializeField]
    ChattingBlock myBlock;
    [SerializeField]
    ChattingBlock otherBlock;

    TMP_InputField input;
    Button sendBtn;
    Transform boardTransform;

    DatabaseReference curUsersDialogueRef;

    // Start is called before the first frame update
    void Start()
    {
        input = GetUI<TMP_InputField>("ChatInput");
        sendBtn = GetUI<Button>("SendButton");
        boardTransform = GetUI<Transform>("Content");

        curUsersDialogueRef = GameManager.Database.RootReference.Child($"Users/{curUid}/dialogues");

        sendBtn.onClick.AddListener(SendMessage);
        Refresh();
    }

    void SendMessage()
    {

        if (input != null && input.text != "")
        {
            string text = input.text;

            string key = curUsersDialogueRef.Push().Key;

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { key + "/uid", UserData.myUid },
                { key + "/nickname", UserData.myNickname },
                { key + "/content", text },
                { key + "/timestamp",ServerValue.Timestamp }
            };

            curUsersDialogueRef.UpdateChildrenAsync(data);

            var ad = boardTransform.GetComponentsInChildren<ChattingBlock>(true);

            foreach (ChattingBlock block in ad) { 
                Destroy(block.gameObject);
            }

            Refresh();

        }

    }

    [ContextMenu("refresh")]
    //TODO : 갱신{로직 개선 필요}
    void Refresh()
    {

        curUsersDialogueRef.OrderByChild("timestamp").GetValueAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("대화내역 읽어오기 실패");
                return;
            }

            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot content in task.Result.Children)
            {
                ChattingBlock block = null;

                Debug.Log(content.Child("uid").ToString());
                Debug.Log(UserData.myUid);

                if (content.Child("uid").Value.ToString().Equals(UserData.myUid))//본인의 내역인 경우
                {
                    block = Instantiate(myBlock, boardTransform);
                    block.SetBlock(content.Child("content").Value.ToString());
                }
                else //다른사람의 내역인 경우
                { 
                    block = Instantiate(otherBlock, boardTransform);
                    block.SetBlock(content.Child("nickname").Value.ToString(),
                                   content.Child("content").Value.ToString());
                }
            }

            StartCoroutine(AlineCO());

        });

    }

    IEnumerator AlineCO()
    {
        yield return new WaitForSeconds(0.2f);
        var sdf = Instantiate(myBlock, boardTransform);
        sdf.SetBlock(" ");
        yield return null;
        sdf.gameObject.SetActive(false);
    }


}
