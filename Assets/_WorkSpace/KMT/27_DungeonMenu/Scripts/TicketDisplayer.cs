using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TicketDisplayer : MonoBehaviour
{

    [SerializeField]
    Slider ticketSlider;
    [SerializeField]
    TextMeshProUGUI ticketText;

    const int MAX_TICKET_COUNT = 3;

    private void OnEnable()
    {
        ItemData data = DataTableManager.Instance.GetItemData(9/*골드티켓*/);

        ticketSlider.value = data.Number.Value / (float)MAX_TICKET_COUNT;
        ticketText.text = $"[ {data.Number.Value} / {MAX_TICKET_COUNT} ]";
    }



}
