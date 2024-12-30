using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    ItemData itemData;

    private void OnEnable()
    {
        itemData = target as ItemData;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("테이블 매니저 바로가기"))
        {
            UnityEditor.Selection.activeObject = DataTableManager.Instance;
        }

        Rect rect = GUILayoutUtility.GetAspectRect(2f);
        rect.width *= 0.5f;
        if (itemData.SpriteImage != null)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(itemData.SpriteImage);
            GUI.DrawTexture(rect, texture);
        }

        base.OnInspectorGUI();
    }
}
#endif

[System.Serializable]
public struct ItemGain
{
    public ItemData item;
    public int gain;
}

public class ItemData : ScriptableObject, ICsvRowParseable
{
    [SerializeField] int id;
    public int Id => id;

    [SerializeField] string itemName;
    public string ItemName => itemName;

    [SerializeField] Sprite spriteImage;
    public Sprite SpriteImage => spriteImage;

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
        if (spriteImage == null)
            spriteImage = DataTableManager.Instance.DummySprite;

        // DESCRIPTION
        description = cells[(int)Column.DESCRIPTION];
    }
#endif
}
