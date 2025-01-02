using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinTicketDipalyer : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        text.text = $"{GameManager.TableData.GetItemData(9).Number.Value.ToString()} / 3";
    }
}
