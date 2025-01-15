using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MyRoomCData")]
public class MyRoomCData : ScriptableObject
{
    // 캐릭터 id
    [SerializeField] public int id;
    // 캐릭터 이름
    [SerializeField] public string CharacterName;
    // 캐릭터 스프라이트
    [SerializeField] public Sprite image;
    // 대사들
    [SerializeField] public string[] CharacterDialogue;
}
