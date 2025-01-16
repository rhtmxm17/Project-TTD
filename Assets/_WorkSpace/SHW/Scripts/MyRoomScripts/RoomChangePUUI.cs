using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class RoomChangePUUI : BaseUI
{
    [SerializeField] RoomData[] rooms;
    private UserDataInt hasRoom;
    private bool hasRoomBool=false;
    
    // 바꿀 배경 이미지
    [SerializeField] Image backImage;
    
    int spriteIndex = 0;

    private void Start()
    {
        LoadRoomData();
        Init();
    }

    private void OnEnable()
    {
        // 혹시나 UI가 꺼진 상태에서 닫을 경우 대비
        MarkUI();
    }

    private void Init()
    {
        // 다음으로 넘기는 버튼(우버튼)
        GetUI<Button>("NextButton").onClick.AddListener(()=>NextImage());
        // 이전으로 넘기는 버튼 (좌버튼)
        GetUI<Button>("BeforeButton").onClick.AddListener(()=>BeforeImage());
        
        // UI숨기기 버튼
        GetUI<Button>("HideUIButton").onClick.AddListener(()=>HideUI());
        GetUI<Button>("MarkUIButton").onClick.AddListener(()=>MarkUI());
        // 배경 이미지 구매
         GetUI<Button>("Buybutton").onClick.AddListener(()=>BuyRoom());
        
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
        GetRoom(spriteIndex);
        if (hasRoomBool == false)
        {
            GetUI("BuyRoomButton").SetActive(true);
        }
        else
        {
            GetUI("BuyRoomButton").SetActive(false);
        }
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
    public void BuyRoom()
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
        GetRoom(spriteIndex);
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(gold, gold.Value -price)
            .SetDBValue(hasRoom,1)
            .Submit(result =>
            {
                if (false == result)
                {
                    Debug.LogWarning("요청 전송에 실패했습니다");
                    return;
                }

                Debug.Log("방 구매 성공!");
                GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                    $"성공적으로 방이 구매되었습니다.",
                    "창닫기",
                    null
                );
            });
        
        GetUI("BuyRoomButton").SetActive(false);
        GetUI("ConfirmPopup").SetActive(false);
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
        GetUI<TMP_Text>("BuyButtonText").text = rooms[spriteIndex].roomPrice.ToString();
        
        GetRoom(spriteIndex);
        // 방 소유하지 않았으면 구매버튼 활성화
        if (hasRoomBool == false)
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
        GetUI<TMP_Text>("BuyButtonText").text = rooms[spriteIndex].roomPrice.ToString();
        
        GetRoom(spriteIndex);
        // 방 소유하지 않았으면 구매버튼 활성화
        if (hasRoomBool == false)
        {
            GetUI("BuyRoomButton").SetActive(true);
        }
        else
        {
            GetUI("BuyRoomButton").SetActive(false);
        }
    }
    
    // DB 정보 불러오기
    private void GetRoom(int i)
    {
        hasRoom = GameManager.UserData.PlayData.HasRoom[i];
        hasRoomBool = hasRoom.Value > 0;
    }
    
    // UI숨기기 버튼
    private void HideUI()
    {
        GetUI("Title").SetActive(false);
        GetUI("NextButton").SetActive(false);
        GetUI("BeforeButton").SetActive(false);
        GetUI("ExplainPopup").SetActive(false);
        GetUI("HideUIButton").SetActive(false);
        GetUI("MarkUIButton").SetActive(true);
    }

    // UI 다시 나타내기 버튼
    private void MarkUI()
    {
        GetUI("Title").SetActive(true);
        GetUI("NextButton").SetActive(true);
        GetUI("BeforeButton").SetActive(true);
        GetUI("ExplainPopup").SetActive(true);
        GetUI("HideUIButton").SetActive(true);
        GetUI("MarkUIButton").SetActive(false);
    }
    
    // 창닫기
    private void CloseTap(string _name)
    {
        GetUI(_name).SetActive(false);
    }
}
