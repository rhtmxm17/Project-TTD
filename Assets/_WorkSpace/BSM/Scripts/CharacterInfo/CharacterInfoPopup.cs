using UnityEngine;

public class CharacterInfoPopup : MonoBehaviour
{
    private CharacterInfoController _characterInfoController;
    
    [HideInInspector] public int ListIndex;
    
    
    private void Awake()
    {
        _characterInfoController = GetComponentInParent<CharacterInfoController>();
    }
    
    private void OnDisable()
    {
        InfoPopupClose();
    }

    /// <summary>
    /// 상세 팝업 종료 후 탭 타입 변경
    /// </summary>
    private void InfoPopupClose()
    {
        _characterInfoController.CurCharacterInfo.CharacterModels[ListIndex].gameObject.SetActive(false);
        _characterInfoController.StartListSort();
        _characterInfoController._infoUI._detailTabColor.color = Color.cyan;
        _characterInfoController._infoUI._enhanceTabColor.color = Color.white;
        _characterInfoController._infoUI._evolutionTabColor.color = Color.white;
        _characterInfoController.CurInfoTabType = InfoTabType.DETAIL;

        gameObject.SetActive(false);
    }
}
