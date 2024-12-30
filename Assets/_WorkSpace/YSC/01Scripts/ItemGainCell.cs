using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGainCell : MonoBehaviour
{
    [SerializeField] public Sprite spriteImage;
    int itemID;
    string itemName;
    string itemDescription;

    int itemCount;
    public string NumberText;

    [SerializeField] ItemData itemData;

    [Header("아이템 이름")]
    [SerializeField] TMP_Text itemNameText;
    [Header("아이템 갯수")]
    [SerializeField] TMP_Text itemCountText;
    [Header("아이템 이미지")]
    [SerializeField] public Image itemImage;

    public void SetItem(ItemData item)
    {
        itemID = item.Id;
        itemName = item.ItemName;
        itemCount = 5; // 생기면 거기에 맞게 변경
        spriteImage = item.SspriteImage;
        itemDescription = item.Description;
    }

    public void OnEnable()
    {
        SetItem(itemData);
        itemNameText.text = itemName;
        itemCountText.text = NumberText;
        itemImage.sprite = spriteImage;
    }
}
