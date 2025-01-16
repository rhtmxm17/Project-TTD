using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossInfoPanel : BaseUI
{
    [SerializeField] private string _bossDescription;
    [SerializeField] private List<CharacterData> _bossDatas;
    private Button _bossInfoCloseButton;
    
    private TextMeshProUGUI _bossDescText;

    private Image _bossElementIconImage;
    private Image _skillAIcon;
    private Image _skillBIcon;
    
    protected override void Awake()
    {
        base.Awake();
        Init();
        ButtonOnClickListener();
        SetBossInfo();
    }

    private void Init()
    {
        _bossDescText = GetUI<TextMeshProUGUI>("BossDescText");
        
        _skillAIcon = GetUI<Image>("SkillIconA");
        _skillBIcon = GetUI<Image>("SkillIconB");
        _bossElementIconImage = GetUI<Image>("BossElementIconImage");
        
        _bossInfoCloseButton = GetUI<Button>("BossInfoCloseButton");
    }

    private void ButtonOnClickListener()
    {
        _bossInfoCloseButton.onClick.AddListener(() => transform.parent.gameObject.SetActive(false));
    }
    
    private void SetBossInfo()
    {
        _bossElementIconImage.sprite = _bossDatas[0].FaceIconSprite;
        _bossDescText.text = _bossDescription;
        _skillAIcon.sprite = _bossDatas[0].NormalSkillIcon;
        _skillBIcon.sprite = _bossDatas[0].SpecialSkillIcon;
    }
    
}
