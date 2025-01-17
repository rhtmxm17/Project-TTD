using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타깃의 위치로 순간이동하는 스킬
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/TeleportToTarget")]
public class Teleport : Skill
{
    [Header("텔레포트 타깃으로부터 목적지 오프셋")]
    [SerializeField]
    Vector3 teleportOffset;
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
            self.transform.position = target.transform.position + teleportOffset;
        }

        yield return waitPostDelay;
    }

}
