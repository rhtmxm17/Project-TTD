using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class RoomChangePUUI : BaseUI
{
    [SerializeField] RoomData[] rooms;  
    
    // 바꿀 배경 이미지
    [SerializeField] Image backImage;
    
    int spriteIndex = 0;
    // 방의 소유 여부
    // TODO: DB로 보내는 작업 해야함..ㅠㅠㅠ 
    bool hasRoom = false;

    private void Start()
    {
        Init();
        LoadRoomData();
    }

    private void Init()
    {
        // 다음으로 넘기는 버튼(우버튼)
        GetUI<Button>("NextButton").onClick.AddListener(()=>NextImage());
        // 이전으로 넘기는 버튼 (좌버튼)
        GetUI<Button>("BeforeButton").onClick.AddListener(()=>BeforeImage());
        
        GetUI("BuyRoomButton").SetActive(false);    // 초기 1번 방은 무조건 열어둘거라 false로 설정
        // UI숨기기 버튼
        GetUI<Button>("HideUIButton").onClick.AddListener(()=>HideUI());
        GetUI<Button>("MarkUIButton").onClick.AddListener(()=>MarkUI());
        // 배경 이미지 구매
        GetUI<Button>("BuyRoomButton").onClick.AddListener(()=>BuyRoom());
        
        // 배경 이미지 바꾸기 결정
        GetUI<Button>("SetImageButton").onClick.AddListener(()=>
        {
            ChangeRoom("RoomPreview");
            CloseTap("RoomChangePopup");
        });
    }
    
    private void LoadRoomData()
    {
        spriteIndex = GameManager.UserData.Profile.MyroomBgIdx.Value;
        GetUI<Image>("RoomPreview").sprite = rooms[spriteIndex].RoomSprite;
        GetUI<TMP_Text>("RoomNameText").text = rooms[spriteIndex].RoomName;
        GetUI<TMP_Text>("ExplainText").text = rooms[spriteIndex].RoomDescription;
        hasRoom = rooms[spriteIndex].isHas;
    }
    
    // 방 이미지 변경
    private void ChangeRoom(string _name)
    {
        // 선택한 이미지의 스프라이트로 바꾼다
        backImage.sprite = GetUI<Image>(_name).sprite;
        // 이미지 넘버 DB에 보냄
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(GameManager.UserData.Profile.MyroomBgIdx, spriteIndex)
            .Submit(result =>
            {
                if (false == result)
                    Debug.Log($"요청 전송에 실패함");
            });
    }
    
    // 방 구매
    private void BuyRoom()
    {
        // 소유 골드 가져오기
        UserDataInt gold = GameManager.TableData.GetItemData(1).Number;
        int price = rooms[spriteIndex].roomPrice;
        
        if ( gold.Value < price)
        {
            Debug.LogWarning("골드부족!");
            return;
        }
        
        // 구매
        // TODO: DB생기면 바꿀 곳
        rooms[spriteIndex].isHas = true;
        hasRoom = rooms[spriteIndex].isHas;
        // TODO: 구매한 방의 bool 값 DB에 저장
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(gold, gold.Value -price)
            .Submit(result =>
            {
                if (false == result)
                {
                    Debug.LogWarning("요청 전송에 실패했습니다");
                    return;
                }

                Debug.Log("방 구매 성공!");
            });
        
        GetUI("BuyRoomButton").SetActive(false);
    }
    
    // 우버튼 
    private void NextImage()
    {
        spriteIndex++;
        if (spriteIndex >= rooms.Length)
        {
            spriteIndex = 0;
        }
        GetUI<Image>("RoomPreview").sprite = rooms[spriteIndex].RoomSprite;
        GetUI<TMP_Text>("RoomNameText").text = rooms[spriteIndex].RoomName;
        GetUI<TMP_Text>("ExplainText").text = rooms[spriteIndex].RoomDescription;
        // TODO: DB생기면 바꿀 곳
        hasRoom = rooms[spriteIndex].isHas;
        // 방 소유하지 않았으면 구매버튼 활성화
        if (hasRoom == false)
        {
            GetUI("BuyRoomButton").SetActive(true);
        }
        else
        {
            GetUI("BuyRoomButton").SetActive(false);
        }
    }
    
    // 좌버튼
    private void BeforeImage()
    {
        spriteIndex--;
        if (spriteIndex < 0)
        {
            spriteIndex = rooms.Length-1;
        }
        GetUI<Image>("RoomPreview").sprite = rooms[spriteIndex].RoomSprite;
        GetUI<TMP_Text>("RoomNameText").text = rooms[spriteIndex].RoomName;
        GetUI<TMP_Text>("ExplainText").text = rooms[spriteIndex].RoomDescription;
        // TODO: DB생기면 바꿀 곳
        hasRoom = rooms[spriteIndex].isHas;
        // 방 소유하지 않았으면 구매버튼 활성화
        if (hasRoom == false)
        {
            GetUI("BuyRoomButton").SetActive(true);
        }
        else
        {
            GetUI("BuyRoomButton").SetActive(false);
        }
    }
    
    // UI숨기기 버튼
    private void HideUI()
    {
        GetUI("CloseChangeRoomPopup").SetActive(false);
        GetUI("Title").SetActive(false);
        GetUI("NextButton").SetActive(false);
        GetUI("BeforeButton").SetActive(false);
        GetUI("ExplainPopup").SetActive(false);
        GetUI("HomeButton").SetActive(false);
        GetUI("MenuButton").SetActive(false);
        GetUI("HideUIButton").SetActive(false);
        GetUI("MarkUIButton").SetActive(true);
    }

    // UI 다시 나타내기 버튼
    private void MarkUI()
    {
        GetUI("CloseChangeRoomPopup").SetActive(true);
        GetUI("Title").SetActive(true);
        GetUI("NextButton").SetActive(true);
        GetUI("BeforeButton").SetActive(true);
        GetUI("ExplainPopup").SetActive(true);
        GetUI("HomeButton").SetActive(true);
        GetUI("MenuButton").SetActive(true);
        GetUI("HideUIButton").SetActive(true);
        GetUI("MarkUIButton").SetActive(false);
    }
    
    // 창닫기
    private void CloseTap(string _name)
    {
        GetUI(_name).SetActive(false);
    }
}
