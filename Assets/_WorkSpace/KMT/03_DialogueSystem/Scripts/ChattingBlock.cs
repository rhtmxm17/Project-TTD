using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChattingBlock : MonoBehaviour
{

    [SerializeField]
    VerticalLayoutGroup layoutGroup;

    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI contentText;

    [SerializeField]
    GameObject nameBoard;
    [SerializeField]
    Image boardImg;

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
        contentText.text = content;
        boardImg.color = myBlockColor;
        layoutGroup.childAlignment = TextAnchor.UpperRight;
    }

    /// <summary>
    /// 다른 사람이 작성한 채팅 블록을 작성
    /// </summary>
    /// <param name="name">작성될 이름</param>
    /// <param name="content">작성될 내용</param>
    public void SetOtherBlock(in string name, in string content)
    {
        nameBoard.SetActive(true);
        nameText.text = name;
        contentText.text = content;
        boardImg.color = otherBlockColor;
        layoutGroup.childAlignment = TextAnchor.UpperLeft;
    }

}
