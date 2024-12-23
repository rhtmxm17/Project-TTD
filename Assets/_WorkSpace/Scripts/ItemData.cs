using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemData : ScriptableObject, ICsvRowParseable
{
    [SerializeField] int id;
    public int Id => id;

    [SerializeField] string itemName;
    public string ItemName => itemName;

    [SerializeField] Sprite spriteImage;
    public Sprite SspriteImage => spriteImage;

    [SerializeField] string description;
    public string Description => description;

    #region 유저 데이터
    /// <summary>
    /// 소지 개수
    /// </summary>
    public UserDataInt Number { get; private set; }

    private void OnEnable()
    {
        Number = new UserDataInt($"Items/{id}");
    }
    #endregion

#if UNITY_EDITOR
    private enum Column
    {
        ID,
        /// <summary>
        /// 사용되지 않음(DataManager에서 사용)
        /// </summary>
        FILE_NAME,
        NAME,
        SPRITE,
        DESCRIPTION,
    }

    public void ParseCsvRow(string[] cells)
    {
        // ID
        if (false == int.TryParse(cells[(int)Column.ID], out id))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // NAME
        itemName = cells[(int)Column.NAME];

        // SPRITE
        spriteImage = AssetDatabase.LoadAssetAtPath<Sprite>($"{DataTableManager.SpritesAssetFolder}/{cells[(int)Column.SPRITE]}.asset");

        // DESCRIPTION
        description = cells[(int)Column.DESCRIPTION];
    }
#endif
}
