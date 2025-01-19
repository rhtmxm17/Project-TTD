using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileIconList : ScriptableObject
{
    public Sprite[] IconList => iconList;

    [SerializeField] Sprite[] iconList;
}
