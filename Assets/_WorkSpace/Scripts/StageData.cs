using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class StageData : ScriptableObject
{
    /// <summary>
    /// 각 스테이지의 고유 번호
    /// </summary>
    public int Id => id;

    /// <summary>
    /// 스테이지를 대표하는 이름
    /// </summary>
    public string StageName => stageName;

    /// <summary>
    /// 스테이지의 적 구성
    /// </summary>
    public List<WaveInfo> Waves => waves;

    /// <summary>
    /// 클리어 보상 목록
    /// </summary>
    public List<RewardInfo> Reward => reward;

    /// <summary>
    /// (유저 데이터) 클리어 횟수
    /// </summary>
    public UserDataInt ClearCount { get; private set; }

    [SerializeField] int id;

    [SerializeField] string stageName;

    /// <summary>
    /// waves[웨이브 번호].monsters[몬스터 번호]
    /// </summary>
    [SerializeField] List<WaveInfo> waves;

    [SerializeField] List<RewardInfo> reward;

    private void OnEnable()
    {
        ClearCount = new UserDataInt($"Stages/{id}/ClearCount");
    }

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
    public struct RewardInfo
    {
        /// <summary>
        /// 보상 아이템 종류
        /// </summary>
        public ItemData rewardItem;

        /// <summary>
        /// 보상 개수
        /// </summary>
        public int number;
    }

#if UNITY_EDITOR
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
                reward = new List<RewardInfo>();

                // NAME
                stageName = cells[(int)Column.STAGE_NAME];
            }
            else
            {
                // 첫 행에 다른 ID가 나타났다면 더이상 자신의 데이터가 아님
                if (false == string.IsNullOrEmpty(cells[(int)Column.ID]))
                {
                    Debug.Log("다음 데이터에 도달");
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
                    continue;
                }

                if (false == float.TryParse(cells[(int)Column.MONSTER_POS_X], out monsterInfo.pose.x))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    continue;
                }

                if (false == float.TryParse(cells[(int)Column.MONSTER_POS_Y], out monsterInfo.pose.y))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    continue;
                }
                currentWave.Add(monsterInfo);
            }

            // 현재 행에 보상 정보가 있다면 추가
            ItemData rewardItemData = AssetDatabase.LoadAssetAtPath<ItemData>($"{DataTableManager.ItemAssetFolder}/{cells[(int)Column.REWARD_NAME]}.asset");
            if (rewardItemData != null)
            {
                RewardInfo rewardInfo = new RewardInfo() { rewardItem = rewardItemData };

                if (false == int.TryParse(cells[(int)Column.REWARD_NUMBER], out rewardInfo.number))
                {
                    Debug.LogError($"잘못된 데이터로 갱신 시도됨");
                    continue;
                }

                reward.Add(rewardInfo);
            }

            line++;
        }
    }
#endif
}
