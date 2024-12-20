using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChattingBlock : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI contentText;

    /// <summary>
    /// 이름은 설정하지 않고 내용만 작성
    /// </summary>
    /// <param name="content">작성될 내용</param>
    public void SetBlock(in string content)
    {
        contentText.text = content;
    }

    /// <summary>
    /// 이름과 내용 모두 작성
    /// </summary>
    /// <param name="name">작성될 이름</param>
    /// <param name="content">작성될 내용</param>
    public void SetBlock(in string name, in string content)
    {
        if (nameText != null)
        {
            nameText.text = name;
        }
        contentText.text = content;
    }

}
