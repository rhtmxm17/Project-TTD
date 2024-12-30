using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ShopItemData")]
public class ShopItemData : ScriptableObject
{
    public int Id => id;
    [SerializeField] int id;

    /// <summary>
    /// 상점에서 표시되는 품목 이름
    /// </summary>
    public string ShopItemName => shopItemName;
    [SerializeField] string shopItemName;

    /// <summary>
    /// 상점에서 표시되는 이미지
    /// </summary>
    public Sprite Sprite => sprite;
    [SerializeField] Sprite sprite;

    /// <summary>
    /// 팝업에서 확인 가능한 해설
    /// </summary>
    public string Description => description;
    [SerializeField] string description;

    /// <summary>
    /// 가격
    /// </summary>
    public ItemGain Price => price;
    [SerializeField] ItemGain price;

    /// <summary>
    /// 품목 구성
    /// </summary>
    public List<ItemGain> Products => products;
    [SerializeField] List<ItemGain> products;

    /// <summary>
    /// 구매 횟수 제한 여부
    /// </summary>
    public bool IsLimited => isLimited;
    [SerializeField] bool isLimited;

    /// <summary>
    /// 구매 가능 횟수
    /// </summary>
    public int LimitedCount => limitedCount;
    [SerializeField] int limitedCount;

    #region 유저 데이터
    /// <summary>
    /// 구매한 횟수
    /// </summary>
    public UserDataInt Bought { get; private set; }

    private void OnEnable()
    {
        Bought = new UserDataInt($"ShopItems/{id}/Bought");
    }
    #endregion
}
