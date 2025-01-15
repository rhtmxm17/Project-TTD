using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterInteract : MonoBehaviour
{
    [SerializeField] private GameObject talkBox;
    [SerializeField] Image talkImage;
    
    [SerializeField] TMP_Text talkText;                 // 출력할 대화창
    private string[] talkdialogue;     // 대사들 모음
    [SerializeField] private int repeatNum;             // 반복 대사 스텍
    
    [SerializeField] private MyRoomCData[] roomCData;
    private int characterIndex;
    
    // 팝업 페이드인/아웃관련
    RectTransform rectTransform;
    
    private Coroutine talkCoroutine;
    
    // 상점에서 사용할 경우
    [SerializeField] private bool isShop;
    
    // 친구방 놀러감
    [SerializeField] public bool isFriendRoom;
    [SerializeField] public int friendRoomIndex;

    // 대화 활성화
    public void ClickCharacter()
    {
        if(talkCoroutine != null) return;
        
        characterIndex = GameManager.UserData.Profile.MyroomCharaIdx.Value -1;
        
        // 친구룸 놀러감
        if (isFriendRoom)
        {
            characterIndex = friendRoomIndex;
        }
        
        talkdialogue = roomCData[characterIndex].CharacterDialogue;

        // 상점에서 사용할 경우
        if (isShop)
        {
            // 주어진 대사에서 랜덤하게 대사 출력
            talkText.text = talkdialogue[Random.Range(0,talkdialogue.Length-1)];
        
            // 5스택 대사
            if (repeatNum == 5)
            {
                talkText.text = talkdialogue[talkdialogue.Length-1];
            }
        }
        
        // 마이룸에서 사용할 경우(기본)
        else
        {
            talkText.text = talkdialogue[Random.Range(0,talkdialogue.Length-4)];
            
            // 5스택 대사
            if (repeatNum == 5)
            {
                talkText.text = talkdialogue[Random.Range(talkdialogue.Length-3,talkdialogue.Length-2)];
            }
            // 10스택 대사 
            if (repeatNum == 10)
            {
                talkText.text = talkdialogue[talkdialogue.Length-1];
                repeatNum = 0;
            }
        }
        
        talkBox.SetActive(true);
        talkCoroutine = StartCoroutine(OffTalkBoxCO());
    }

    // 대화 비활성화 용 코루틴  + 페이드아웃 기능 추가
    IEnumerator OffTalkBoxCO()
    {
        // 페이드 인 기능
        Color fadeColor = talkImage.color;
        Color textColor = talkText.color;
        float time = 0f;
        fadeColor.a = Mathf.Lerp(0f, 1f, time);
        textColor.a = Mathf.Lerp(0f, 1f, time);
        while (fadeColor.a<1f)
        {
            time += Time.deltaTime/1;
            fadeColor.a = Mathf.Lerp(0f, 1f, time);
            textColor.a = Mathf.Lerp(0f, 1f, time);
            talkImage.color = fadeColor;
            talkText.color = textColor;
            yield return null;
        }
        
        yield return new WaitForSeconds(2f);
        
        // 페이드 아웃
        time = 0f;
        fadeColor.a = Mathf.Lerp(1f, 0f, time);
        textColor.a = Mathf.Lerp(1f, 0f, time);
        while (fadeColor.a>0f)
        {
            time += Time.deltaTime/1;
            fadeColor.a = Mathf.Lerp(1f, 0f, time);
            textColor.a = Mathf.Lerp(1f, 0f, time);
            talkImage.color = fadeColor;
            talkText.color = textColor;
            yield return null;
        }
        
        // 대사 비활성화
        talkBox.SetActive(false);
        // 랜덤 대사 츨력을 위한 스텍 증가
        repeatNum += 1;
        talkCoroutine = null;
    }
    
}