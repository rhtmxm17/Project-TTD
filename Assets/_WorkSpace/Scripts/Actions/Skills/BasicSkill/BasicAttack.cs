using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill/BasicMeleeAttack")]

public class BasicAttack : Skill
{
    [SerializeField] float atkMultiplier = 1f;
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

        // 실제로 공격이 적용되는 구간
        if (target != null && target.IsAlive)
        {
            target.Damaged(self.CurAttackPoint, self.igDefenseRate, self.characterData.StatusTable.type); // 타겟에게 데미지 적용
            if (hitEffect != null)
            {
                Instantiate(hitEffect, target.transform.position, Quaternion.Euler(90,90,90));
            }
        }
        yield return waitPostDelay;
    }

}
