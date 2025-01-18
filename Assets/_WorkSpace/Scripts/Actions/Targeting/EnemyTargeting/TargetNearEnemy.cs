using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TargetNearEnemy))]
public class TargetNearEnemyLabel : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("가장 가까운 적을 지정하는 타겟팅 규칙");
    }
}
#endif

/// <summary>
/// 가장 가까운 적을 지정하는 타겟팅 규칙
/// </summary>
public class TargetNearEnemy : Targeting
{
    public override Combatable GetTarget(Combatable self)
    {
        Combatable result = null;
        float minSqrMagnitude = float.MaxValue;

        if (null == self?.Group?.Enemy?.CharList)
        {
            Debug.LogWarning("의도되지 않은 동작: 적 그룹을 찾을 수 없는 상태에서 스킬 사용");
            return null;
        }

        foreach (Combatable enemyUnit in self.Group.Enemy.CharList)
        {
            if (enemyUnit.IsTaunt)
                return enemyUnit;

            float sqrMagnitude = Vector3.SqrMagnitude(self.transform.position - enemyUnit.transform.position);
            if (minSqrMagnitude > sqrMagnitude)
            {
                minSqrMagnitude = sqrMagnitude;
                result = enemyUnit;
            }
        }
        return result;
    }
}
