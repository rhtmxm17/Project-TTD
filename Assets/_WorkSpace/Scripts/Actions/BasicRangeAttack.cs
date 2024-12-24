using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill/BasicRangeAttack")]
public class BasicRangeAttack : Skill
{

    [SerializeField] Targeting targetingLogic;
    [SerializeField] Sprite ProjectileSprite;
    [SerializeField] Projectile projectilePrefab;
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
        if (target.IsAlive)
        {
            var projectile = Instantiate(projectilePrefab);
            projectile.transform.position = self.transform.position;
            projectile.StartChase(target, self.AttackPoint.Value, ProjectileSprite);
        }

    }
}
