using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력 비율이 가장 낮은 아군 하나를 회복시키는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/SingleTargetHeal")]
public class Heal : Skill
{
    [Header("디버그용 힐 수치, 나중에는 캐릭터 수치로 추가")]
    [SerializeField]
    float healRate;

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return null;

        if (target != null && target.IsAlive)
        {
            target.Healed(healRate * self.CurAttackPoint);
            //TODO : 후딜 추가?하기
            //yield return new WaitForSeconds(5);
        }
    }
}
