using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject, ISheetManageable
{
    // 능력치 표 내지는 계산식으로 변경 필요함
    [System.Serializable]
    public struct Status
    {
        public float Range;

    }

    [SerializeField] int id;
    public int Id => id;

    [SerializeField] string name;
    public string Name => name;

    [SerializeField] GameObject modelPrefab;
    public GameObject ModelPrefab => modelPrefab;

    [SerializeField] Status statusTable;
    public Status StatusTable => statusTable;

    [SerializeField] Sprite skillSprite;
    public Sprite SkillSprite => skillSprite;

    [SerializeField] Sprite faceIconSprite;
    public Sprite FaceIconSprite => faceIconSprite;

#if UNITY_EDITOR
    private enum Column
    {
        ID,
        /// <summary>
        /// 사용되지 않음(DataManager에서 사용)
        /// </summary>
        FILE_NAME,
        NAME,
        RANGE,
        SHAPE,
        SKILL_SPRITE,
    }

    public void ParseCsvLine(string[] cells)
    {
        // ID
        if (false == int.TryParse(cells[(int)Column.ID], out id))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // NAME
        name = cells[(int)Column.NAME];

        // RANGE
        if (false == float.TryParse(cells[(int)Column.RANGE], out statusTable.Range))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // SHAPE
        modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_WorkSpace/Prefabs/{cells[(int)Column.SHAPE]}.prefab");

        // SKILL_SPRITE
        skillSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/_WorkSpace/Sprites/{cells[(int)Column.SKILL_SPRITE]}.asset");
    }
#endif
}
