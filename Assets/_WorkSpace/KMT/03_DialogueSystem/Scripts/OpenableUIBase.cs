using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableUIBase : MonoBehaviour
{

    public virtual void OpenWindow()
    { 
        gameObject.gameObject.SetActive(true);
    }

    public virtual void CloseWindow()
    {
        gameObject.gameObject.SetActive(false);
    }

}
