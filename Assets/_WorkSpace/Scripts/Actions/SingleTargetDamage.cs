using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 준비 동작 후 즉발로 피해를 입히는 샘플
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/SingleTargetDamage")]
public class SingleTargetDamage : Skill
{
    [SerializeField] Targeting targetingLogic;
    [SerializeField] float atkMultiplier = 1f;
    [SerializeField] string animationTriggerParam;
    [SerializeField, Tooltip("선딜레이")] float preDelay;
    [SerializeField, Tooltip("후딜레이")] float postDelay;

    // 캐싱 데이터
    private int animationHash;
    private WaitForSeconds waitPreDelay;
    private WaitForSeconds waitPostDelay;

    private void OnEnable()
    {
        // 런타임 진입시 필요한 데이터 캐싱
        animationHash = Animator.StringToHash(animationTriggerParam);
        waitPreDelay = new WaitForSeconds(preDelay);
        waitPostDelay = new WaitForSeconds(postDelay);
    }

    protected override IEnumerator SkillRoutineImplement(Combatable self)
    {
        Combatable target = targetingLogic.GetTarget(self); // 설정된 규칙에 따라 타겟 산출

        // 지정된 애니메이션 시작
        self.UnitAnimator.SetTrigger(animationHash);

        yield return waitPreDelay;
        
        // 실제로 공격이 적용되는 구간
        if(target.IsAlive)
            target?.Damaged(self.AttackPoint.Value * atkMultiplier); // 타겟에게 데미지 적용
        
        // 히트스캔 이펙트 추가

        yield return waitPostDelay;
    }
}
