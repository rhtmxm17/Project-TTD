using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterInteract : MonoBehaviour
{
    [SerializeField] private GameObject talkBox;
    // 후에는 SO로 데이터 받아서 하는 방식으로의 전환 필요
    [SerializeField] TMP_Text talkText;                 // 출력할 대화창
     private string[] talkdialogue;     // 대사들 모음
    [SerializeField] private int repeatNum;             // 반복 대사 스텍
    
    [SerializeField] private MyRoomCData[] roomCData;
    private int characterIndex;
    
    private Coroutine talkCoroutine;
    
    // 대화 활성화
    public void ClickCharacter()
    {
        if(talkCoroutine != null) return;
        
        characterIndex = GameManager.UserData.Profile.MyroomCharaIdx.Value -1;
        talkdialogue = roomCData[characterIndex].CharacterDialogue; 
        
        // 주어진 대사에서 랜덤하게 대사 출력
        talkText.text = talkdialogue[Random.Range(0,talkdialogue.Length-1)];
        // 랜덤 대사 스택이 다 쌓이면
        if (repeatNum == 5)
        {
            talkText.text = talkdialogue[talkdialogue.Length-1];
            repeatNum = 0;
        }
        
        talkBox.SetActive(true);
        talkCoroutine = StartCoroutine(OffTalkBoxCO());
    }

    // 대화 비활성화 용 코루틴
    IEnumerator OffTalkBoxCO()
    {
        yield return new WaitForSeconds(1f);
        talkBox.SetActive(false);
        // 랜덤 대사 츨력을 위한 스텍 증가
        repeatNum += 1;
        talkCoroutine = null;
    }
    
}