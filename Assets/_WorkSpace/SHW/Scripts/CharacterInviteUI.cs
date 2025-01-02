using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInviteUI : BaseUI
{
    // 인스펙터에서 직접 넣는 방식 말고 찾을 방법...
    [SerializeField] private Image _image;
    // (임시) id를 가지고 있다고 가정
    [SerializeField] private int id;

    private void Start()
    {
        GetUI<Button>("InviteButton").onClick.AddListener(()=>SetCharacter());
    }

    private void SetCharacter()
    {
        _image.sprite = GetUI<Image>("Icon").sprite;
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(GameManager.UserData.Profile.MyroomCharaIdx, id)
            .Submit(result =>
            {
                if (false == result)
                    Debug.Log($"요청 전송에 실패함");
            });
    }
}
