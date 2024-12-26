using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class HYJ_SelectManager : MonoBehaviour
{
    [SerializeField] public int curPos;
    [SerializeField] public int curUnitIndex;

    // 키 값은 위치 / 밸류 값은 유닛 고유번호;
    public Dictionary<int, int> battleInfo = new Dictionary<int, int>();

    public void LookLog()
    {
        Debug.Log("-------");
        foreach(var (key, value) in battleInfo)
        {
            Debug.Log($"{key} : {value}");
        }
        Debug.Log("-------");
    }
}
