using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum UnitType
{
    NONE, WATER, FIRE, GRASS, GROUND, ELECTRIC
}

[System.Serializable]
public enum BuffType
{
    NONE,
    RUSH = 1 << 11,
    BREAK_THROUGH = 1 << 10,
    GUARD = 1 << 8,
    SHELLING = 1 << 9, 
    DEFENSE = 1 << 7,
    SUPPORT = 1 << 5,
}

[System.Serializable]
public enum SortType
{
    NONE, LEVEL, POWERLEVEL, ENHANCELEVEL, OFFENSIVEPOWER, DEFENSEIVEPOWER, HEALTH
}

[System.Serializable]
public enum InfoTabType
{
    DETAIL, ENHANCE, EVOLUTION
}

[System.Serializable]
public enum ElementType
{
    //0: 화룡, 1: 수룡, 2:정룡 ? 3: 토룡, 4: 진룡
    FIRE, WATER, WOOD, EARTH, METAL, NONE
}

[System.Serializable]
public enum DragonVeinType
{
    SINGLE, MULTI, NONE
}

[System.Serializable]
public enum RoleType
{
    DEFENDER, ATTACKER, SUPPORTER, NONE
}