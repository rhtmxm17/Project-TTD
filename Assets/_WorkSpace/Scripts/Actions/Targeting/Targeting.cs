using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targeting : ScriptableObject
{
    /// <summary>
    /// 오버라이드된 타겟팅 로직에 따라 타깃 대상을 반환.
    /// </summary>
    /// <param name="self">본인의 CombManager그룹</param>
    /// <returns>타겟팅 대상 반환, 없으면 null 반환하도록 오버라이드하기.</returns>
    public abstract Combatable GetTarget(Combatable self);
}
