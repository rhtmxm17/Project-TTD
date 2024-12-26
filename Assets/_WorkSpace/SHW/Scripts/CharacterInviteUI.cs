using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInviteUI : BaseUI
{
    // 인스펙터에서 직접 넣는 방식 말고 찾을 방법...
    [SerializeField] private Image _image;

    private void Start()
    {
        GetUI<Button>("InviteButton").onClick.AddListener(()=>SetCharacter());
    }

    private void SetCharacter()
    {
        _image.sprite = GetUI<Image>("Icon").sprite;
    }
}
