using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public struct ItemGainInfo
{
    [SerializeField] public ItemData item;
    [SerializeField] public int number;
}
public class ItemGainPopup : MonoBehaviour
{
    public event UnityAction onPopupClosed;

    // [Header("만들프리펩")]
    // [SerializeField] public List<ItemGainCell> itemGains;

    [Header("prefabs")]
    [SerializeField] ItemGainCell itemGainCellPrefab; // 획득 아이템 정보 출력칸 프리팹
    [Header("child")]
    [SerializeField] LayoutGroup layoutGroup; // 획득 아이템 목록이 표시될 영역


    [SerializeField] Button backGroundButton; // '빈 곳을 클릭해 닫기'의 빈 곳


    [SerializeField] ItemGainPopup itemGainPopup;


    // 아이템 정보 팝업창
    [SerializeField] public ShopPopupController infoPopup;
    [SerializeField] Button infoPopupButton;
    [SerializeField] ItemData _item;
    [SerializeField] RectTransform _ItemContent;


    [SerializeField] List<ItemData> _items;
    [SerializeField] private List<ItemGainCell> _itemGainList;

    private void Awake()
    {
        itemGainPopup = GetComponent<ItemGainPopup>();
       //  backGroundButton.onClick.AddListener(onPopupClosed);
       // _item = GetComponent<ItemData>();
       // infoPopupButton = itemGainCellPrefab.GetComponent<Button>();
       // infoPopupButton.onClick.AddListener(() => Popup(_item));
    }
    private void Start()
    {
        ListUpItems();
    }
    /// <summary>
    /// 획득한 아이템 목록을 등록하는 초기화 함수
    /// </summary>
    /// <param name="gain">획득한 아이템과 개수 목록</param>
    public void Initialize(List<ItemGainInfo> gain)
    {
         foreach (ItemGainInfo info in gain)
         {
             ItemGainCell instance = Instantiate(itemGainCellPrefab, layoutGroup.transform);

            // 정보 출력칸 초기화
            // 별도의 클래스 선언 대신 itemGainCell을 BaseUI 타입으로 넣어서 바인딩 해도 무방
            instance.spriteImage = info.item.SspriteImage;
            instance.NumberText = info.number.ToString();
         }
    }
    private void ListUpItems()
    {
        _itemGainList = new List<ItemGainCell>(_items.Count);

        foreach (ItemData gains in _items)
        {
            ItemGainCell cell = Instantiate(itemGainCellPrefab, layoutGroup.transform);
            // cell.SetItem(_item);
            cell.GetComponent<Button>().onClick.AddListener(() => Popup(_item));
        }

    }
    private void Popup(ItemData data)
    {
         ShopPopupController popup = Instantiate(infoPopup, _ItemContent);
         popup.Initialize(data);
    }
    private void StageClearReward(StageData data)
    {
        ItemGainPopup popup = Instantiate(itemGainPopup, GameManager.PopupCanvas);
       // popup.Initialize(data.Reward);
    }

}
