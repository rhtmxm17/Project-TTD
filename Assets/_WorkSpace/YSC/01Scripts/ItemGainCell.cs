using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGainCell : MonoBehaviour
{
    [System.Serializable]
    private struct InitField
    {
        [Header("아이템 이름")]
        public TMP_Text itemNameText;
        [Header("아이템 갯수")]
        public TMP_Text itemCountText;
        [Header("아이템 이미지")]
        public Image itemImage;
    }

    [SerializeField] InitField initField;

    public ItemData Data { get; private set; }

    public void SetItem(ItemData item, int count)
    {
        Data = item;
        initField.itemCountText.text = count.ToString();
        initField.itemNameText.text = item.ItemName;
        initField.itemImage.sprite = item.SpriteImage;
    }
}
