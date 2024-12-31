using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomUI : BaseUI
{
    [SerializeField] Sprite[] sprites;
    private int index;
    
    private void Start()
    {
        // FIXME : 여기에 기존 룸 체인지 팝업UI의 스프라이트 들을 가져오고 싶은데...
        GameManager.UserData.TryInitDummyUserAsync(28, () =>
        {
            Debug.Log("완료");
            LoadRoomImage();
        });
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
        
        // 캐릭터 바꾸기 팝업 띄우기
        GetUI<Button>("CharacterChangeButton").onClick.AddListener(()=>OpenSetRoomPopup("CharacterChangePopup"));
        // 캐릭터 바꾸기 팝업 닫기
        GetUI<Button>("CloseChangeCharacter").onClick.AddListener(()=>CloseSetRoomPopup("CharacterChangePopup"));

        // 채팅 열기 버튼
        GetUI<Button>("ChatButton").onClick.AddListener(()=> GetUI<OpenableWindow>("ChatCanvas").OpenWindow());
        // 채팅 닫기
        GetUI<Button>("CloseChat").onClick.AddListener(()=> GetUI<OpenableWindow>("ChatCanvas").CloseWindow());

        GetUI<Button>("VisitButton").onClick.AddListener(
            GetUI<OpenableWindow>("FriendTapCanvas").OpenWindow
        );
    }

    private void LoadRoomImage()
    {
        index = GameManager.UserData.Profile.MyroomBgIdx.Value;
        GetUI<Image>("BackImage").sprite = sprites[index];
    }

    private void OpenTap(string _name)
    {
        GetUI(_name).SetActive(true);
    }

    private void CloseTap(string _name)
    {
        GetUI(_name).SetActive(false);
    }
    
    private void OpenSetRoomPopup(string _name)
    {
        // GetUI("Safe Area").SetActive(false);
        GetUI(_name).SetActive(true);
    }

    private void CloseSetRoomPopup(string _name)
    {
        // GetUI("Safe Area").SetActive(true);
        GetUI(_name).SetActive(false);
    }

    private void ChangeCharacter(string _name)
    {
        GetUI("MyRoomCharacter");
        GetUI<Image>("MyRoomCharacter").sprite = GetUI<Image>(_name).sprite;
    }
}