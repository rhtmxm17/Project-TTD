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
        
        // 초기 이미지 및 설명 표기용
        // TODO: 왠지 초기 이미지의 셋팅에 문제있음 나중에 수정..일단 보류(기능상 문제는 없음)
        GetUI<Image>("RoomPreview").sprite = rooms[spriteIndex].RoomSprite;
        GetUI<TMP_Text>("RoomNameText").text = rooms[spriteIndex].RoomName;
        GetUI<TMP_Text>("ExplainText").text = rooms[spriteIndex].RoomDescription;
        
        
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
        
    }
    
    // 창닫기
    private void CloseTap(string _name)
    {
        GetUI(_name).SetActive(false);
    }
}
