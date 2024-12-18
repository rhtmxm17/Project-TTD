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
    public override SampleUnitClass GetTarget(SampleUnitClass self)
    {
        SampleUnitClass result = self;
        float minValue = 1f;

        foreach (SampleUnitClass allyUnit in self.Group.Members)
        {
            float hpPercentage = allyUnit.Status.hp / allyUnit.Status.maxHp;
            if (minValue > hpPercentage)
            {
                minValue = hpPercentage;
                result = allyUnit;
            }
        }
        return result;
    }
}
