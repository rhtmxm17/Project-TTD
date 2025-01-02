using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayUIManager : SingletonBehaviour<OverlayUIManager>
{
    [SerializeField] ItemGainPopup itemGainPopupPrefab;

    private void Awake()
    {
        RegisterSingleton(this);
    }

    public ItemGainPopup PopupItemGain(List<ItemGain> gain = null)
    {
        ItemGainPopup popupInstance = Instantiate(itemGainPopupPrefab, this.transform);
        if (gain != null)
        {
            popupInstance.AddItemGainCell(gain);
        }

        return popupInstance;
    }
}
