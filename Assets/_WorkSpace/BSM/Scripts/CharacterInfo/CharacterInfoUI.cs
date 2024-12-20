using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterInfoUI : BaseUI
{
    public TextMeshProUGUI _nameText;
    
    protected override void Awake()
    {
        base.Awake();
        UIBind();
    }

    private void UIBind()
    {
        _nameText = GetUI<TextMeshProUGUI>("NameText");
    }
    
}
