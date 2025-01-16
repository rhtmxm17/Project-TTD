using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfo : BaseUI
{
    [SerializeField] private List<Button> _buttons;
    
    [SerializeField] private List<CharacterData> _bossData;
    [SerializeField] private string _bossDescription;
    
    private TextMeshProUGUI _bossDescText;
    
    private Image _bossElementIconImage;
    private Image _skillAIcon;
    private Image _skillBIcon;
    
    private Button _bossInfoCloseButton;
    private Button _bossInfoButton;
    
    private GameObject _bossInfoPanel;
    
    protected override void Awake()
    {
        base.Awake();
        Init();
        ButtonOnClickListener();
    }

    private void Update()
    {
        
    }

    private void Init()
    {
        _bossInfoPanel = GetUI("BossInfoPanel");

        _bossDescText = GetUI<TextMeshProUGUI>("BossDescText");

        _skillAIcon = GetUI<Image>("SkillIconA");
        _skillBIcon = GetUI<Image>("SkillIconB");
        _bossElementIconImage = GetUI<Image>("BossElementIconImage");
        
        _bossInfoCloseButton = GetUI<Button>("BossInfoCloseButton");
        _bossInfoButton = GetUI<Button>("InfoButton");
    }

    private void ButtonOnClickListener()
    {
        _bossInfoButton.onClick.AddListener(() =>
        {
            _bossInfoPanel.SetActive(true);
            SetBossInfo();
        });
        _bossInfoCloseButton.onClick.AddListener(() =>
        {
            _bossInfoPanel.SetActive(false);
            ButtonReset();
        });
    }

    private void SetBossInfo()
    {
        foreach(Button btn in _buttons)
        {
            btn.interactable = false;
        }
        
        _bossElementIconImage.sprite = _bossData[0].FaceIconSprite;
        _bossDescText.text = _bossDescription;
        _skillAIcon.sprite = _bossData[0].NormalSkillIcon;
        _skillBIcon.sprite = _bossData[0].SpecialSkillIcon;
    }

    private void ButtonReset()
    {
        foreach(Button btn in _buttons)
        {
            btn.interactable = true;
        }
    }
    
}
