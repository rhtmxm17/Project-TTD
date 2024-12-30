using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterLevelUpCheat : MonoBehaviour
{
    
    private CharacterInfoController _characterController;

    private Button _cheatConfirmButton;
    private TMP_InputField _inputfield;
    private int _characterLevel;
    
    private void Start()
    {
        _characterController = FindObjectOfType<CharacterInfoController>();
        
        _inputfield = transform.GetChild(1).GetComponent<TMP_InputField>();
        _cheatConfirmButton = transform.GetChild(2).GetComponent<Button>(); 
        _inputfield.onValueChanged.AddListener(x=> InputfieldTextChanged(x));
        _cheatConfirmButton.onClick.AddListener(() => _characterController.CurCharacterInfo.LevelUpCheat(_characterLevel));
    }

    private void InputfieldTextChanged(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _characterLevel = 0;
            return;
        }
        
        _characterLevel = Convert.ToInt32(text); 
    }
    
    
}
