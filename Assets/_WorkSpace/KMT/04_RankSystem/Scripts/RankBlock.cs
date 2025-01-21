using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RankBlock : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI counterText;
    [SerializeField]
    Image iconImg;
    [SerializeField]
    TextMeshProUGUI nicknameText;
    [SerializeField]
    TextMeshProUGUI scoreText;

    /// <summary>
    /// 블록의 순위 표기 세팅
    /// </summary>
    /// <param name="counter">순위 숫자</param>
    /// <returns></returns>
    public RankBlock SetCounter(int counter)
    {
        counterText.text = counter.ToString();
        return this;
    }

    /// <summary>
    /// 블록의 정보 세팅
    /// </summary>
    /// <param name="nickname">닉네임</param>
    /// <param name="score">점수</param>
    public void SetBlockInfo(in string nickname, long score)
    {
        nicknameText.text = nickname;
        scoreText.text = string.Format("{0:#,###}", score);
    }

}
