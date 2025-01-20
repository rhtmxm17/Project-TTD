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
    // 스킬이펙트
    [SerializeField] ParticleSystem hitEffect;

    
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

        self.PlayAttckSnd();

        if (target != null && target.IsAlive)
        {
            target.Healed(healRate * self.CurAttackPoint);
            if (hitEffect != null)
            {
                Instantiate(hitEffect, target.transform.position, Quaternion.Euler(90,90,90));
            }

        }

        yield return waitPostDelay;

    }
}
