using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    CharacterData characterData;

    private void OnEnable()
    {
        characterData = target as CharacterData;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("테이블 매니저 바로가기"))
        {
            UnityEditor.Selection.activeObject = DataTableManager.Instance;
        }

        Rect rect = GUILayoutUtility.GetAspectRect(2f);
        rect.width *= 0.5f;
        if (characterData.FaceIconSprite != null)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(characterData.FaceIconSprite);
            GUI.DrawTexture(rect, texture);
        }

        rect.x += rect.width;
        if (characterData.ModelPrefab != null)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(characterData.ModelPrefab);
            GUI.DrawTexture(rect, texture);
        }

        base.OnInspectorGUI();
    }
}
#endif

/// <summary>
/// 캐릭터의 속성입니다
/// </summary>
public enum CharacterType
{
    _0,
    _1,
    _2,
    _3,
    _4,
}

public class CharacterData : ScriptableObject, ICsvRowParseable
{
    /// <summary>
    /// 24.12.19 김민태
    ///     - skill so 데이터 필드 추가
    /// </summary>

    // 능력치 표 내지는 계산식으로 변경 필요함
    [System.Serializable]
    public struct Status
    {
        public float Range;
        public float BasicSkillCooldown;
        public float SecondSkillCost;

        public float attackPointBase;
        public float attackPointGrowth;
        public float healthPointBase;
        public float healthPointGrouth;
        public float defensePointBase;
        public float defensePointGrouth;
        public float defenseCon;
        public CharacterType type;
    }

    [SerializeField] int id;
    public int Id => id;

    [SerializeField] new string name;
    public string Name => name;

    [SerializeField] Sprite faceIconSprite;
    public Sprite FaceIconSprite => faceIconSprite;

    [SerializeField] GameObject modelPrefab;
    public GameObject ModelPrefab => modelPrefab;

    [SerializeField] Status statusTable;
    public Status StatusTable => statusTable;

    [Header("Skill datas")]
    [SerializeField] Skill basicSkillDataSO;
    public Skill BasicSkillDataSO => basicSkillDataSO;

    [SerializeField] Skill skillDataSO;
    public Skill SkillDataSO => skillDataSO;

    [SerializeField] Skill secondSkillDataSO;
    public Skill SecondSkillDataSO => secondSkillDataSO;

    #region 유저 데이터
    /// <summary>
    /// 유저 데이터. DataManager의 LoadUserData()가 호출된 적이 있어야 정상적인 값을 갖는다<br/>
    /// 주의: 에디터의 Enter Play Mode Settings에서 도메인 리로드가 비활성화 되어있을 경우 이전 실행시의 값이 남아있을 수 있음
    /// 주의2: 로그아웃을 구현해야 한다면 마찬가지로 이전 유저의 값이 남아있으므로 인증 정보 변경시 정리하는 메서드 추가할것
    /// </summary>
    public UserDataInt Level { get; private set; }
    public UserDataInt Enhancement { get; private set; }

    private void OnEnable()
    {
        Level = new UserDataInt($"Characters/{id}/Level");
        Enhancement = new UserDataInt($"Characters/{id}/Enhancement");
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
        FACE_ICON,
        SHAPE,
        BASE_ATTACK,
        NORMAL_SKILL,
        NS_COOLDOWN,
        SPECIAL_SKILL,
        SS_COST,
        RANGE,
        ATK_BASE,
        ATK_GROWTH,
        DEF_BASE,
        DEF_GROWTH,
        HP_BASE,
        HP_GROWTH,
        DEFCON,
        CHAR_TYPE,
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
        name = cells[(int)Column.NAME];

        // FACE_ICON
        faceIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{DataTableManager.SpritesAssetFolder}/{cells[(int)Column.FACE_ICON]}.asset");
        if (faceIconSprite == null)
            faceIconSprite = DataTableManager.Instance.DummySprite;

        // SHAPE
        modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{DataTableManager.PrefabsAssetFolder}/{cells[(int)Column.SHAPE]}.prefab");

        // BASE_ATTACK
        basicSkillDataSO = AssetDatabase.LoadAssetAtPath<Skill>($"{DataTableManager.SkillAssetFolder}/{cells[(int)Column.BASE_ATTACK]}.asset");

        // NORMAL_SKILL
        skillDataSO = AssetDatabase.LoadAssetAtPath<Skill>($"{DataTableManager.SkillAssetFolder}/{cells[(int)Column.NORMAL_SKILL]}.asset");

        // SPECIAL_SKILL
        secondSkillDataSO = AssetDatabase.LoadAssetAtPath<Skill>($"{DataTableManager.SkillAssetFolder}/{cells[(int)Column.SPECIAL_SKILL]}.asset");

        // NS_COOLDOWN
        if (false == float.TryParse(cells[(int)Column.NS_COOLDOWN], out statusTable.BasicSkillCooldown))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // SS_COST
        if (false == float.TryParse(cells[(int)Column.SS_COST], out statusTable.SecondSkillCost))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // RANGE
        if (false == float.TryParse(cells[(int)Column.RANGE], out statusTable.Range))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // ATK_BASE
        if (false == float.TryParse(cells[(int)Column.ATK_BASE], out statusTable.attackPointBase))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // ATK_GROWTH
        if (false == float.TryParse(cells[(int)Column.ATK_GROWTH], out statusTable.attackPointGrowth))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // DEF_BASE
        if (false == float.TryParse(cells[(int)Column.DEF_BASE], out statusTable.defensePointBase))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // DEF_GROWTH
        if (false == float.TryParse(cells[(int)Column.DEF_GROWTH], out statusTable.defensePointGrouth))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // HP_BASE
        if (false == float.TryParse(cells[(int)Column.HP_BASE], out statusTable.healthPointBase))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // HP_GROWTH
        if (false == float.TryParse(cells[(int)Column.HP_GROWTH], out statusTable.healthPointGrouth))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // DEFCON
        if (false == float.TryParse(cells[(int)Column.DEFCON], out statusTable.defenseCon))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }

        // CHAR_TYPE
        if (false == int.TryParse(cells[(int)Column.CHAR_TYPE], out int type))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }
        statusTable.type = (CharacterType)type;

    }
#endif

}
