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
    //추후 레벨, 전투력으로 정렬
    NONE, NAME, LEVEL 
}

[System.Serializable]
public enum InfoTabType
{
    DETAIL, ENHANCE, EVOLUTION, MEAN
}

[System.Serializable]
public enum ElementType
{
    FIRE, WATER, GRASS, GROUND, ELECTRIC
}

[System.Serializable]
public enum RoleType
{
    
}