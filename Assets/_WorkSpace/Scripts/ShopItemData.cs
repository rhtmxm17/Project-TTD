using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ShopItemData))]
public class ShopItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("테이블 매니저 바로가기"))
        {
            UnityEditor.Selection.activeObject = DataTableManager.Instance;
        }

        base.OnInspectorGUI();
    }
}
#endif

/// <summary>
/// 상점 판매 품목 정보
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/ShopItemData")]
public class ShopItemData : ScriptableObject, ICsvMultiRowParseable
{
    public int Id => id;
    [SerializeField] int id;

    /// <summary>
    /// 상점에서 표시되는 품목 이름
    /// </summary>
    public string ShopItemName => shopItemName;
    [SerializeField] string shopItemName;

    /// <summary>
    /// 상점에서 표시되는 이미지(시트에서 지정하지 않았다면 첫번째 상품의 이미지)
    /// </summary>
    public Sprite Sprite
    {
        get
        {
            if (sprite == null)
                return Products[0].item.SpriteImage;
            else
                return sprite;
        } 
    }
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

    /// <summary>
    ///  복수구매여부 T면 구매확인창으로 여러개 구매
    /// </summary>
    public bool IsMany => isMany;
    [SerializeField] bool isMany;

    #region 유저 데이터
    /// <summary>
    /// 구매한 횟수
    /// </summary>
    public UserDataInt Bought => GameManager.UserData.GetPackageBought(id);

    #endregion

#if UNITY_EDITOR
    private enum Column
    {
        ID,
        /// <summary>
        /// 사용되지 않음(DataManager에서 사용)
        /// </summary>
        FILE_NAME,
        PACKAGE_NAME,
        SPRITE,
        DESCRIPTION,
        PRICE_ITEM,
        PRICE_NUMBER,
        PRODUCTS_ITEM,
        PRODUCTS_NUMBER,

        IS_LIMITED,
        LIMITED_COUNT,
        IS_MANY,
    }

    public void ParseCsvMultiRow(string[] lines, ref int line)
    {
        bool isFirst = true;
        while (lines.Length > line)
        {
            string[] cells = lines[line].Split(',');

            if (isFirst) // 첫 줄 한정
            {
                isFirst = false;
                // ID
                if (false == int.TryParse(cells[(int)Column.ID], out this.id))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    return;
                }

                products = new List<ItemGain>();

                // NAME
                shopItemName = cells[(int)Column.PACKAGE_NAME];

                // SPRITE
                // 공백일 경우 첫 번째 상품의 이미지 사용
                if (string.IsNullOrEmpty(cells[(int)Column.SPRITE]))
                    sprite = null;
                else
                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{DataTableManager.SpritesAssetFolder}/{cells[(int)Column.SPRITE]}.asset");

                // DESCRIPTION
                description = cells[(int)Column.DESCRIPTION];

                // 공백일 경우 무료 품목
                if (string.IsNullOrEmpty(cells[(int)Column.PRICE_ITEM]))
                    price.item = null;
                else
                {
                    price.item = AssetDatabase.LoadAssetAtPath<ItemData>($"{DataTableManager.ItemAssetFolder}/{cells[(int)Column.PRICE_ITEM]}.asset");
                    if (null == price.item)
                    {
                        Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                        return;
                    }

                    // PRICE_NUMBER
                    if (false == int.TryParse(cells[(int)Column.PRICE_NUMBER], out price.gain))
                    {
                        Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                        return;
                    }
                }

                // IS_LIMITED: 테이블값이 T면 구매횟수 제한 존재
                isLimited = ("T" == cells[(int)Column.IS_LIMITED]);

                // LIMITED_COUNT
                if (false == int.TryParse(cells[(int)Column.LIMITED_COUNT], out limitedCount))
                {
                    Debug.Log($"구매 가능 횟수 데이터에 기본값(0) 적용됨");
                    limitedCount = 0;
                }
                
                // IS_MANY: 테이블값이 T면 복수구매창(구매확인창) 을 엶, F면 그냥 limitedCount로
                isMany = ("T" == cells[(int)Column.IS_MANY]);
                if (string.IsNullOrEmpty(cells[(int)Column.IS_MANY]))
                    isMany = false;
                
            }
            else
            {
                // 첫 행에 다른 ID가 나타났다면 더이상 자신의 데이터가 아님
                if (false == string.IsNullOrEmpty(cells[(int)Column.ID]))
                {
                    Debug.Log($"상품 데이터({shopItemName}) 생성됨");
                    line--;
                    return;
                }
            }

            // 현재 행에 품목 정보가 있다면 추가
            ItemData productItemData = AssetDatabase.LoadAssetAtPath<ItemData>($"{DataTableManager.ItemAssetFolder}/{cells[(int)Column.PRODUCTS_ITEM]}.asset");
            if (productItemData != null)
            {
                ItemGain productData = new ItemGain() { item = productItemData };

                if (false == int.TryParse(cells[(int)Column.PRODUCTS_NUMBER], out productData.gain))
                {
                    Debug.Log($"상품 데이터에 기본값(1개) 적용됨");
                    productData.gain = 1;
                }

                products.Add(productData);
            }

            line++;
        }
    }
#endif
}
