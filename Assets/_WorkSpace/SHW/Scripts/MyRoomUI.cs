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
        
        // 전체 방꾸미기 팝업 열기 버튼
        GetUI<Button>("ChangeRoomButton").onClick.AddListener(()=>OpenTap("ChangeRoom"));
        // 전체 방꾸미기 팝업 닫기 버튼
        GetUI<Button>("CloseChangeRoom").onClick.AddListener(()=>CloseTap("ChangeRoomPanel"));
        // 방뒷배경 바꾸기 팝업 띄우기
        GetUI<Button>("RoomChangeButton").onClick.AddListener(()=>OpenSetRoomPopup("RoomChangePopup"));
        // 배경 바꾸기 팝업 닫기
        GetUI<Button>("CloseChangeRoomPopup").onClick.AddListener(()=>CloseSetRoomPopup("RoomChangePopup"));
        // 방 이미지 바꾸기1
        GetUI<Button>("room1").onClick.AddListener(()=>ChangeRoom("room1"));
        // 방 이미지 바꾸기2
        GetUI<Button>("room2").onClick.AddListener(()=>ChangeRoom("room2"));
        // 캐릭터 바꾸기 팝업 띄우기
        GetUI<Button>("CharacterChangeButton").onClick.AddListener(()=>OpenSetRoomPopup("CharacterChangePopup"));
        // 캐릭터 바꾸기 팝업 닫기
        GetUI<Button>("CloseChangeCharacter").onClick.AddListener(()=>CloseSetRoomPopup("CharacterChangePopup"));
        // 캐릭터 이미지 바꾸기1
        GetUI<Button>("Character1").onClick.AddListener(()=>ChangeCharacter("Character1"));
        // 캐릭터 이미지 바꾸기2
        GetUI<Button>("Character2").onClick.AddListener(()=>ChangeCharacter("Character2"));
        // 캐릭터 이미지 바꾸기3
        GetUI<Button>("Character3").onClick.AddListener(()=>ChangeCharacter("Character3"));

        // 채팅 열기 버튼
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
        GetUI("SafeArea").SetActive(false);
        GetUI("ChangeRoomPanel").SetActive(false);
        GetUI(_name).SetActive(true);
    }

    private void CloseSetRoomPopup(string _name)
    {
        GetUI("SafeArea").SetActive(true);
        GetUI("ChangeRoomPanel").SetActive(true);
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