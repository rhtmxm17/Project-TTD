using UnityEngine;

public class CharacterInfoPopup : BaseUI
{
    [HideInInspector] public int ListIndex;
    
    private CharacterInfoController _characterInfoController;

    private GameObject _bonusPopup;
    private GameObject _tokenPopup;
    private GameObject _enhanceResultPopup;
    private GameObject _mileageUsePopup;
    
    
    
    
    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _characterInfoController = GetComponentInParent<CharacterInfoController>();
        _bonusPopup = GetUI("BonusPopup");
        _tokenPopup = GetUI("EnhanceTokenPopup");
        _enhanceResultPopup = GetUI("EnhanceResultPopup");
        _mileageUsePopup = GetUI("MileageUsePopup");
    }
    
    private void OnDisable()
    {
        InfoPopupClose();
        
        if(_bonusPopup.activeSelf) _bonusPopup.SetActive(false);
        
        if(_enhanceResultPopup.activeSelf) _enhanceResultPopup.SetActive(false);
        
        if(_mileageUsePopup.activeSelf) _mileageUsePopup.SetActive(false);
    }

    /// <summary>
    /// 상세 팝업 종료 후 탭 타입 변경
    /// </summary>
    private void InfoPopupClose()
    {
        if (_characterInfoController.CurCharacterInfo != null)
        {
            if (_characterInfoController.CurCharacterInfo.CharacterModels.Count > 0)
            {
                _characterInfoController.CurCharacterInfo.CharacterModels[ListIndex].gameObject.SetActive(false);
            } 
        } 
        
        _characterInfoController.StartListSort();
        _characterInfoController._infoUI._detailTabColor.color = Color.cyan;
        _characterInfoController._infoUI._enhanceTabColor.color = Color.white;
        _characterInfoController._infoUI._evolutionTabColor.color = Color.white;
        _characterInfoController.CurInfoTabType = InfoTabType.DETAIL;

        gameObject.SetActive(false);
    }
}
