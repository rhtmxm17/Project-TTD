using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : ScriptableObject
{
    [SerializeField] int id;
    public int Id => id;

    // 우선 웨이브는 생각하지 않고 만듬
    [SerializeField] List<MonsterInfo> monsters;
    /// <summary>
    /// 스테이지의 적 구성
    /// </summary>
    public List<MonsterInfo> Monsters => monsters;


    [SerializeField] List<RewardInfo> reward;
    /// <summary>
    /// 클리어 보상 목록
    /// </summary>
    public List<RewardInfo> Reward => reward;


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
}
