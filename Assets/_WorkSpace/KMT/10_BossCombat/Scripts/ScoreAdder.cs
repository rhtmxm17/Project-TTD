using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAdder : MonoBehaviour
{

    Combatable combatable;

    private void Awake()
    {
        combatable = GetComponent<Combatable>();
        combatable.onDamagedEvent.AddListener(((BossStageManager)StageManager.Instance).AddScore);
        
    }
}
