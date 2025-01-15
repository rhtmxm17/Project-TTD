using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 단일 대상에게 공격력 버프를 주는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/SingleTargetAttackBuff")]
public class AttackBuff : Skill
{
    [Header("공격 배율")]
    [SerializeField]
    float atkRate;
    [SerializeField]
    float duringTime;

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return null;

        if (target != null && target.IsAlive)
        {
            target.StartCoroutine(StartBufftimeCO(target, (int)(atkRate * self.CurAttackPoint)));
            //TODO : 후딜 추가?하기
            //yield return new WaitForSeconds(5);
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
