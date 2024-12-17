using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    // FIXME : 완성 후 전체적으로 리펙토링 및 수정 필요
    
    // 불러올 데이터가 있는 곳
    [SerializeField] DialogueData dialogueData;
    
    // 불러온 데이터를 적용 시킬 곳
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dialogueText;
    
    private Dialogue[] dialogues;
    
    // 대사의 카운트를 늘릴 변수
    private int nameCounter = 0;
    private int maxNameCounter;
    private int dialogueCounter = 0;
    private int maxDialogueCounter;

    private void Start()
    {
        // 불러올 데이터를 셋팅
        dialogues = dialogueData.dialogues;
        
        maxDialogueCounter = dialogueData.dialogues.Length;
        maxNameCounter = dialogueData.dialogues[nameCounter].contexts.Length;
        
        StoryPrint(nameCounter,dialogueCounter);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(nameCounter<maxNameCounter)
            {
                if(dialogueCounter<maxDialogueCounter)
                {
                    // TODO 
                    // 다이얼로그 카운터 +1 한뒤
                    // 스토리 프린트 함수 실행
                    // 후에 카운터 추가하는 부분 다른 함수로 빼기
                }
            }
        }
    }

    private void StoryPrint(int nameCount, int dialogueCount)
    {
        nameText.text = dialogues[nameCount].name;
        dialogueText.text = dialogues[nameCount].contexts[dialogueCounter];
    }
}
