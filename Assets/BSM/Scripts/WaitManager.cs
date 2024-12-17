using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitManager : SingletonBehaviour<WaitManager>
{
    
    private Dictionary<float, WaitForSeconds> _waitDict = new Dictionary<float, WaitForSeconds>();
    
    private void Awake()
    {
        RegisterSingleton(this);
    }

    public WaitForSeconds Wait(float key)
    {
        if (!_waitDict.ContainsKey(key))
        {
            _waitDict[key] = new WaitForSeconds(key); 
        }
        
        return _waitDict[key];        
    }
    
}
