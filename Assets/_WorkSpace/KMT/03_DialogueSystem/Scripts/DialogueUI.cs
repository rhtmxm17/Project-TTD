using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DialogueUI : BaseUI
{
    [SerializeField]
    string curUid;

    [SerializeField]
    ChattingBlock[] chatingList;

    int maxChattingCnt;

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

        curUsersDialogueRef = GameManager.Database.RootReference.Child($"Users/{UserData.myUid}/dialogues");

        maxChattingCnt = chatingList.Length - 1;

        sendBtn.onClick.AddListener(SendMessage);
        Refresh();
    }

    [SerializeField]
    string uid;
    [SerializeField]
    string nick;

    void SendMessage()
    {

        if (input != null && input.text != "")
        {
            string text = input.text;
            input.text = "";

            string key = curUsersDialogueRef.Push().Key;

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { key + "/uid", uid },
                { key + "/nickname", nick },
                { key + "/content", text },
                { key + "/timestamp",ServerValue.Timestamp }
            };

            curUsersDialogueRef.UpdateChildrenAsync(data);

            Refresh();

        }

    }

    [ContextMenu("refresh")]
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
            Dictionary<string, object> updateDataDic = new Dictionary<string, object>();

            long curChildIdx = snapshot.ChildrenCount - 1;


            foreach (ChattingBlock blocks in chatingList)
            { 
                blocks.gameObject.SetActive(false);
            }

            foreach (DataSnapshot content in task.Result.Children)
            {
                if (curChildIdx < maxChattingCnt && curChildIdx >= 0)//채팅블록 재설정
                {
                    if (content.Child("uid").Value.ToString().Equals(curUid))//본인의 내역인 경우
                    {
                        chatingList[curChildIdx].SetMyChatBlock(content.Child("content").Value.ToString());
                    }
                    else //다른사람의 내역인 경우
                    {
                        chatingList[curChildIdx].SetOtherBlock(content.Child("nickname").Value.ToString(),
                                       content.Child("content").Value.ToString());
                    }

                    chatingList[curChildIdx].gameObject.SetActive(true);
                }
                else //데이터베이스 제거 리스트에 추가
                {
                    updateDataDic.Add(content.Key, null);
                }

                curChildIdx--;


            }

            if (updateDataDic.Count > 0)
            {
                curUsersDialogueRef.UpdateChildrenAsync(updateDataDic);
            }

            StartCoroutine(AlineCO());

        });

    }

    IEnumerator AlineCO()
    {
        yield return new WaitForSeconds(0.2f);
        chatingList[maxChattingCnt].gameObject.SetActive(true);
        chatingList[maxChattingCnt].SetMyChatBlock(" /n/n/n");
        yield return new WaitForSeconds(0.2f);
        chatingList[maxChattingCnt].gameObject.SetActive(false);

    }


}
