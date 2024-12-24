using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : BaseUI
{
    [Header("아이템 이름")]
    [SerializeField] TMP_Text itemName;
    [Header("아이템 가격")]
    [SerializeField] int itemPrice;      // UI에 표기되는 가격표   
    [Header("아이템 갯수")]
    [SerializeField] int itemCount;      // UI에 표기되는 갯수
    [Header("아이템 이미지")]
    [SerializeField] Image itemImage;
    // [SerializeField] Sprite spriteImage;
    [Header("구매버튼")]
    [SerializeField] Button buyButton;
    [Header("얻는 아이템 (Ex. 토큰)")]
    [SerializeField] int getCount;       // UI에 표기되는 아이템갯수?
    [Header("매진")]
    [SerializeField] bool isSoldOut;
    [Header("설명")]
    [SerializeField] string _description;

    [SerializeField] private TMP_Text buyButtonText;
    [SerializeField] private ItemData itemGive;         // 살때 주는 재화 데이터 Ex. gold
    [SerializeField] private ItemData itemGet;          // 살때 받는 아이템      Ex. Token

    // 아이템
    private ItemData _item;

    private void Start()
    {
        Init();

        // 테스트용 가짜 유저 세팅
        UserDataManager.InitDummyUser(3);
    }
    private void Init()
    {
        buyButton.GetComponentInChildren<Button>().onClick.AddListener(Buy);
        buyButtonText = GetUI<TMP_Text>("BuyButtonText");
    }

    // ItemData랑 맞는지 확인해야함.
    public void SetItem(ItemData item)
    {
        _item = item;
        itemName.text = item.name;
        itemImage.sprite = item.SspriteImage;
        _description = item.Description;

    }
    private void Buy()
    {
        itemGive = GameManager.TableData.GetItemData(1);
        itemGet = GameManager.TableData.GetItemData(2);
        // ItemData gold = GameManager.TableData.GetItemData(1);
        // ItemData tocken = GameManager.TableData.GetItemData(2);


        Debug.Log(itemGive.Number.Value);
        itemGive.Number.onValueChanged += Gold_onValueChanged;
        itemGet.Number.onValueChanged += Tocken_onValueChanged;

        GameManager.UserData.StartUpdateStream()                    // DB에 갱신 요청 시작
            .SetDBValue(itemGive.Number, itemGive.Number.Value - 10)        // 골드 --
            .SetDBValue(itemGet.Number, itemGet.Number.Value + 2)     // 토큰 ++, 일괄로 갱신할 내용들 등록
            .Submit(OnComplete);                                    // 위에 갱신할것들 갱신요청 전송
        
        // TODO:
        // 구매하는 아이템에 따라 요구하는 재화, 받는 아이템 바꿀 수 있도록.


        itemCount--;
        SoldOut();
    }


    // 갱신 효과 결과반환
    private void OnComplete(bool result)
    {
        if (false == result)
        {
            Debug.Log($"네트워크 오류");
            return;
        }
        Debug.Log($"구매 하였습니다.");
    }


    // UserDataXX 타입의 값이 갱신되면 통지받음
    private void Gold_onValueChanged(long num)
    {
        Debug.Log($"골드가 {num}개로 바뀜.");
    }
    private void Tocken_onValueChanged(long num)
    {
        Debug.Log($"토큰이 {num}개로 바뀜.");
    }
    public void SoldOut()
    {
        if (itemCount <= 0)
        {
            isSoldOut = true;
        }
        else
            return;
        if(isSoldOut)
        {
            buyButtonText.text = "매!\t진!";
            buyButton.onClick.RemoveListener(Buy);
            //buyButton.gameObject.SetActive(false);
            // gameObject.SetActive(false);    // 기능 다 되면 비활성화 / 지우기

        }
        // TODO : 비활성화 이미지 띄우기
        // 그냥 이미지 색 검정에다가 알파값 낮춰서 주고 구매버튼 비활성화 하면서 텍스트 매진으로 변경하면될듯

    }
}
