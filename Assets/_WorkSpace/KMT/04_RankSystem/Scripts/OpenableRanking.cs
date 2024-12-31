using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableRanking : OpenableUIBase
{
    [SerializeField]
    OpenableWindow rankUI;

    public override void CloseWindow()
    {
        rankUI.CloseWindow();
        base.CloseWindow();
    }
}
