using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChattingBlock : MonoBehaviour
{

    [SerializeField]
    VerticalLayoutGroup chatLayoutGroup;
    [SerializeField]
    VerticalLayoutGroup emojiLayoutGroup;

    [Header("Contents")]
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI contentText;
    [SerializeField]
    Image contentEmogi;

    [Header("Boards")]
    [SerializeField]
    GameObject nameBoard;
    [SerializeField]
    GameObject chatBoard;
    [SerializeField]
    GameObject emojiBoard;

    [Header("Board Imgs")]
    [SerializeField]
    Image chatBoardImg;
    [SerializeField]
    Image emojiBoardImg;

    [Header("BlockColors")]
    [SerializeField]
    Color myBlockColor;
    [SerializeField]
    Color otherBlockColor;


    /// <summary>
    /// 본인이 작성한 채팅 블록을 작성.
    /// </summary>
    /// <param name="content">작성될 내용</param>
    public void SetMyChatBlock(in string content)
    {
        nameBoard.SetActive(false);
        chatBoard.SetActive(true); 
        emojiBoard.SetActive(false);
        contentText.text = content;
        chatBoardImg.color = myBlockColor;
        emojiBoardImg.color = myBlockColor;
        chatLayoutGroup.childAlignment = TextAnchor.UpperRight;
        emojiLayoutGroup.childAlignment = TextAnchor.UpperRight;
    }

    public void SetMyEmojiBlock(Sprite sprite)
    {
        nameBoard.SetActive(false);
        chatBoard.SetActive(false);
        emojiBoard.SetActive(true);
        contentEmogi.sprite = sprite;
        chatBoardImg.color = myBlockColor;
        emojiBoardImg.color = myBlockColor;
        chatLayoutGroup.childAlignment = TextAnchor.UpperRight;
        emojiLayoutGroup.childAlignment = TextAnchor.UpperRight;

    }

    /// <summary>
    /// 다른 사람이 작성한 채팅 블록을 작성
    /// </summary>
    /// <param name="name">작성될 이름</param>
    /// <param name="content">작성될 내용</param>
    public void SetOtherBlock(in string name, in string content)
    {
        nameBoard.SetActive(true);
        chatBoard.SetActive(true);
        emojiBoard.SetActive(false);
        nameText.text = name;
        contentText.text = content;
        chatBoardImg.color = otherBlockColor;
        emojiBoardImg.color = otherBlockColor;
        chatLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        emojiLayoutGroup.childAlignment = TextAnchor.UpperLeft;
    }

    public void SetOtherEmojiBlock(in string name, Sprite sprite)
    {
        nameBoard.SetActive(true);
        chatBoard.SetActive(false);
        emojiBoard.SetActive(true);
        nameText.text = name;
        contentEmogi.sprite = sprite;
        chatBoardImg.color = otherBlockColor;
        emojiBoardImg.color = otherBlockColor;
        chatLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        emojiLayoutGroup.childAlignment = TextAnchor.UpperLeft;
    }

}
