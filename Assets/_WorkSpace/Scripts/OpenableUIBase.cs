using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenableUIBase : BaseUI
{
    [SerializeField] Button backButton;
    public Button BackButton => backButton;

    protected override void Awake()
    {
        base.Awake();
        
        if (null == backButton)
        {
            backButton = GetUI<Button>("Back Button");
        }
    }

    public virtual void OpenWindow()
    { 
        gameObject.SetActive(true);
    }

    public virtual void CloseWindow()
    {
        gameObject.SetActive(false);
    }

}
