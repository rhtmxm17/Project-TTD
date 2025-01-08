using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEpisodeData : ScriptableObject
{
    public int Id => id;

    /// <summary>
    /// 에피소드 진입시 바로 재생될 스토리
    /// </summary>
    public StoryDirectingData PreStory => preStory;

    /// <summary>
    /// (존재한다면) 스토리 전투
    /// </summary>
    public StageData Stage => stage;

    /// <summary>
    /// 클리어 보상 목록<br/>
    /// 전투가 없다면 스토리 재생 직후, 있다면 전투 승리 후 지급
    /// </summary>
    public List<ItemGain> Reward => reward;

    /// <summary>
    /// (존재한다면) 전투 이후 재생될 스토리
    /// </summary>
    public StoryDirectingData PostStory => postStory;

    /// <summary>
    /// (유저 데이터) 클리어 횟수
    /// </summary>
    public UserDataInt ClearCount { get; private set; }

    [SerializeField] int id;
    [SerializeField] StoryDirectingData preStory;
    [SerializeField] StageData stage = null;
    [SerializeField] List<ItemGain> reward;
    [SerializeField] StoryDirectingData postStory = null;

    private void OnEnable()
    {
        ClearCount = new UserDataInt($"Story/{id}/ClearCount");
    }
}
