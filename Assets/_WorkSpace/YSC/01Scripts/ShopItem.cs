using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] TMP_Text itemName;
    [SerializeField] int itemPrice;
    [SerializeField] Image itemImage;
    [SerializeField] Button buyButton;

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        
    }
    private void Buy()
    {

    }


}
