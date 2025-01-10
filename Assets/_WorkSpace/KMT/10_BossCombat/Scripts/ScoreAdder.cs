using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAdder : MonoBehaviour
{

    Combatable combatable;

    private void Awake()
    {
        combatable = GetComponent<Combatable>();
        combatable.onDamagedEvent.AddListener(((IDamageAddable)StageManager.Instance).IDamageAdd);
        if(StageManager.Instance is IProgressable)
            ((IProgressable)StageManager.Instance).IPrograssable(combatable);

    }
}
