using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class RoomChangePUUI : BaseUI
{
    // TODO: 나중에 스크립터 오브젝트로 만들어서 테마의 이름과 내용을 가져오도록 수정 필요
    [SerializeField] Sprite[] sprites;
    [SerializeField] string[] names;
    [SerializeField] string[] descriptions;
    
    int spriteIndex = 0;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        // 다음으로 넘기는 버튼(우버튼)
        GetUI<Button>("NextButton").onClick.AddListener(()=>NextImage());
        // 이전으로 넘기는 버튼 (좌버튼)
        GetUI<Button>("BeforeButton").onClick.AddListener(()=>BeforeImage());
        
        // 초기 이미지 및 설명 표기용
        GetUI<Image>("RoomPreview").sprite = sprites[spriteIndex];
        GetUI<TMP_Text>("RoomNameText").text = names[spriteIndex];
        GetUI<TMP_Text>("ExplainText").text = descriptions[spriteIndex];
    }
    
    // 우버튼 
    private void NextImage()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }
        GetUI<Image>("RoomPreview").sprite = sprites[spriteIndex];
        GetUI<TMP_Text>("RoomNameText").text = names[spriteIndex];
        GetUI<TMP_Text>("ExplainText").text = descriptions[spriteIndex];
    }
    
    // 좌버튼
    private void BeforeImage()
    {
        spriteIndex--;
        if (spriteIndex < 0)
        {
            spriteIndex = sprites.Length-1;
        }
        GetUI<Image>("RoomPreview").sprite = sprites[spriteIndex];
        GetUI<TMP_Text>("RoomNameText").text = names[spriteIndex];
        GetUI<TMP_Text>("ExplainText").text = descriptions[spriteIndex];
    }
}
