using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfo : BaseUI
{
    
    [SerializeField] private GameObject _bossInfoCanvas;
    private Button _bossInfoButton;
    
    protected override void Awake()
    {
        base.Awake();
        Init();
        ButtonOnClickListener();
    }

    private void Init()
    {
        _bossInfoButton = GetUI<Button>("InfoButton");
    }

    private void ButtonOnClickListener()
    {
        _bossInfoButton.onClick.AddListener(() => _bossInfoCanvas.SetActive(true));
    }

}
