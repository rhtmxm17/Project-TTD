using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyroomInitializer))]
public class MyRoomOpenable : OpenableUIBase
{

    MyroomInitializer initializer;
    OpenableWindow friendCanvas;
    OpenableWindow chatCanvas;

    protected override void Awake()
    {
        base.Awake();
        initializer = GetComponent<MyroomInitializer>();

        friendCanvas = GetUI<OpenableWindow>("FriendTapCanvas");
        chatCanvas = GetUI<OpenableWindow>("ChatCanvas");

        GetComponent<MyroomInitializer>().Initialize(this);

        CloseWindow();
    }

    public override void OpenWindow()
    {
        initializer.InitRoom(UserData.myUid);
        base.OpenWindow();
    }

    public override void CloseWindow()
    {
        friendCanvas.CloseWindow();
        chatCanvas.CloseWindow();
        base.CloseWindow();
    }
}
