using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TargetMostHpEnemy))]
public class TargetMostHpEnemyLabel : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("가장 체력 백분율이 높은 적군을 지정하는 타겟팅 규칙");
    }
}
#endif

public class TargetMostHpEnemy : Targeting
{
    public override Combatable GetTarget(Combatable self)
    {
        Combatable result = self;
        float maxValue = 1f;

        foreach (Combatable enemy in self.Group.Enemy.CharList)
        {
            float hpPercentage = enemy.Hp.Value / enemy.MaxHp.Value;
            if (maxValue < hpPercentage)
            {
                maxValue = hpPercentage;
                result = enemy;
            }
        }
        return result;
    }
}
