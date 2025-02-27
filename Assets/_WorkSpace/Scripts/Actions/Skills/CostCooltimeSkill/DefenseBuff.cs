using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력 비율이 가장 낮은 아군 하나의 방어력을 올리는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/SingleTargetDefenseBuff")]
public class DefenseBuff : Skill
{
    [Header("방어 배율")]
    [SerializeField]
    float defRate;
    [SerializeField]
    float duringTime;
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
            target.StartCoroutine(StartBufftimeCO(target, (int)(defRate * self.CurAttackPoint)));
            if (hitEffect != null)
            {
                Instantiate(hitEffect, target.transform.position, Quaternion.Euler(90,90,90));
            }
        }

        yield return waitPostDelay;

    }

    IEnumerator StartBufftimeCO(Combatable target, int amount)
    {
        yield return null;
        target.AddDefBuff(amount);
        yield return new WaitForSeconds(duringTime);
        if (target != null && target.IsAlive)
            target.RemoveDefBuff(amount);
    }
}
