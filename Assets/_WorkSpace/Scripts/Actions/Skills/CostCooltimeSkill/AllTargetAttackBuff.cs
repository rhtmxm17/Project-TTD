using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아군 전원의 공격력을 올리는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/AllTargetAttackBuff")]
public class AllTargetAttackBuff : Skill
{
    [Header("공격버프 배율")]
    [SerializeField]
    float atkRate;
    [SerializeField]
    float duringTime;
    [SerializeField, Tooltip("선딜레이")] float preDelay = 0.25f;
    [SerializeField, Tooltip("후딜레이")] float postDelay = 0.75f;

    // 캐싱 데이터
    private WaitForSeconds waitPreDelay;
    private WaitForSeconds waitPostDelay;

    private void OnEnable()
    {
        // 런타임 진입시 필요한 데이터 캐싱
        waitPreDelay = new WaitForSeconds(preDelay);
        waitPostDelay = new WaitForSeconds(postDelay);
    }

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return waitPreDelay;
        Debug.Log("<color=red>공격 나감!</color>");

        foreach (Combatable friendlyTarget in self.Group.CharList)
        {
            if (friendlyTarget != null && friendlyTarget.IsAlive)
            {
                friendlyTarget.StartCoroutine(StartBufftimeCO(friendlyTarget, (int)(atkRate * self.CurAttackPoint)));
            }
        }

        yield return waitPostDelay;

    }

    IEnumerator StartBufftimeCO(Combatable target, int amount)
    {
        yield return null;
        target.AddAtkBuff(amount);
        yield return new WaitForSeconds(duringTime);
        target.RemoveAtkBuff(amount);
    }
}
