using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아군 전원의 방어력을 올리는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/AllTargetDefenseBuff")]
public class AllTargetDefenseBuff : Skill
{
    [Header("방어 배율")]
    [SerializeField]
    float defRate;
    [SerializeField]
    float duringTime;

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return null;

        foreach (Combatable friendlyTarget in self.Group.CharList)
        {
            if (friendlyTarget != null && friendlyTarget.IsAlive)
            {
                friendlyTarget.StartCoroutine(StartBufftimeCO(friendlyTarget, (int)(defRate * self.AttackPoint.Value)));
            }
        }

    }

    IEnumerator StartBufftimeCO(Combatable target, int amount)
    {
        yield return null;
        target.AddDefBuff(amount);
        yield return new WaitForSeconds(duringTime);
        target.RemoveDefBuff(amount);
    }
}
