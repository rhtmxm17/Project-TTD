using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

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
            Texture2D texture = AssetPreview.GetAssetPreview(characterData.ModelPrefab.gameObject);
            GUI.DrawTexture(rect, texture);
        }

        base.OnInspectorGUI();
    }
}
#endif

public class CharacterData : ScriptableObject, ICsvRowParseable
{
    [System.Serializable]
    public struct Status
    {
        public float Range;
        public float BasicSkillCooldown;
        public float SecondSkillCooldown;

        /// <summary>
        /// 0레벨 공격력
        /// </summary>
        public float attackPointBase;

        /// <summary>
        /// 레벨별 공격력 성장치
        /// </summary>
        public float attackPointGrowth;
        public float healthPointBase;
        public float healthPointGrouth;
        public float defensePointBase;
        public float defensePointGrouth;
        public float defenseCon;
        public ElementType type;
        public RoleType roleType;
        public DragonVeinType dragonVeinType;
    }

    public int Id => id;

    public string Name => name;
    
    private int BonusStats => Level.Value % 10 == 0 ? ((Level.Value / 10) * 10) : 0;
    
    public float AttackPointLeveled => (((statusTable.attackPointBase + statusTable.attackPointGrowth) * Level.Value) * (1f + 0.1f * Enhancement.Value)) + BonusStats;

    public float HpPointLeveled => (((statusTable.healthPointBase + statusTable.healthPointGrouth) * Level.Value) * (1f + 0.1f * Enhancement.Value)) + BonusStats;

    public float DefensePointLeveled => (((statusTable.defensePointBase + statusTable.defensePointGrouth) * Level.Value) * (1f + 0.1f * Enhancement.Value)) + BonusStats;

    public float PowerLevel => (AttackPointLeveled + HpPointLeveled + DefensePointLeveled);
    
    public Sprite FaceIconSprite => faceIconSprite;

    public CharacterModel ModelPrefab => modelPrefab;

    public Status StatusTable => statusTable;

    /// <summary>
    /// 기본 공격
    /// </summary>
    public Skill BasicSkillDataSO => basicSkillDataSO;

    /// <summary>
    /// 기본 스킬
    /// </summary>
    public Skill SkillDataSO => skillDataSO;

    public Sprite NormalSkillIcon => normalSkillIcon;

    public string NormalSkillToolTip => normalSkillToolTip;

    /// <summary>
    /// 용맥 해방 스킬
    /// </summary>
    public Skill SecondSkillDataSO => secondSkillDataSO;

    public Sprite SpecialSkillIcon => specialSkillIcon;

    public string SpecialSkillToolTip => specialSkillToolTip;

    /// <summary>
    /// 전용 강화 아이템
    /// </summary>
    public int EnhanceItemID => enhanceItemId;
    
    [SerializeField] int id;
    [SerializeField] new string name;
    [SerializeField] Sprite faceIconSprite;
    [SerializeField] CharacterModel modelPrefab;
    [SerializeField] Status statusTable;
    [Header("Skill datas")]
    [SerializeField] Skill basicSkillDataSO;
    [SerializeField] Skill skillDataSO;
    [SerializeField] Sprite normalSkillIcon;
    [SerializeField] string normalSkillToolTip;
    [SerializeField] Skill secondSkillDataSO;
    [SerializeField] Sprite specialSkillIcon;
    [SerializeField] string specialSkillToolTip;
    [SerializeField] int getCharacterItemId;
    [SerializeField] int enhanceItemId;

    #region 유저 데이터 참조
    public UserDataInt Level => GameManager.UserData.GetCharacterLevel(id);
    public UserDataInt Enhancement => GameManager.UserData.GetCharacterEnhancement(id);
    public UserDataInt EnhanceMileagePerMill => GameManager.UserData.GetCharacterMileage(id);
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
        NS_ICON,
        NS_TOOLTIP,
        SPECIAL_SKILL,
        SS_COST,
        SS_ICON,
        SS_TOOLTIP,
        RANGE,
        ATK_BASE,
        ATK_GROWTH,
        DEF_BASE,
        DEF_GROWTH,
        HP_BASE,
        HP_GROWTH,
        DEFCON,
        DROPDOWN_CHAR_TYPE,
        CHAR_TYPE,
        DROPDOWN_ROLE_TYPE,
        ROLE_TYPE,
        DROPDOWN_DRAGONVEIN_TYPE,
        DRAGONVEIN_TYPE,
        GET_CHARACTER_ITEM,
        ENHANCE_ITEM,
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
        faceIconSprite = SearchAsset.SearchSpriteAsset(cells[(int)Column.FACE_ICON]);
        if (faceIconSprite == null)
            faceIconSprite = DataTableManager.Instance.DummySprite;

        // SHAPE
        //modelPrefab = AssetDatabase.LoadAssetAtPath<CharacterModel>($"{DataTableManager.PrefabsAssetFolder}/{cells[(int)Column.SHAPE]}.prefab");
        modelPrefab = SearchAsset.SearchPrefabAsset<CharacterModel>(cells[(int)Column.SHAPE]);

        // BASE_ATTACK
        basicSkillDataSO = SearchAsset.SearchSOAsset<Skill>(cells[(int)Column.BASE_ATTACK]);

        // NORMAL_SKILL
        skillDataSO = SearchAsset.SearchSOAsset<Skill>(cells[(int)Column.NORMAL_SKILL]);

        if (skillDataSO != null) // 일반 스킬이 기재되어 있다면
        {
            // NS_COOLDOWN
            if (false == float.TryParse(cells[(int)Column.NS_COOLDOWN], out statusTable.BasicSkillCooldown))
            {
                Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                return;
            }

            // NS_ICON
            normalSkillIcon = SearchAsset.SearchSpriteAsset(cells[(int)Column.NS_ICON]);
            if (normalSkillIcon == null)
                normalSkillIcon = DataTableManager.Instance.DummySprite;

            // NS_TOOLTIP
            normalSkillToolTip = cells[(int)Column.NS_TOOLTIP];
        }

        // SPECIAL_SKILL
        secondSkillDataSO = SearchAsset.SearchSOAsset<Skill>(cells[(int)Column.SPECIAL_SKILL]);

        if (secondSkillDataSO != null) // 특수 스킬이 기재되어 있다면
        {
            // SS_COST
            if (false == float.TryParse(cells[(int)Column.SS_COST], out statusTable.SecondSkillCooldown))
            {
                Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                return;
            }

            //Special_Skill_Icon
            specialSkillIcon = SearchAsset.SearchSpriteAsset(cells[(int)Column.SS_ICON]);

            if (specialSkillIcon == null)
                specialSkillIcon = DataTableManager.Instance.DummySprite;

            // SS_TOOLTIP
            specialSkillToolTip = cells[(int)Column.SS_TOOLTIP];
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
        statusTable.type = (ElementType)type;

        // CHAR_ROLE_TYPE
        if (false == int.TryParse(cells[(int)Column.ROLE_TYPE], out int roleType))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }
        statusTable.roleType = (RoleType)roleType;

        // CHAR_DRAGON_TYPE
        if (false == int.TryParse(cells[(int)Column.DRAGONVEIN_TYPE], out int dragonType))
        {
            Debug.LogError($"잘못된 데이터로 갱신 시도됨");
            return;
        }
        statusTable.dragonVeinType = (DragonVeinType)dragonType;

        // GET_CHARACTER_ITEM
        if (int.TryParse(cells[(int)Column.GET_CHARACTER_ITEM], out getCharacterItemId))
        {
            ItemData itemdata = DataTableManager.Instance.GetItemData(getCharacterItemId);
            if (itemdata == null)
            {
                Debug.LogError($"캐릭터 획득 아이템(ID:{id})을 찾지 못함");
            }
            else
            {
                /// 아이템 획득 이벤트에 그 개수를 검사해 캐릭터 또는 전용 강화재료를 획득하는 메서드를 추가한다

                // 직렬화된 UnityEvent 제거 (한 아이템 획득이 여러 메서드를 갖는 경우를 고려하지 않음)
                while (0 < itemdata.OnNumberChanged.GetPersistentEventCount())
                {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(itemdata.OnNumberChanged, 0);
                }
                // 직렬화되는 UnityEvent 등록
                UnityEditor.Events.UnityEventTools.AddPersistentListener(itemdata.OnNumberChanged, AcquireCharacter);
                EditorUtility.SetDirty(itemdata);
            }
        }
        else
        {
            getCharacterItemId = 0; // 해당 정보가 필요 없는 캐릭터라면 기본값으로 0 입력
        }

        // ENHANCE_ITEM
        if (false == int.TryParse(cells[(int)Column.ENHANCE_ITEM], out enhanceItemId))
        {
            enhanceItemId = 0; // 해당 정보가 필요 없는 캐릭터라면 기본값으로 0 입력
        }
    }
#endif

    private void AcquireCharacter(int itemNumber)
    {
        Debug.Log($"캐릭터({this.Name}) 획득 회수:{itemNumber}");
        if (itemNumber <= 0)
        {
            Debug.LogError("아이템 정보가 잘못됨!");
        }
        else if  (itemNumber == 1)
        {
            // 캐릭터 첫 획득일 경우
            GameManager.UserData.ApplyCharacter(this.id);
        }
        else
        {
            // 중복 획득일 경우
            ItemData enhanceItem = DataTableManager.Instance.GetItemData(enhanceItemId);

            UserDataManager.Instance.StartUpdateStream()
                .AddDBValue(enhanceItem.Number, 10)
                .Submit((result) =>
                {
                    Debug.Log("캐릭터 중복 획득으로 강화 아이템으로 전환");
                    var popup = GameManager.OverlayUIManager.PopupItemGain(new List<ItemGain> { new ItemGain() { item = enhanceItem, gain = 10 } });
                    popup.Title.text = "중복 캐릭터 획득 전환";
                });
        }

    }
}
