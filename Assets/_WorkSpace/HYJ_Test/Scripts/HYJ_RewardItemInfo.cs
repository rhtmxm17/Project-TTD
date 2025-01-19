using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HYJ_RewardItemInfo : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemAmountText;
    [SerializeField] TMP_Text itemNameText;
    
    /// <summary>
    /// 아이템 이미지 초기 설정
    /// </summary>
    /// <param name="itemGain"></param>
    public void InitRewardItemInfo(ItemGain itemGain)
    {
        itemImage.GetComponent<Image>().sprite = itemGain.item.SpriteImage;
        itemAmountText.text = $"x{itemGain.gain}";
        itemNameText.text = itemGain.item.ItemName;
    }
}
