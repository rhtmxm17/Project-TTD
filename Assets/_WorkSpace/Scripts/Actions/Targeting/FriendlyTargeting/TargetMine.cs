using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TargetMine))]
public class TargetMineLabel : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("본인을 지정하는 타겟팅 규칙");
    }
}
#endif


public class TargetMine : Targeting
{
    public override Combatable GetTarget(Combatable self)
    {
        return self;
    }
}
