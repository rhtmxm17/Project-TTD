using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterList : MonoBehaviour
{
    [HideInInspector] public List<CharacterInfo> _characters;

    private void OnEnable() => UpdateCharacterList();

    private void UpdateCharacterList()
    {
        _characters = GetComponentsInChildren<CharacterInfo>().ToList();
    }
}