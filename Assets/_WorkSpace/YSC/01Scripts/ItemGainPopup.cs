using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    [Header("프리펩")]
    [SerializeField] ItemGainCell itemGainCellPrefab; // 획득 아이템 정보 출력칸 프리팹
    [Header("child")]
    [SerializeField] LayoutGroup layoutGroup; // 획득 아이템 목록이 표시될 영역


    [SerializeField] Button backGroundButton; // '빈 곳을 클릭해 닫기'의 빈 곳


    [SerializeField] ItemGainPopup itemGainPopup;   // 아이템획득팝업 프리펩




    // 아이템 정보 팝업창
    [Header("아이템 정보 팝업")]
    [SerializeField] public ShopPopupController infoPopup;
    [SerializeField] ItemData[] _items;
    [SerializeField] RectTransform _ItemContent;    // 아이템 프리펩만드는 위치


   // [SerializeField] List<ItemData> _items;
    [SerializeField] private List<ItemGainCell> _itemGainList; // 위에 itemGainCellPrefab 프리펩으로 
    //[SerializeField] List<ItemGainInfo> _iteminfo;

    private void Awake()
    {
        itemGainPopup = GetComponent<ItemGainPopup>();
        backGroundButton.onClick.AddListener(onPopupClosed);
    }

    private void Start()
    {
        //Initialize(_iteminfo);
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
         //_itemGainList.Add(itemGainCellPrefab.GetComponent<ItemGainCell>());
         _itemGainList = new List<ItemGainCell>(_items.Length);

        for (int i = 0; i < _items.Length; i++)
        {
            int index = i;
            ItemGainCell cell = Instantiate(itemGainCellPrefab, layoutGroup.transform);
            Debug.Log($"리스트확인인덱스 {index} ");
            _itemGainList.Add(cell.GetComponent<ItemGainCell>());

            cell.SetItem(_items[index]);

            cell.GetComponent<Button>().onClick.AddListener(() => Popup(itemGainCellPrefab));
            cell.GameObject();
        }
    }


    private void Popup(ItemGainCell data)
    {
         ShopPopupController popup = Instantiate(infoPopup, GameManager.PopupCanvas);
         popup.Initialize(data);
    }
    private void StageClearReward(StageData data)
    {
        ItemGainPopup popup = Instantiate(itemGainPopup, GameManager.PopupCanvas);
       // popup.Initialize(data.Reward);
    }

}


/* ListUpItems에 forloop쓰던
//   foreach (ItemData gains in _items)
//   {
//       int index = _items.IndexOf(gains); // for문을 돌려야될거같음
//       ItemGainCell cell = Instantiate(itemGainCellPrefab, layoutGroup.transform);
//       Debug.Log($"리스트확인인덱스 {index} ");
//       cell.SetItem(_item[index]);
//
//       cell.GetComponent<Button>().onClick.AddListener(() => Popup(itemGainCellPrefab));
//   }
*/