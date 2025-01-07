using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomSort : MonoBehaviour
{
    private List<CharacterInviteUI> charactersInvite;
    [SerializeField] Toggle sortToggle;

    private void Start()
    {
        charactersInvite = GetComponentsInChildren<CharacterInviteUI>().ToList();
        
    }
    
    /// <summary>
    /// 갖고 있는 캐릭터들 id순으로 정렬
    /// 토글에 직접 붙여서 사용중
    /// </summary>
    public void Sort()
    {
        if (sortToggle.isOn)
        {
            // 오른차순 정렬
            charactersInvite.Sort();
        }
        else
        {
            // 내림차순 정렬
            charactersInvite.Sort();
            charactersInvite.Reverse();
        }
    }
}
