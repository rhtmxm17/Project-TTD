using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HYJ_RewardItemInfo : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemAmountText;
    [SerializeField] TMP_Text itemNameText;
    
    public void InitRewardItemInfo(ItemGain itemGain)
    {
        
        itemImage.GetComponent<Image>().sprite = itemGain.item.SpriteImage;
        itemAmountText.text = itemGain.gain.ToString();
        itemNameText.text = itemGain.item.ItemName;
    }
}
