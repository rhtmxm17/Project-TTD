using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomUI : BaseUI
{
    private void Start()
    {
        SetMyRoomUI();
    }

    private void SetMyRoomUI()
    {
        // 뒤로가기 버튼
        GetUI<Button>("MyRoomBackButton").onClick.AddListener(()=>CloseTap("MyRoom"));
        
        // 방뒷배경 바꾸기 팝업 띄우기
        GetUI<Button>("RoomChangeButton").onClick.AddListener(()=>OpenSetRoomPopup("RoomChangePopup"));
        // 배경 바꾸기 팝업 닫기
        GetUI<Button>("CloseChangeRoomPopup").onClick.AddListener(()=>CloseSetRoomPopup("RoomChangePopup"));
        // 배경 이미지 바꾸기 결정
        GetUI<Button>("SetImageButton").onClick.AddListener(()=>ChangeRoom("RoomPreview"));
        
        // 캐릭터 바꾸기 팝업 띄우기
        GetUI<Button>("CharacterChangeButton").onClick.AddListener(()=>OpenSetRoomPopup("CharacterChangePopup"));
        // 캐릭터 바꾸기 팝업 닫기
        GetUI<Button>("CloseChangeCharacter").onClick.AddListener(()=>CloseSetRoomPopup("CharacterChangePopup"));

        // 채팅 열기 버튼
        GetUI<Button>("ChatButton").onClick.AddListener(()=>OpenTap("ChatPopup"));
        // 채팅 닫기
        GetUI<Button>("CloseChat").onClick.AddListener(()=>CloseTap("ChatPopup"));

        GetUI<Button>("VisitButton").onClick.AddListener(
            GetUI<OpenableWindow>("FriendTapCanvas").OpenWindow
            );

    }

    private void OpenTap(string _name)
    {
        GetUI(_name).SetActive(true);
    }

    private void CloseTap(string _name)
    {
        GetUI(_name).SetActive(false);
    }

    //TODO: 선택한 이미지를 DB에 올리는 작업 추가 해야함
    private void OpenSetRoomPopup(string _name)
    {
        GetUI("Safe Area").SetActive(false);
        GetUI(_name).SetActive(true);
    }

    private void CloseSetRoomPopup(string _name)
    {
        GetUI("Safe Area").SetActive(true);
        GetUI(_name).SetActive(false);
    }

    // TODO: 나중에는 이미지가 소유 여부에 따라서 선택 가능 불가능이 바뀌어야 한다.
    private void ChangeRoom(string _name)
    {
        // 바꿀 UI와 그 스프라이트를
        GetUI("BackImage");
        // 선택한 이미지의 스프라이트로 바꾼다
        GetUI<Image>("BackImage").sprite = GetUI<Image>(_name).sprite;
        // 선택한 이미지를 DB에 저장? -> 다음 접속에도 불러오도록 설정??
    }

    private void ChangeCharacter(string _name)
    {
        GetUI("MyRoomCharacter");
        GetUI<Image>("MyRoomCharacter").sprite = GetUI<Image>(_name).sprite;
    }
}