using UnityEngine;
using UnityEngine.UI;

public class CharacterFilterUI : BaseUI
{
    [HideInInspector] public Button _fireFilterButton;
    [HideInInspector] public Button _waterFilterButton;
    [HideInInspector] public Button _grassFilterButton;
    [HideInInspector] public Button _groundFilterButton;
    [HideInInspector] public Button _electricFilterButton;
    [HideInInspector] public Button _deffenseFilterButton;
    [HideInInspector] public Button _attackFilterButton;
    [HideInInspector] public Button _supportFilterButton;
    [HideInInspector] public Button _singleFilterButton;
    [HideInInspector] public Button _multiFilterButton;
    [HideInInspector] public Button _elementAllFilterButton;
    [HideInInspector] public Button _roleAllFilterButton;
    [HideInInspector] public Button _dragonAllFilterButton;
    
    private Button _elementFilterCloseButton;
    private Button _roleFilterCloseButton;
    private Button _dragonVeinFilterCloseButton;
    
    
    protected override void Awake()
    {
        base.Awake();
        UIBind();
        SubscribeEvent();
    }
    
    private void UIBind()
    {
        _elementFilterCloseButton = GetUI<Button>("ElementFilterCloseButton");
        _roleFilterCloseButton = GetUI<Button>("RoleFilterCloseButton");
        _dragonVeinFilterCloseButton = GetUI<Button>("DragonFilterCloseButton");
         
        //각 필터 버튼 Bind
        //속성
        _fireFilterButton = GetUI<Button>("FireFilter");
        _waterFilterButton = GetUI<Button>("WaterFilter");
        _grassFilterButton = GetUI<Button>("GrassFilter");
        _groundFilterButton = GetUI<Button>("GroundFilter");
        _electricFilterButton = GetUI<Button>("ElectricFilter");
        _elementAllFilterButton = GetUI<Button>("ElementAllFilter");
        
        //역할군
        _deffenseFilterButton = GetUI<Button>("DefensiveFilter");
        _attackFilterButton = GetUI<Button>("OffensiveFilter");
        _supportFilterButton = GetUI<Button>("SupportiveFilter");
        _roleAllFilterButton = GetUI<Button>("RoleAllFilter");
        
        //용맥
        _singleFilterButton = GetUI<Button>("SingleFilter");
        _multiFilterButton = GetUI<Button>("MultiFilter");
        _dragonAllFilterButton = GetUI<Button>("DragonAllFilter");
    }
    
    private void SubscribeEvent()
    {
        _elementFilterCloseButton.onClick.AddListener(() => transform.GetChild(0).gameObject.SetActive((false)));
        _roleFilterCloseButton.onClick.AddListener(() => transform.GetChild(1).gameObject.SetActive((false)));
        _dragonVeinFilterCloseButton.onClick.AddListener(() => transform.GetChild(2).gameObject.SetActive((false)));
        
    }
}
