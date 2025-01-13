using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInviteUI : BaseUI
{
    // 변경시킬 이미지
    [SerializeField] Image image;
    [SerializeField] MyRoomCData myRoomCData;
    private int id;
    
    private void Start()
    {
        GetUI<Button>("InviteButton").onClick.AddListener(()=>SetCharacter());
    }

    private void OnEnable()
    {
        id = myRoomCData.id;
        GetUI<Image>("Icon").sprite = myRoomCData.image;
        GetUI<TMP_Text>("NameText").text = myRoomCData.CharacterName;
        
        ActiveCharacter();
    }

    /// <summary>
    /// 보유하고 있는 캐릭터만 활성화 하여 초대 가능
    /// </summary>
    private void ActiveCharacter()
    {
        if (GameManager.UserData.HasCharacter(id))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void SetCharacter()
    {
        image.sprite = myRoomCData.image;
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(GameManager.UserData.Profile.MyroomCharaIdx, id)
            .Submit(result =>
            {
                if (false == result)
                    Debug.Log($"요청 전송에 실패함");
            });
    }
}