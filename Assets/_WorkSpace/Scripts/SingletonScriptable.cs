using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScriptable<T> : ScriptableObject where T : SingletonScriptable<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T[] assets = Resources.LoadAll<T>("");
                if (assets == null)
                {
                    Debug.LogError($"생성된 {typeof(T).Name} 타입 애셋이 없습니다.");
                    return null;
                }
                else if (assets.Length != 1)
                {
                    Debug.LogWarning($"생성된 {typeof(T).Name} 타입 애셋이 여러개 입니다.");
                }
                instance = assets[0];
            }
            return instance;
        }
    }
}
