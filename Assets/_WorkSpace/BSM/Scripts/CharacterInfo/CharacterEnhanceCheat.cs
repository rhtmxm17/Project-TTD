using System;
using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class CharacterEnhanceCheat : MonoBehaviour
{
    private CharacterInfoController _characterController;

    private Button _cheatConfirmButton;
    private TMP_InputField _inputfield;
    private int _enhanceLevel;
    
    private void Start()
    {
        _characterController = FindObjectOfType<CharacterInfoController>();
        
        _inputfield = transform.GetChild(1).GetComponent<TMP_InputField>();
        _cheatConfirmButton = transform.GetChild(2).GetComponent<Button>(); 
        _inputfield.onValueChanged.AddListener(x=> InputfieldTextChanged(x));
        _cheatConfirmButton.onClick.AddListener(() => _characterController.CurCharacterEnhance.CheatEnhance(_enhanceLevel));
    }

    private void InputfieldTextChanged(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _enhanceLevel = 0;
            return;
        }
        
        _enhanceLevel = Math.Clamp(Convert.ToInt32(text), 0, 10); 
    }
    
}
