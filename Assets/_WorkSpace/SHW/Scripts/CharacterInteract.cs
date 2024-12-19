using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterInteract : MonoBehaviour
{
    [SerializeField] private GameObject talkBox;
    // 당장 안써서 주석처리
    // [SerializeField] TMP_Text talkText;

    // 최소한의 구현...
    // TODO : 추가적인 기능 생기면 그때 리펙토링이랑 같이 작업...하자...
    public void ClickCharacter()
    {
        talkBox.SetActive(true);
        StartCoroutine(OffTalkBoxCO());
    }

    IEnumerator OffTalkBoxCO()
    {
        yield return new WaitForSeconds(5f);
        talkBox.SetActive(false);
    }
}
