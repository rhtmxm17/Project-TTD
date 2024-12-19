using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    CombManager characterManager;

    [SerializeField]
    List<CombManager> monsetWaveQueue = new List<CombManager>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            characterManager.StartCombat(monsetWaveQueue[0]);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            characterManager.EndCombat();
        }
    }
}
