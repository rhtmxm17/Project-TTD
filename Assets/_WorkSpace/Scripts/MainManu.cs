using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManu : BaseUI
{
    private CanvasSwitch canvasSwitch;


    protected override void Awake()
    {
        base.Awake();

        GameManager.UserData.TryInitDummyUserAsync(0, () =>
        {
            Debug.Log("완료");
        });
    }
}
