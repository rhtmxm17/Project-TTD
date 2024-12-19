using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    public string TestBuff = "";
    
    public int FrontCount { get; set; } 
    public int MiddleCount { get; set; } 
    public int BackCount { get; set; }

    private BuffType _curBuffType;
    private BuffType _nextBuffType;

 
    
    public void CurrentBuff()
    { 
        if (FrontCount == 0 || MiddleCount == 0 || BackCount == 0)
        {
            _curBuffType = BuffType.NONE; 
            TestBuff = _curBuffType.ToString();
            return; 
        }
        
        _nextBuffType = (BuffType)(1 << (FrontCount * 3) + (MiddleCount * 2));
        if(_nextBuffType.Equals(_curBuffType)) return;
        if(!Enum.IsDefined(typeof(BuffType), _nextBuffType)) return;
        
        _curBuffType = _nextBuffType;  
        TestBuff = _curBuffType.ToString();
    }   
    
}
