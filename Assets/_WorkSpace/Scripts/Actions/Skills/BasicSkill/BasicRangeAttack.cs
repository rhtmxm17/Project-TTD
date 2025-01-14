using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill/BasicRangeAttack")]
public class BasicRangeAttack : Skill
{
    [SerializeField] Sprite ProjectileSprite;
    [SerializeField] Projectile projectilePrefab;

    [SerializeField] float atkMultiplier = 1f;
    [SerializeField, Tooltip("선딜레이")] float preDelay = 0.25f;
    [SerializeField, Tooltip("후딜레이")] float postDelay = 0.75f;

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

        // 실제로 공격이 적용되는 구간
        if (target != null && target.IsAlive)
        {
            var projectile = Instantiate(projectilePrefab);
            projectile.transform.position = self.transform.position;
            projectile.StartChase(target, self.AttackPoint.Value, self.igDefenseRate, ProjectileSprite, self.characterData.StatusTable.type);
        }

        yield return waitPostDelay;
    }
}
