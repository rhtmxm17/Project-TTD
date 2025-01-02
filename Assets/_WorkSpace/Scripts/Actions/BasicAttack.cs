using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill/BasicMeleeAttack")]

public class BasicAttack : Skill
{
    [SerializeField] Targeting targetingLogic;

/*    private void OnEnable()
    {
        // 런타임 진입시 필요한 데이터 캐싱
        animationHash = Animator.StringToHash(animationTriggerParam);
        waitPreDelay = new WaitForSeconds(preDelay);
        waitPostDelay = new WaitForSeconds(postDelay);
    }*/

    protected override IEnumerator SkillRoutineImplement(Combatable self)
    {
        yield return null;

        Combatable target = targetingLogic.GetTarget(self); // 설정된 규칙에 따라 타겟 산출

        // 지정된 애니메이션 시작
        //self.UnitAnimator.SetTrigger(animationHash);

        // 실제로 공격이 적용되는 구간
        if (target != null && target.IsAlive)
            target?.Damaged(self.AttackPoint.Value, self.igDefenseRate); // 타겟에게 데미지 적용

    }

}
