using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 단일 대상에게 공격력 버프를 주는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/SingleTargetAttackBuff")]
public class AttackBuff : Skill
{
    [Header("공격 배율")]
    [SerializeField]
    float atkRate;
    [SerializeField]
    float duringTime;
    [SerializeField, Tooltip("선딜레이")] float preDelay;
    [SerializeField, Tooltip("후딜레이")] float postDelay;

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
            target.StartCoroutine(StartBufftimeCO(target, (int)(atkRate * self.CurAttackPoint)));

        }

        yield return waitPostDelay;
    }

    IEnumerator StartBufftimeCO(Combatable target, int amount)
    {
        yield return null;
        target.AddAtkBuff(amount);
        yield return new WaitForSeconds(duringTime);
        target.RemoveAtkBuff(amount);
    }

}
