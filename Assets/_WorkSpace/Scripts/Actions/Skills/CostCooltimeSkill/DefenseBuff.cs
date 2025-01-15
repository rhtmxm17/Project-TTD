using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력 비율이 가장 낮은 아군 하나의 방어력을 올리는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/SingleTargetDefenseBuff")]
public class DefenseBuff : Skill
{
    [Header("방어 배율")]
    [SerializeField]
    float defRate;
    [SerializeField]
    float duringTime;

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return null;

        if (target != null && target.IsAlive)
        {
            target.StartCoroutine(StartBufftimeCO(target, (int)(defRate * self.AttackPoint.Value * self.AttackFloorBuff)));
            //TODO : 후딜 추가?하기
            //yield return new WaitForSeconds(5);
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
