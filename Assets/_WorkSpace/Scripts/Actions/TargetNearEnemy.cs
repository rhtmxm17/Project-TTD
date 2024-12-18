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
    public override SampleUnitClass GetTarget(SampleUnitClass self)
    {
        SampleUnitClass result = null;
        float minSqrMagnitude = float.MaxValue;

        foreach (SampleUnitClass enemyUnit in self.Group.Enemy.Members)
        {
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
