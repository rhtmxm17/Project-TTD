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

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return null;

        foreach (Combatable friendlyTarget in self.Group.CharList)
        {
            if (friendlyTarget != null && friendlyTarget.IsAlive)
            {
                friendlyTarget.StartCoroutine(StartBufftimeCO(friendlyTarget, (int)(atkRate * self.CurAttackPoint)));
            }
        }

    }

    IEnumerator StartBufftimeCO(Combatable target, int amount)
    {
        yield return null;
        target.AddAtkBuff(amount);
        yield return new WaitForSeconds(duringTime);
        target.RemoveAtkBuff(amount);
    }
}
