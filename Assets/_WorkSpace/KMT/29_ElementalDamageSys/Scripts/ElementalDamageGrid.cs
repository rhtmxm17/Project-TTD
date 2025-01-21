
public static class ElementalDamageGrid
{
    /// <summary>
    /// 속성에 따른 데미지 배율 계산
    /// </summary>
    /// <param name="attackType">데미지를 가하는 측의 속성</param>
    /// <param name="recievedType">데미지를 받는 측의 속성</param>
    /// <returns></returns>
    public static float GetDamageRate(ElementType attackType, ElementType recievedType)
    {
        return elementDamageGrid[(int)attackType, (int)recievedType];
    }

    /// <summary>
    /// 속성에 따른 상성 계산
    /// </summary>
    /// <param name="attackType">데미지를 가하는 측의 속성</param>
    /// <param name="recievedType">데미지를 받는 측의 속성</param>
    /// <returns>[유리속성 : 1, 정속성 : 0, 역속성 : -1]</returns>
    public static int GetDamageRateInt(ElementType attackType, ElementType recievedType)
    {
        return elementDamageGridInt[(int)attackType, (int)recievedType];
    }

    static float[,] elementDamageGrid = new float[,]
    {  //         NONE, FIRE, WATER, WIND, EARTH, METAL
       /*NONE*/  {1    ,1    ,1     ,1    ,1     ,1     },
       /*FIRE*/  {1    ,1    ,0.85f ,1    ,1.15f ,1     },
       /*WATER*/ {1    ,1.15f,1     ,0.85f,1     ,1     },
       /*WIND*/  {1    ,1    ,1.15f ,1    ,1     ,0.85f },
       /*EARTH*/ {1    ,0.85f,1     ,1    ,1     ,1.15f },
       /*METAL*/ {1    ,1    ,1     ,1.15f,0.85f ,1     },
    };

    /// <summary>
    /// 상성표, attack => damaged [유리속성 : 1, 정속성 : 0, 역속성 : -1]
    /// </summary>
    static int[,] elementDamageGridInt = new int[,]
    {  //         NONE, FIRE, WATER, WIND, EARTH, METAL
        /*NONE*/  {0    ,0    ,0    ,0    ,0     ,0     },
        /*FIRE*/  {0    ,0    ,-1   ,0    ,1     ,0     },
        /*WATER*/ {0    ,1    ,0    ,-1   ,0     ,0     },
        /*WIND*/  {0    ,0    ,1    ,0    ,0     ,-1    },
        /*EARTH*/ {0    ,-1   ,0    ,0    ,0     ,1     },
        /*METAL*/ {0    ,0    ,0    ,1    ,-1    ,0     },
    };

}
