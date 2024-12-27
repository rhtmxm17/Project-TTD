using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFilter : MonoBehaviour
{
    
    private CharacterFilterUI _characterFilterUI;


    private void Awake()
    {
        _characterFilterUI = GetComponent<CharacterFilterUI>();
    }

    private void Start()
    {
        
    }

    private void SubscribeFilterEvent()
    {
        _characterFilterUI._fireFilterButton.onClick.AddListener(()=>Debug.Log("빠이어"));
    }
    
    private void FilterEventFunc()
    {
        //0:불 / 1:물 / 2:풀 / 3:땅 / 4:번개
        
    }
    
    
    
}
