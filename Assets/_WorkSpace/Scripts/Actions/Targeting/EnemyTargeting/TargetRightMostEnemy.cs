using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TargetRightMostEnemy))]
public class TargetRightMostEnemyLabel : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("가장 우측에 있는 적을 지정하는 타겟팅 규칙");
    }
}
#endif

public class TargetRightMostEnemy : Targeting
{
    public override Combatable GetTarget(Combatable self)
    {
        Combatable result = null;
        float distance = float.MinValue;

        if (null == self?.Group?.Enemy?.CharList)
        {
            Debug.LogWarning("의도되지 않은 동작: 적 그룹을 찾을 수 없는 상태에서 스킬 사용");
            return null;
        }

        foreach (Combatable enemyUnit in self.Group.Enemy.CharList)
        {
            if (enemyUnit.IsTaunt)
                return enemyUnit;

            float enemyDist = Vector3.Dot(enemyUnit.transform.position, Vector3.right);

            if (distance < enemyDist)
            {
                distance = enemyDist;
                result = enemyUnit;
            }
        }
        return result;
    }
}
