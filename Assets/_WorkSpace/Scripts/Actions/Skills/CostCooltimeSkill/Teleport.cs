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

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return null;

        if (target != null && target.IsAlive)
        {
            self.transform.position = target.transform.position + teleportOffset;
        }
    }

}
