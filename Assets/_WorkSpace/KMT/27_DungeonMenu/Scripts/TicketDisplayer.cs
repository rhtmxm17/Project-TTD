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

    ItemData data;

    private void Awake()
    {
        data = DataTableManager.Instance.GetItemData(9/*골드티켓*/);
    }

    private void OnEnable()
    {

        ticketSlider.value = data.Number.Value / (float)MAX_TICKET_COUNT;
        ticketText.text = $"[ {data.Number.Value} / {MAX_TICKET_COUNT} ]";

        data.Number.onValueChanged += UpdateGoldTicket;
    }


    private void OnDisable()
    {
        data.Number.onValueChanged -= UpdateGoldTicket;
    }

    void UpdateGoldTicket(long ticket)
    {
        ticketSlider.value = data.Number.Value / (float)MAX_TICKET_COUNT;
        ticketText.text = $"[ {data.Number.Value} / {MAX_TICKET_COUNT} ]";
    }

}
