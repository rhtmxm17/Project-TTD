
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

    static float[,] elementDamageGrid = new float[,]
    {  //         NONE, FIRE, WATER, WOOD, EARTH, METAL
       /*NONE*/  {1    ,1    ,1     ,1    ,1     ,1     },
       /*FIRE*/  {1    ,1    ,0.85f ,1.15f,1     ,1     },
       /*WATER*/ {1    ,1.15f,1     ,1    ,1     ,0.85f },
       /*WOOD*/  {1    ,0.85f,1     ,1    ,1.15f ,1     },
       /*EARTH*/ {1    ,1    ,1     ,0.85f,1     ,1.15f },
       /*METAL*/ {1    ,1    ,1.15f ,1    ,0.85f ,1     },
    };

}
