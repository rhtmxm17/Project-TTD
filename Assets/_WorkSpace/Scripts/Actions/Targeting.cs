using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targeting : ScriptableObject
{
    // 시전자를 매개변수로 대상을 반환받기
    // 입력과 반환 타입은 Combatable등으로 할 필요가 있을듯
    public abstract Combatable GetTarget(Combatable self);
}
