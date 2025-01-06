using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TargetDeadlyPercentageAlly))]
public class TargetDeadlyPercentageAllyLabel : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("가장 체력 백분율이 낮은 아군을 지정하는 타겟팅 규칙");
    }
}
#endif

/// <summary>
/// 가장 체력 백분율이 낮은 아군을 지정하는 타겟팅 규칙
/// </summary>
public class TargetDeadlyPercentageAlly : Targeting
{
    public override Combatable GetTarget(Combatable self)
    {
        Combatable result = self;
        float minValue = 1f;

        foreach (Combatable allyUnit in self.Group.CharList)
        {
            float hpPercentage = allyUnit.Hp.Value / allyUnit.MaxHp.Value;
            if (minValue > hpPercentage)
            {
                minValue = hpPercentage;
                result = allyUnit;
            }
        }
        return result;
    }
}
