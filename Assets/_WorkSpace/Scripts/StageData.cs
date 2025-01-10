using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(StageData))]
public class StageDataDataEditor : Editor
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

public enum StatusBuffType : int
{
    ERROR,
    ATK_PERCENTAGE,
    DEF_PERCENTAGE,
}

public class StageData : ScriptableObject, ICsvMultiRowParseable
{
    // ================== 테이블 데이터 속성 ==================

    /// <summary>
    /// 각 스테이지의 고유 번호
    /// </summary>
    public int Id => id;

    // === UI 관련 데이터 ===

    /// <summary>
    /// 스테이지를 대표하는 이름
    /// </summary>
    public string StageName => stageName;

    /// <summary>
    /// 대표 이미지(null일 수 있음, 이미지를 사용하는 팝업창 등에 기본값을 준비할것)
    /// </summary>
    public Sprite SpriteImage => spriteImage;

    /// <summary>
    /// 해설 텍스트
    /// </summary>
    public string Description => description;


    // === 인게임 데이터 ===

    /// <summary>
    /// 스테이지의 적 구성<br/>
    /// Waves[웨이브 번호].monsters[몬스터 번호]
    /// </summary>
    public List<WaveInfo> Waves => waves;

    /// <summary>
    /// 클리어 보상 목록
    /// </summary>
    public List<ItemGain> Reward => reward;

    /// <summary>
    /// 편성 칸에 적용되는 버프 목록
    /// </summary>
    public List<BuffInfo> TileBuff => tileBuff;

    /// <summary>
    /// 스테이지 제한 시간(초 단위)
    /// </summary>
    public int TimeLimit => TimeLimit;

    /// <summary>
    /// 스테이지 뒷배경 정보 프리팹
    /// </summary>
    public ScrollableBG BackgroundTypePrefab => backgroundTypePrefab;

    // === 특수 데이터 ===

    /// <summary>
    /// (스토리 재생 가능하다면) 에피소드 진입시 바로 재생될 스토리
    /// </summary>
    public StoryDirectingData PreStory => preStory;

    /// <summary>
    /// (존재한다면) 전투 이후 재생될 스토리
    /// </summary>
    public StoryDirectingData PostStory => postStory;

    // ================== 유저 데이터 속성 ==================

    /// <summary>
    /// (유저 데이터) 클리어 횟수
    /// </summary>
    public UserDataInt ClearCount => GameManager.UserData.GetStageClearCount(id);

    /// <summary>
    /// 해당 스테이지가 해금되었는지의 여부
    /// </summary>
    public bool IsOpened
    {
        get
        {
            foreach (int stageId in lockConditionStageIDs)
            {
                // 지정된 Id를 갖는 스테이지의 클리어 이력이 없으면 잠김 상태
                StageData prevStage = GameManager.TableData.GetStageData(stageId);
                if (prevStage != null && prevStage.ClearCount.Value == 0)
                {
                    return false;
                }
            }

            if (lockConditionStageIDs.Count == 0)
            {
                // 리스트가 비어있다면 기본값(이전 id의 스테이지) 조회
                StageData prevStage = GameManager.TableData.GetStageData(id - 1);
                
                // 이전 id가 비어있다면 (prevStage == null) 조건 없이 개방
                if (prevStage != null && prevStage.ClearCount.Value == 0)
                {
                    return false;
                }
            }

            // 모든 검사를 통과하면 해금 상태
            return true;
        }
    }

    // ================== 직렬화 ==================

    [SerializeField] int id;
    [SerializeField] string stageName;
    [SerializeField] List<WaveInfo> waves;
    [SerializeField] List<ItemGain> reward;
    [SerializeField] List<BuffInfo> tileBuff;
    [SerializeField] int timeLimit;
    [SerializeField] ScrollableBG backgroundTypePrefab;
    [SerializeField] Sprite spriteImage;
    [SerializeField, TextArea] string description;
    [SerializeField] StoryDirectingData preStory = null;
    [SerializeField] StoryDirectingData postStory = null;
    [SerializeField] List<int> lockConditionStageIDs; // 개방 조건에 해당하는 선행 스테이지 목록

    [System.Serializable]
    public struct WaveInfo
    {
        public List<MonsterInfo> monsters;
    }

    [System.Serializable]
    public struct MonsterInfo
    {
        /// <summary>
        /// 몬스터로 생성될 캐릭터
        /// </summary>
        public CharacterData character;

        /// <summary>
        /// 레벨
        /// </summary>
        public int level;

        /// <summary>
        /// 위치
        /// </summary>
        public Vector2 pose;
    }



    [System.Serializable]
    public struct BuffInfo
    {
        public StatusBuffType type;
        public float value;
        public int tileIndex;
    }

    /// <summary>
    /// 클리어 횟수를 증가시키고 플레이어가 클리어 이력이 없었다면 보상을 획득<br/>
    /// 콜백값은 보상 획득 여부와 무관함에 주의
    /// </summary>
    /// <param name="onCompleteCallback">완료시 콜백, 반환값은 DB 접속 성공 여부</param>
    public void UserGetRewardOnceAsync(UnityAction<bool> onCompleteCallback)
    {
        var stream = GameManager.UserData.StartUpdateStream();

        // 클리어 기록이 없다면 보상 획득을 스트림에 등록
        if (this.ClearCount.Value == 0)
        {
            foreach (var item in this.Reward)
            {
                stream.AddDBValue(item.item.Number, item.gain);
            }
        }
        
        // 클리어 횟수 갱신을 스트림에 둥록
        stream.AddDBValue(this.ClearCount, 1);

        stream.Submit(onCompleteCallback);
    }

#if UNITY_EDITOR
    #region 데이터 파싱
    private enum Column
    {
        ID,
        /// <summary>
        /// 사용되지 않음(DataManager에서 사용)
        /// </summary>
        FILE_NAME,
        STAGE_NAME,

        WAVE,

        MONSTER_NAME,
        MONSTER_LEVEL,
        MONSTER_POS_X,
        MONSTER_POS_Y,

        REWARD_NAME,
        REWARD_NUMBER,

        DROPDOWN_BUFF_TYPE,
        BUFF_TYPE,
        BUFF_VALUE,
        BUFF_TILE_INDEX,

        TIME_LIMIT,
        BACKGROUND_TYPE,

        SPRITE_IMAGE,
        DESCRIPTION,

        PRE_STORY,
        POST_STORY,

        LOCK_CONDITON_ID,
    }

    public void ParseCsvMultiRow(string[] lines, ref int line)
    {
        bool isFirst = true;
        List<MonsterInfo> currentWave = null;
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

                waves = new List<WaveInfo>();
                reward = new List<ItemGain>();
                tileBuff = new List<BuffInfo>();
                lockConditionStageIDs = new List<int>();

                // NAME
                stageName = cells[(int)Column.STAGE_NAME];

                // TIME_LIMIT
                if (false == int.TryParse(cells[(int)Column.TIME_LIMIT], out timeLimit))
                {
                    timeLimit = 0;
                }

                /////// null을 허용하는 데이터들

                // BACKGROUND_TYPE
                backgroundTypePrefab = AssetDatabase.LoadAssetAtPath<ScrollableBG>(
                    $"{DataTableManager.PrefabsAssetFolder}/ScrollBackground/{cells[(int)Column.BACKGROUND_TYPE]}.prefab");

                // SPRITE_IMAGE
                spriteImage = AssetDatabase.LoadAssetAtPath<Sprite>($"{DataTableManager.SpritesAssetFolder}/{cells[(int)Column.SPRITE_IMAGE]}.asset");

                // PRE_STORY
                preStory = AssetDatabase.LoadAssetAtPath<StoryDirectingData>($"{DataTableManager.StoryAssetFolder}/{cells[(int)Column.PRE_STORY]}.asset");

                // NAME
                description = cells[(int)Column.DESCRIPTION];

                // POST_STORY
                postStory = AssetDatabase.LoadAssetAtPath<StoryDirectingData>($"{DataTableManager.StoryAssetFolder}/{cells[(int)Column.POST_STORY]}.asset");
            }
            else
            {
                // 첫 행에 다른 ID가 나타났다면 더이상 자신의 데이터가 아님
                if (false == string.IsNullOrEmpty(cells[(int)Column.ID]))
                {
                    Debug.Log($"스태이지({stageName}) 생성됨");
                    line--;
                    return;
                }
            }

            // 웨이브 구분용 열에 데이터가 존재한다면
            if (false == string.IsNullOrEmpty(cells[(int)Column.WAVE]))
            {
                WaveInfo newWave = new WaveInfo();
                newWave.monsters = currentWave = new List<MonsterInfo>();
                waves.Add(newWave);
            }

            // 현재 행에 몬스터 정보가 있다면 추가
            CharacterData monsterCharacterData = AssetDatabase.LoadAssetAtPath<CharacterData>($"{DataTableManager.CharacterAssetFolder}/{cells[(int)Column.MONSTER_NAME]}.asset");
            if (monsterCharacterData != null)
            {
                MonsterInfo monsterInfo = new MonsterInfo() { character = monsterCharacterData };

                if (false == int.TryParse(cells[(int)Column.MONSTER_LEVEL], out monsterInfo.level))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    return;
                }

                if (false == float.TryParse(cells[(int)Column.MONSTER_POS_X], out monsterInfo.pose.x))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    return;
                }

                if (false == float.TryParse(cells[(int)Column.MONSTER_POS_Y], out monsterInfo.pose.y))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    return;
                }
                currentWave.Add(monsterInfo);
            }

            // 현재 행에 보상 정보가 있다면 추가
            ItemData rewardItemData = AssetDatabase.LoadAssetAtPath<ItemData>($"{DataTableManager.ItemAssetFolder}/{cells[(int)Column.REWARD_NAME]}.asset");
            if (rewardItemData != null)
            {
                ItemGain rewardInfo = new ItemGain() { item = rewardItemData };

                if (false == int.TryParse(cells[(int)Column.REWARD_NUMBER], out rewardInfo.gain))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    return;
                }

                reward.Add(rewardInfo);
            }

            // 현재 행에 버프 정보가 있다면 추가
            if (int.TryParse(cells[(int)Column.BUFF_TYPE], out int buffType))
            {
                BuffInfo buffInfo = new BuffInfo();
                buffInfo.type = (StatusBuffType)buffType;

                if (false == float.TryParse(cells[(int)Column.BUFF_VALUE], out buffInfo.value))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    return;
                }

                if (false == int.TryParse(cells[(int)Column.BUFF_TILE_INDEX], out buffInfo.tileIndex))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    return;
                }

                tileBuff.Add(buffInfo);
            }

            // 현재 행에 선행 스테이지 정보가 있다면 추가
            if (int.TryParse(cells[(int)Column.LOCK_CONDITON_ID], out int lockId))
            {
                lockConditionStageIDs.Add(lockId);
            }

            line++;
        }
    }
    #endregion 데이터 파싱
#endif // UNITY_EDITOR
}
