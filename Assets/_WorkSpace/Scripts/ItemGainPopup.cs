using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemGainPopup : MonoBehaviour
{
    public event UnityAction onPopupClosed;
    public TMP_Text Title => title;

    [Header("프리펩")]
    [SerializeField] ItemGainCell itemGainCellPrefab;   // 획득 아이템 정보 출력칸 프리팹

    [Header("child")]
    [SerializeField] TMP_Text title;
    [SerializeField] LayoutGroup layoutGroup;           // 획득 아이템 목록이 표시될 영역
    [SerializeField] Button backGroundButton;           // '빈 곳을 클릭해 닫기'의 빈 곳
    [SerializeField] Button itemGainOkButton;           // 확인버튼

    List<ItemGainCell> items = new List<ItemGainCell>();

    private void Awake()
    {
        backGroundButton.onClick.AddListener(OnPopupOKButtonClicked);
        itemGainOkButton.onClick.AddListener(OnPopupOKButtonClicked);
    }

    protected virtual void OnPopupOKButtonClicked()
    {
        onPopupClosed?.Invoke();
        for (int i = 0; GameManager.PopupCanvas.transform.childCount > i; i++)
        {
            Destroy(GameManager.PopupCanvas.transform.GetChild(i).gameObject);
           // Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 획득한 아이템 목록을 등록하는 함수, 누적 가능
    /// </summary>
    /// <param name="gain">획득한 아이템과 개수 목록</param>
    public void AddItemGainCell(List<ItemGain> gain)
    {
         foreach (ItemGain info in gain)
         {
             ItemGainCell cell = Instantiate(itemGainCellPrefab, layoutGroup.transform);

            // 정보 출력칸 초기화
            cell.SetItem(info.item, info.gain);
            items.Add(cell);

        }
    }


    /// <summary>
    /// 캐릭터풀강시 대체지급만을 위한 획득창 / 아니면 단일아이템
    /// </summary>
    /// <param name="item"></param>
    public void AddItemGainCell(ItemData item)
    {

            ItemGainCell cell = Instantiate(itemGainCellPrefab, layoutGroup.transform);
            // 정보 출력칸 초기화
            cell.SetItem(item, 10);
            items.Add(cell);
        
    }

    public void ClearAllItems()
    {
        foreach (ItemGainCell item in items)
        {
            Destroy(item.gameObject);
        }

        items.Clear();
    }
}
