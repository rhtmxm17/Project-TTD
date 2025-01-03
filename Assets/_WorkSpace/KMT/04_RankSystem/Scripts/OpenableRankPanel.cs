using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableRankPanel : OpenableUIBase
{
    [SerializeField]
    OpenableWindow rankUI;


    public override void CloseWindow()
    {
        rankUI.CloseWindow();
        base.CloseWindow();
    }
}
