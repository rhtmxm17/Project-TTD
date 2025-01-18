using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 본인에게 도발을 거는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/TauntSelf")]
public class TauntSelf : Skill
{
    [Header("지속시간")]
    [SerializeField]
    float duaringTime;

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
            target.StartCoroutine(TauntSkillCO(target));
        }

        yield return waitPostDelay;

    }

    IEnumerator TauntSkillCO(Combatable target)
    {
        yield return null;
        target.AddTauntCount();
        yield return new WaitForSeconds(duaringTime);
        target.RemoveTauntCount();

    }
}
