using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력 비율이 가장 낮은 아군 하나를 지속적으로 회복시키는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/SingleTargetRecorvery")]
public class Recovery : Skill
{

    [Header("지속 회복치의 총 배율 ex) 1.5f = 150%")]
    [SerializeField]
    float healMultiplier;
    [SerializeField]
    int duaringSec;

    WaitForSeconds ticDelay = new WaitForSeconds(1);

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return null;

        float healAmount = self.AttackPoint.Value * self.AttackFloorBuff * healMultiplier / duaringSec;//수치 조정

        if (target != null && target.IsAlive)
        {
            target.StartCoroutine(SkillRoutineImplement(target, healAmount));
        }
    }

    IEnumerator SkillRoutineImplement(Combatable target, float healAmountPerTic)
    {
        yield return null;

        for (int i = 0; i < duaringSec; i++)
        {
            target.Healed(healAmountPerTic);
            yield return ticDelay;
        }
    }

}
