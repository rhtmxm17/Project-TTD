using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStacker : MonoBehaviour
{
    [SerializeField] OutskirtsUI outskirtsUI;

    private void OnEnable()
    {
        outskirtsUI.UIStack.Push(this.gameObject);
    }
}
