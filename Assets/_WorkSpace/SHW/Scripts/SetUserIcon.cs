using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DB에 유저의 프로필 아이콘인덱스를 넘기는 코드
/// </summary>
public class SetUserIcon : BaseUI
{
    // 현재는 각자가 id를 들고 아이콘이미지 버튼을 누르면 인덱스를 DB에 보냄
    [SerializeField] int iconIndex;
    //프로필에 나타날 이미지 설정
    [SerializeField] private Image icon;
    
    private void Start()
    {
        GetUI<Button>("Character").onClick.AddListener(()=>SetDBIcon());
    }

    public void SetDBIcon()
    {
        icon.sprite = GetUI<Image>("Character").sprite;
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(GameManager.UserData.Profile.IconIndex, iconIndex)
            .Submit(result =>
            {
                if (false == result)
                    Debug.Log($"요청 전송에 실패함");
            });
    }
}
