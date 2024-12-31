using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableRanking : OpenableUIBase
{
    [SerializeField]
    OpenableWindow rankUI;
    [SerializeField]
    MyRankSetter rankBlock;

    public override void OpenWindow()
    {
        base.OpenWindow();
        rankBlock.SetRankBlock();
    }

    public override void CloseWindow()
    {
        rankUI.CloseWindow();
        base.CloseWindow();
    }
}
