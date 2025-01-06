using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator
{
    /// <summary>
    /// 데미지 계산식
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="igDefRate">공격의 방어 무시율</param>
    /// <param name="targetDef">피격 대상의 방어력</param>
    /// <param name="targetDefConst">비격 대상의 방어상수 </param>
    /// <returns></returns>
    public static float Calc(float damage, float igDefRate, float targetDef, float targetDefConst)
    {
        return Mathf.Max(damage - ((targetDef * (1 - igDefRate)) + targetDefConst), 0);
    }
}
