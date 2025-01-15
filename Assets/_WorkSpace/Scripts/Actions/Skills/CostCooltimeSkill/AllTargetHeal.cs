using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 준비 동작 후 즉발로 모든 아군을 회복시키는 샘플
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/AllTargetHeal")]
public class AllTargetHeal : Skill
{
    [SerializeField] float healMultiplier = 1f;
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

        if (self.Group != null)
        {
            foreach (Combatable friendlyCharacter in self.Group.CharList)
            {
                // 실제로 힐이 적용되는 구간
                if (friendlyCharacter != null && friendlyCharacter.IsAlive)
                    friendlyCharacter.Healed(self.CurAttackPoint * healMultiplier); // 타겟에게 힐 적용
            }
        }

        // 히트스캔 이펙트 추가

        yield return waitPostDelay;
    }
}
