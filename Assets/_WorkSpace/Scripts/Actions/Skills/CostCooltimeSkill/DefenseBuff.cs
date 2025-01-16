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
        Debug.Log("<color=red>공격 나감!</color>");

        if (target != null && target.IsAlive)
        {
            target.StartCoroutine(StartBufftimeCO(target, (int)(defRate * self.CurAttackPoint)));

        }

        yield return waitPostDelay;

    }

    IEnumerator StartBufftimeCO(Combatable target, int amount)
    {
        yield return null;
        target.AddDefBuff(amount);
        yield return new WaitForSeconds(duringTime);
        target.RemoveDefBuff(amount);
    }
}
