using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveClearCheat : MonoBehaviour
{
    [SerializeField]
    GameObject monsterWaveParent;


    public void OnClick()
    { 

        var managers = monsterWaveParent.GetComponentsInChildren<CombManager>();
        Combatable[] monsters = null;
        foreach (var manager in managers)
        { 
            monsters = manager.GetComponentsInChildren<Combatable>();
            if (monsters.Length > 0)
                break;
        }

        if (monsters == null)
            return;

        foreach (Combatable m in monsters)
        {
            m.Damaged(float.MaxValue / 2, 999999);
        }

    }

}
