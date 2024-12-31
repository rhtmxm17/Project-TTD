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
    [SerializeField] ItemGainCell itemGainCellPrefab;   // 획득 아이템 정보 출력칸 프리팹

    [Header("child")]
    [SerializeField] LayoutGroup layoutGroup;           // 획득 아이템 목록이 표시될 영역
    [SerializeField] Button backGroundButton;           // '빈 곳을 클릭해 닫기'의 빈 곳
    [SerializeField] ItemGainPopup itemGainPopup;       // 아이템획득팝업 프리펩
    [SerializeField] Button itemGainOkButton;           // 확인버튼


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
        GameManager.UserData.TryInitDummyUserAsync(3, () =>
        {
            Debug.Log($"더미데이터 로그인 완료완료");

        });

        //Initialize(_iteminfo);
        ListUpItems();
        itemGainOkButton = transform.Find("ItemGainOkButton").GetComponent<Button>();
        itemGainOkButton.onClick.AddListener(GetItems);
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
            instance.spriteImage = info.item.SpriteImage;
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
            cell.itemImage.sprite = _items[index].SpriteImage;
            cell.GetComponent<Button>().onClick.AddListener(() => Popup(itemGainCellPrefab));
            cell.GameObject();
        }
    }
    private void GetItems()
    {
        foreach (ItemGainCell gains in _itemGainList)
        {
            Debug.Log($"루프 잘 도나 테스트 {gains}"); // 생성된 아이템에서 잘 돌긴함, 근데 밑에 아이템 추가가 안됨? 
           // GameManager.UserData.StartUpdateStream()                    // DB에 갱신 요청 시작
           //     .SetDBValue(gains.itemData.Number, gains.itemData.Number.Value + 2)     // 토큰 ++, 일괄로 갱신할 내용들 등록
           //     .Submit(OnComplete);
        }
    }

    private void OnComplete(bool result)
    {
        if (false == result)
        {
            Debug.Log($"네트워크 오류");
            return;
        }
        Debug.Log($"구매 하였습니다.");
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