using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableUIBase : MonoBehaviour
{

    public virtual void OpenWindow()
    { 
        gameObject.SetActive(true);
    }

    public virtual void CloseWindow()
    {
        gameObject.SetActive(false);
    }

}
