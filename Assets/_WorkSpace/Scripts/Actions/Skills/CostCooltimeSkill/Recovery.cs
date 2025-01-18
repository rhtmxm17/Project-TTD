using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력 비율이 가장 낮은 아군 하나를 지속적으로 회복시키는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/SingleTargetRecorvery")]
public class Recovery : Skill
{

    [Header("지속 회복치의 총 배율 ex) 1.5f = 150%")]
    [SerializeField]
    float healMultiplier;
    [SerializeField]
    int duaringSec;

    // 캐싱 데이터
    private WaitForSeconds waitPreDelay;
    private WaitForSeconds waitPostDelay;

    private void OnEnable()
    {
        // 런타임 진입시 필요한 데이터 캐싱
        waitPreDelay = new WaitForSeconds(preDelay);
        waitPostDelay = new WaitForSeconds(postDelay);
    }

    WaitForSeconds ticDelay = new WaitForSeconds(1);

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return waitPreDelay;

        self.PlayAttckSnd();

        float healAmount = self.CurAttackPoint * healMultiplier / duaringSec;//수치 조정

        if (target != null && target.IsAlive)
        {
            target.StartCoroutine(SkillRoutineImplement(target, healAmount));
        }

        yield return waitPostDelay;

    }

    IEnumerator SkillRoutineImplement(Combatable target, float healAmountPerTic)
    {
        yield return null;

        for (int i = 0; i < duaringSec; i++)
        {
            target.HealedByRecovery(healAmountPerTic);
            yield return ticDelay;
        }
    }

}
