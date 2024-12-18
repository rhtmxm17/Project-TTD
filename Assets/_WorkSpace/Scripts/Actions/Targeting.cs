using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 설계 단계에서 가상의 유닛 동작을 선언하기 위한 더미 클래스입니다
/// </summary>
public class SampleUnitClass
{
    public Transform transform { get; }
    public SampleUnitGroupClass Group { get; }
    public Status status { get; }

    public struct Status
    {
        public float hp;
        public float maxHp;
        public float atk;
    }
}

/// <summary>
/// 설계 단계에서 가상의 유닛 그룹 동작을 선언하기 위한 더미 클래스입니다
/// </summary>
public class SampleUnitGroupClass
{
    // 적 그룹(웨이브?)를 지정
    public SampleUnitGroupClass Enemy { get; }
    public List<SampleUnitClass> Members { get; }
}

public abstract class Targeting : ScriptableObject
{
    // 시전자를 매개변수로 대상을 반환받기
    // 입력과 반환 타입은 Combatable등으로 할 필요가 있을듯
    public abstract SampleUnitClass GetTarget(SampleUnitClass self);
}
