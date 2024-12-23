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
        buyButton.GetComponentInChildren<Button>().onClick.AddListener(Buy);
    }
    private void Buy()
    {
        ItemData item1 = GameManager.TableData.GetItemData(1);
        Debug.Log(item1.Number.Value);
        GameManager.TableData.GetItemData(2);
        Debug.Log(GameManager.TableData.GetItemData(2));

    }


}
