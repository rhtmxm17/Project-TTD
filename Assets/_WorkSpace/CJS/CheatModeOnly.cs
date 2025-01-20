using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheatModeOnly : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.Instance.IsCheatMode == false)
            this.gameObject.SetActive(false);
    }
}
