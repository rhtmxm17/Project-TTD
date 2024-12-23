using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] TMP_Text itemName;
    [SerializeField] int itemPrice;
    [SerializeField] Image itemImage;
    [SerializeField] Button buyButton;

    private void Start()
    {
        Init();

        // 테스트용 가짜 유저 세팅
        UserDataManager.InitDummyUser(3);
    }
    private void Init()
    {
        buyButton.GetComponentInChildren<Button>().onClick.AddListener(Buy);
    }

    private void Buy()
    {
        ItemData gold = GameManager.TableData.GetItemData(1);
        ItemData tocken = GameManager.TableData.GetItemData(2);

        Debug.Log(gold.Number.Value);
        gold.Number.onValueChanged += Gold_onValueChanged;
        tocken.Number.onValueChanged += Tocken_onValueChanged;

        GameManager.UserData.StartUpdateStream()                    // DB에 갱신 요청 시작
            .SetDBValue(gold.Number, gold.Number.Value - 10)        // 골드 --
            .SetDBValue(tocken.Number, tocken.Number.Value + 2)     // 토큰 ++, 일괄로 갱신할 내용들 등록
            .Submit(OnComplete);                                    // 위에 갱신할것들 갱신요청 전송

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

}
