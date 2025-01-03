using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : BaseUI
{
    [Header("현재 방문중인 룸의 uid")]
    [SerializeField]
    string curUid;

    [SerializeField]
    ChattingBlock[] chatingList;

    [SerializeField]
    Sprite[] emogiList;
    [SerializeField]
    Button EmojiButtonPrefab;

    int maxChattingCnt;

    //현재 접속자의 닉네임
    string nick;

    TMP_InputField input;
    Button sendBtn;
    Transform boardTransform;

    OpenableUIBase emojiPanelParent;
    Button emojiButton;
    Button emojiPanelBoarder;
    Transform emojiContentTransform;

    Dictionary<string, Sprite> emogiDict;

    DatabaseReference curUsersDialogueRef;

    protected override void Awake()
    {
        base.Awake();
        input = GetUI<TMP_InputField>("ChatInput");
        sendBtn = GetUI<Button>("SendButton");
        boardTransform = GetUI<Transform>("Content");

        emojiPanelParent = GetUI<OpenableUIBase>("EmojiPanelGroup");
        emojiButton = GetUI<Button>("EmojiButton");
        emojiPanelBoarder = GetUI<Button>("EmojiPanelBoarder");
        emojiContentTransform = GetUI<Transform>("EmojiContents");

        #region 이모티콘 영역

        emojiButton.onClick.AddListener(emojiPanelParent.OpenWindow);
        emojiPanelBoarder.onClick.AddListener(emojiPanelParent.CloseWindow);

        emogiDict = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in emogiList)
        {
            Button btn = Instantiate(EmojiButtonPrefab, emojiContentTransform);
            btn.image.sprite = sprite;
            btn.onClick.AddListener(() => { SendMessage(sprite.name); emojiPanelParent.CloseWindow(); });
            emogiDict.Add(sprite.name, sprite);
        }

        #endregion

        maxChattingCnt = chatingList.Length - 1;

        sendBtn.onClick.AddListener(SendMessage);

        nick = GameManager.UserData.Profile.Name.Value;

    }

    private void OnEnable()
    {
        curUsersDialogueRef = GameManager.Database.RootReference.Child($"Users/{curUid}/dialogues");
        curUsersDialogueRef.ValueChanged += DialogueUpdated;
    }

    private void OnDisable()
    {
        curUsersDialogueRef.ValueChanged -= DialogueUpdated;
    }

    public void SetCurVisitUID(in string uid)
    { 
        curUid = uid;
    }

    private void DialogueUpdated(object sender, ValueChangedEventArgs e)
    {
        Refresh();
    }

    void Start()
    {
        Refresh();
    }


    void SendMessage()
    {

        if (input != null && input.text != "")
        {
            string text = input.text;
            input.text = "";

            string key = curUsersDialogueRef.Push().Key;

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { key + "/uid", UserData.myUid },
                { key + "/nickname", nick },
                { key + "/content", text },
                { key + "/timestamp",ServerValue.Timestamp }
            };

            curUsersDialogueRef.UpdateChildrenAsync(data);

        }

    }

    void SendMessage(in string sendStr)
    {

        string key = curUsersDialogueRef.Push().Key;

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { key + "/uid", UserData.myUid },
            { key + "/nickname", nick },
            { key + "/content", sendStr },
            { key + "/timestamp",ServerValue.Timestamp }
        };

        curUsersDialogueRef.UpdateChildrenAsync(data);

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

                    string stringVal = content.Child("content").Value.ToString();

                    if (content.Child("uid").Value.ToString().Equals(UserData.myUid))//본인의 내역인 경우
                    {

                        if (emogiDict.ContainsKey(stringVal))
                        {
                            chatingList[curChildIdx].SetMyEmojiBlock(emogiDict[stringVal]);
                        }
                        else
                        {
                            chatingList[curChildIdx].SetMyChatBlock(stringVal);
                        }

                    }
                    else //다른사람의 내역인 경우
                    {

                        string nameVal = content.Child("nickname").Value.ToString();

                        if (emogiDict.ContainsKey(stringVal))
                        {
                            chatingList[curChildIdx].SetOtherEmojiBlock(nameVal, emogiDict[stringVal]);
                        }
                        else
                        {
                            chatingList[curChildIdx].SetOtherBlock(nameVal, stringVal);
                        }

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
