using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError($"{typeof(T).Name} singleton이 등록되지 않음");
            }
            return instance;
        }
    }

    protected static void RegisterSingleton(T instance)
    {
        if (SingletonBehaviour<T>.instance != null)
        {
            Debug.LogError($"{typeof(T).Name} singleton 중복 등록 시도됨");
            Destroy(instance);
        }
        else
        {
            SingletonBehaviour<T>.instance = instance;
        }
    }
}
