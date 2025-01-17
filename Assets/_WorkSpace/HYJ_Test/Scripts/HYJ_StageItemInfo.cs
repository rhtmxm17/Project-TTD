using UnityEngine;

public class HYJ_StageItemInfo : MonoBehaviour
{
    [SerializeField] GameObject rewardItemPrefab;
    private void Start()
    {
        InitStageItemInfo();
    }

    /// <summary>
    /// 스테이지 아이템 정보
    /// </summary>
    private void InitStageItemInfo()
    {
        StageData curStageData = GameManager.Instance.sceneChangeArgs.stageData;
        
        foreach (var iReward in curStageData.Reward)
        {
            GameObject iItem = Instantiate(rewardItemPrefab, transform);
            iItem.GetComponent<HYJ_RewardItemInfo>().InitRewardItemInfo(iReward);
        }
    }
}
