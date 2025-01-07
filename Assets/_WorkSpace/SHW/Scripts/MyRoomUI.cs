using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomUI : BaseUI
{
    [SerializeField] Sprite[] roomSprites;
    [SerializeField] Sprite[] charaSprites;      // 임시
    private int roomIndex;
    private int charaIndex;
    
    
    private void Start()
    {
        // FIXME : 여기에 기존 룸 체인지 팝업UI의 스프라이트 들을 가져오고 싶은데...
        GameManager.UserData.TryInitDummyUserAsync(28, () =>
        {
            Debug.Log("완료");
            LoadImage();
        });
        SetMyRoomUI();
    }

    private void SetMyRoomUI()
    {
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

    private void LoadImage()
    {
        roomIndex = GameManager.UserData.Profile.MyroomBgIdx.Value;
        GetUI<Image>("BackImage").sprite = roomSprites[roomIndex];
        // 임시적으로 이미 가지고 있는 캐릭터의 id를 받아서 적용 하는 방식
        // 하이어라키 컨텐츠의 임의 캐릭터 이미지로 변경
        charaIndex = GameManager.UserData.Profile.MyroomCharaIdx.Value;
        GetUI<Image>("MyRoomCharacter").sprite = charaSprites[charaIndex];
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

    public void ChangeCharacter(int _index)
    {
        if (_index < 0 || charaSprites.Length <= _index)
        {
            Debug.LogError("마이룸 캐릭터 커스텀 번호가 잘못됨");
            return;
        }
        GetUI<Image>("MyRoomCharacter").sprite = charaSprites[_index];
    }

    public void ChangeBackground(int _index)
    {
        if (_index < 0 || roomSprites.Length <= _index)
        {
            Debug.LogError("마이룸 배경 커스텀 번호가 잘못됨");
            return;
        }
        GetUI<Image>("BackImage").sprite = roomSprites[_index];
    }
}