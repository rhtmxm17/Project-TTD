using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomData : ScriptableObject
{
    // 방 이미지
    [SerializeField] public Sprite RoomSprite;
    // 방 이름
    [SerializeField] public string RoomName;
    // 방 설명
    [SerializeField] public string RoomDescription;
    // 구매 여부
    [SerializeField] public bool isHas;
    // 가격
    [SerializeField] public int roomPrice;
}
