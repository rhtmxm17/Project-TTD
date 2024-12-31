using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableWindow : MonoBehaviour
{

    public event Action onOpenAction;
    public event Action onCloseAction;

    public virtual void OpenWindow()
    {
        gameObject.SetActive(true);
        onOpenAction?.Invoke();
    }
    
    public virtual void CloseWindow()
    {
        onCloseAction?.Invoke();
        gameObject.SetActive(false);
    }
}
