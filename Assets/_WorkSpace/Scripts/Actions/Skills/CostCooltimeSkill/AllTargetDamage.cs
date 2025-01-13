using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 준비 동작 후 즉발로 모든 적에게 피해를 입히는 샘플
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/AllTargetDamage")]
public class AllTargetDamage : Skill
{
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

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        // 지정된 애니메이션 시작
        self.UnitAnimator.SetTrigger(animationHash);

        yield return waitPreDelay;

        if (self.Group.Enemy != null)
        {
            foreach (Combatable enemy in self.Group.Enemy.CharList) 
            {
                // 실제로 공격이 적용되는 구간
                if (enemy != null && enemy.IsAlive)
                    enemy.Damaged(self.AttackPoint.Value * atkMultiplier, self.igDefenseRate, self.characterData.StatusTable.type); // 타겟에게 데미지 적용
            }
        }
       
        // 히트스캔 이펙트 추가

        yield return waitPostDelay;
    }
}
