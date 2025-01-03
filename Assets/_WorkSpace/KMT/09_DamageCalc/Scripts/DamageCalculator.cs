using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator
{
    public static float Calc(float damage, float igDefRate, float targetDef, float targetDefConst)
    {
        return Mathf.Max(damage - ((targetDef * (1 - igDefRate)) + targetDefConst), 0);
    }
}
