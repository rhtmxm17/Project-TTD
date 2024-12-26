using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class RoomChangePUUI : BaseUI
{
    // TODO: 나중에 스크립터 오브젝트로 만들어서 테마의 이름과 내용을 가져오도록 수정 필요
    // 현재는 임시적으로 이미지스프라이트로 
    [SerializeField] Sprite[] sprites;
    
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
        // 이미지 결정 버튼
        // GetUI<Button>("SetImageButton").onClick.AddListener(()=>SetImage("RoomPreview"));
    }
    
    // 우버튼 
    private void NextImage()
    {
        // 능력부족의 내가 시르다..
        spriteIndex++;
        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }
        GetUI<Image>("RoomPreview").sprite = sprites[spriteIndex];
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
    }
    // 설정 버튼
    private void SetImage(string _name)
    {
        // 자식으로만 되던가...
        GetUI<Image>("BackImage").sprite = GetUI<Image>(_name).sprite;
    }
}
