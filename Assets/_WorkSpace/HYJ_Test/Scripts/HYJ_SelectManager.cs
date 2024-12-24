using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class HYJ_SelectManager : MonoBehaviour
{
    //[SerializeField] public List<List<int>> unitInfo; // ListIndex,InfoIndex : InfoIndex 0은 위치, I
    [SerializeField] public int curPos;

    public Dictionary<int, int> battleInfo;
}
